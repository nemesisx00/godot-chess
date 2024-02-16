using System.Collections.Generic;
using System.Linq;
using Godot;

namespace Chess;

public partial class Chessboard : Node3D
{
	[Signal]
	public delegate void CaptureEventHandler(ChessPiece attacker, ChessPiece defender);
	
	[Signal]
	public delegate void CellClickedEventHandler(BoardCell cell);
	
	[Signal]
	public delegate void ListenOnCellsEventHandler(bool active);
	
	[Signal]
	public delegate void ListenOnPiecesEventHandler(bool active, Teams team);
	
	[Export]
	public Material OverrideMaterial { get; set; }
	
	public List<BoardCell> Cells { get; private set; } = [];
	public List<ChessPiece> Pieces { get; private set; } = [];
	private Node3D piece;
	
	public override void _Ready()
	{
		ReloadTextures();
		
		var children = GetChildren();
		foreach(var child in children)
		{
			if(child is Node3D file)
			{
				foreach(var cell in file.GetChildren().Cast<BoardCell>())
				{
					cell.Clicked += handleCellClicked;
					ListenOnCells += cell.ListenForClicks;
					
					Cells.Add(cell);
				}
			}
		}
	}
	
	public void AddPiece(ChessPiece piece)
	{
		AddChild(piece);
		Pieces.Add(piece);
		ListenOnPieces += piece.ListenForClicks;
		piece.MovementFinished += handleMovementFinished;
	}
	
	public void EnableCellSelection(Teams team)
	{
		EmitSignal(SignalName.ListenOnCells, true);
		EmitSignal(SignalName.ListenOnPieces, false, (int)team);
	}
	
	public void EnablePieceSelection(Teams team)
	{
		EmitSignal(SignalName.ListenOnCells, false);
		EmitSignal(SignalName.ListenOnPieces, true, (int)team);
	}
	
	public void DetectCheck()
	{
		Cells.Where(c => c.InCheck)
			.ToList()
			.ForEach(c => c.InCheck = false);
		
		Pieces.Where(p => p.Type == Piece.King)
			.ToList()
			.ForEach(k => k.GetParent<BoardCell>().InCheck = CheckLogic.IsInCheck(k, this));
	}
	
	public void DisableAllPieceSelection()
	{
		EmitSignal(SignalName.ListenOnPieces, false, (int)Teams.Black);
		EmitSignal(SignalName.ListenOnPieces, false, (int)Teams.White);
	}
	
	public void ReloadTextures()
	{
		if(OverrideMaterial is not null)
		{
			var mesh = GetChild<MeshInstance3D>(0);
			mesh.MaterialOverride = OverrideMaterial;
		}
	}
	
	public void MovePiece(ChessPiece piece, BoardCell cell, bool teleport = false)
	{
		if(teleport)
			Utility.SetGlobalOrigin(piece, cell.GlobalTransform.Origin);
		else
			piece.Destination = cell.GlobalTransform.Origin;
		
		var captured = cell.GetChildren()
			.Where(child => child is ChessPiece)
			.Cast<ChessPiece>()
			.FirstOrDefault();
		
		piece.Reparent(cell);
		
		if(captured is not null)
			EmitSignal(SignalName.Capture, piece, captured);
	}
	
	public void MovePiece(File file, Rank rank, ChessPiece piece, bool teleport = false)
	{
		var cell = GetNode<Node3D>(file.ToString())
			.GetNode<BoardCell>($"{file}{(int)rank + 1}");
		
		MovePiece(piece, cell, teleport);
	}
	
	private void handleCellClicked(BoardCell cell) => EmitSignal(SignalName.CellClicked, cell);
	private void handleMovementFinished(ChessPiece piece) => DetectCheck();
}
