using System.Collections.Generic;
using System.Linq;
using Godot;
using Chess.Gameplay;
using Chess.Autoload;
using System;

namespace Chess.Nodes;

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
	
	[Signal]
	public delegate void PieceHasMovedEventHandler();
	
	[Export]
	public Material OverrideMaterial { get; set; }
	
	public List<BoardCell> Cells { get; private set; } = [];
	public List<ChessPiece> Pieces { get; private set; } = [];
	
	private MoveLog moveLog;
	private Node3D piece;
	
	public override void _Ready()
	{
		moveLog = GetNode<MoveLog>(MoveLog.NodePath);
		
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
	
	public void DisableAllCellSelection() => EmitSignal(SignalName.ListenOnCells, false);
	
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
		{
			piece.Destination = cell;
			tryCastling(piece, cell);
		}
		
		var captured = cell.GetChildren()
			.Where(child => child is ChessPiece && child != piece)
			.Cast<ChessPiece>()
			.FirstOrDefault();
		
		piece.Reparent(cell);
		
		if(!teleport)
			piece.HasMoved = true;
		
		if(captured is not null)
			EmitSignal(SignalName.Capture, piece, captured);
	}
	
	public void MovePiece(File file, Rank rank, ChessPiece piece, bool teleport = false)
	{
		var cell = GetNode<Node3D>(file.ToString())
			.GetNode<BoardCell>($"{file}{(int)rank + 1}");
		
		MovePiece(piece, cell, teleport);
	}
	
	public void ResetPieces()
	{
		MovePiece(File.C, Rank.One, Pieces.Where(p => p.Team == Teams.White && p.Type == Piece.Bishop && p.PieceNumber == 1).First());
		MovePiece(File.F, Rank.One, Pieces.Where(p => p.Team == Teams.White && p.Type == Piece.Bishop && p.PieceNumber == 2).First());
		MovePiece(File.C, Rank.Eight, Pieces.Where(p => p.Team == Teams.Black && p.Type == Piece.Bishop && p.PieceNumber == 1).First());
		MovePiece(File.F, Rank.Eight, Pieces.Where(p => p.Team == Teams.Black && p.Type == Piece.Bishop && p.PieceNumber == 2).First());
		
		MovePiece(File.E, Rank.One, Pieces.Where(p => p.Team == Teams.White && p.Type == Piece.King).First());
		MovePiece(File.E, Rank.Eight, Pieces.Where(p => p.Team == Teams.Black && p.Type == Piece.King).First());
		
		MovePiece(File.B, Rank.One, Pieces.Where(p => p.Team == Teams.White && p.Type == Piece.Knight && p.PieceNumber == 1).First());
		MovePiece(File.G, Rank.One,  Pieces.Where(p => p.Team == Teams.White && p.Type == Piece.Knight && p.PieceNumber == 2).First());
		MovePiece(File.B, Rank.Eight, Pieces.Where(p => p.Team == Teams.Black && p.Type == Piece.Knight && p.PieceNumber == 1).First());
		MovePiece(File.G, Rank.Eight, Pieces.Where(p => p.Team == Teams.Black && p.Type == Piece.Knight && p.PieceNumber == 2).First());
		
		MovePiece(File.A, Rank.Two, Pieces.Where(p => p.Team == Teams.White && p.Type == Piece.Pawn && p.PieceNumber == 1).First());
		MovePiece(File.B, Rank.Two, Pieces.Where(p => p.Team == Teams.White && p.Type == Piece.Pawn && p.PieceNumber == 2).First());
		MovePiece(File.C, Rank.Two, Pieces.Where(p => p.Team == Teams.White && p.Type == Piece.Pawn && p.PieceNumber == 3).First());
		MovePiece(File.D, Rank.Two, Pieces.Where(p => p.Team == Teams.White && p.Type == Piece.Pawn && p.PieceNumber == 4).First());
		MovePiece(File.E, Rank.Two, Pieces.Where(p => p.Team == Teams.White && p.Type == Piece.Pawn && p.PieceNumber == 5).First());
		MovePiece(File.F, Rank.Two, Pieces.Where(p => p.Team == Teams.White && p.Type == Piece.Pawn && p.PieceNumber == 6).First());
		MovePiece(File.G, Rank.Two, Pieces.Where(p => p.Team == Teams.White && p.Type == Piece.Pawn && p.PieceNumber == 7).First());
		MovePiece(File.H, Rank.Two, Pieces.Where(p => p.Team == Teams.White && p.Type == Piece.Pawn && p.PieceNumber == 8).First());
			
		MovePiece(File.A, Rank.Seven, Pieces.Where(p => p.Team == Teams.Black && p.Type == Piece.Pawn && p.PieceNumber == 1).First());
		MovePiece(File.B, Rank.Seven, Pieces.Where(p => p.Team == Teams.Black && p.Type == Piece.Pawn && p.PieceNumber == 2).First());
		MovePiece(File.C, Rank.Seven, Pieces.Where(p => p.Team == Teams.Black && p.Type == Piece.Pawn && p.PieceNumber == 3).First());
		MovePiece(File.D, Rank.Seven, Pieces.Where(p => p.Team == Teams.Black && p.Type == Piece.Pawn && p.PieceNumber == 4).First());
		MovePiece(File.E, Rank.Seven, Pieces.Where(p => p.Team == Teams.Black && p.Type == Piece.Pawn && p.PieceNumber == 5).First());
		MovePiece(File.F, Rank.Seven, Pieces.Where(p => p.Team == Teams.Black && p.Type == Piece.Pawn && p.PieceNumber == 6).First());
		MovePiece(File.G, Rank.Seven, Pieces.Where(p => p.Team == Teams.Black && p.Type == Piece.Pawn && p.PieceNumber == 7).First());
		MovePiece(File.H, Rank.Seven, Pieces.Where(p => p.Team == Teams.Black && p.Type == Piece.Pawn && p.PieceNumber == 8).First());
		
		MovePiece(File.D, Rank.One, Pieces.Where(p => p.Team == Teams.White && p.Type == Piece.Queen).First());
		MovePiece(File.D, Rank.Eight, Pieces.Where(p => p.Team == Teams.Black && p.Type == Piece.Queen).First());
		
		MovePiece(File.A, Rank.One, Pieces.Where(p => p.Team == Teams.White && p.Type == Piece.Rook && p.PieceNumber == 1).First());
		MovePiece(File.H, Rank.One, Pieces.Where(p => p.Team == Teams.White && p.Type == Piece.Rook && p.PieceNumber == 2).First());
		
		MovePiece(File.A, Rank.Eight, Pieces.Where(p => p.Team == Teams.Black && p.Type == Piece.Rook && p.PieceNumber == 1).First());
		MovePiece(File.H, Rank.Eight, Pieces.Where(p => p.Team == Teams.Black && p.Type == Piece.Rook && p.PieceNumber == 2).First());
		
		Pieces.ForEach(p => p.HasMoved = false);
		moveLog.Clear();
	}
	
	private void tryCastling(ChessPiece piece, BoardCell cell)
	{
		if(piece.Type == Piece.King
			&& piece.GetParentOrNull<BoardCell>() is BoardCell start
			&& (start - cell) is BoardVector diff
			&& Math.Abs(diff.File) == 2)
		{
			var rookFile = diff.File > 0 ? File.A : File.H;
			var fileMod = diff.File > 0 ? -1 : 1;
			
			var rook = Cells.Where(cell => cell.Rank == start.Rank && cell.File == rookFile)
				.First()
				.GetChildren()
				.Where(node => node is ChessPiece cp && cp.Type == Piece.Rook && cp.Team == piece.Team)
				.Cast<ChessPiece>()
				.FirstOrDefault();
			
			var rookDest = Cells.Where(cell => cell.Rank == start.Rank && cell.File == (start.File + fileMod))
				.FirstOrDefault();
			
			if(rook is not null && rookDest is not null)
				MovePiece(rook, rookDest);
		}
	}
	
	private void handleCellClicked(BoardCell cell) => EmitSignal(SignalName.CellClicked, cell);
	
	private void handleMovementFinished(BoardCell from, BoardCell to, ChessPiece piece)
	{
		//Is this piece the rook who was moved immediately after the king moved as a part of castling?
		if(piece.Type == Piece.Rook && moveLog.MostRecentEntry.Piece == Piece.King && moveLog.MostRecentEntry.Team == piece.Team
			&& Math.Abs(moveLog.MostRecentEntry.From.File - moveLog.MostRecentEntry.To.File) == 2)
		{
			moveLog.MostRecentEntry.Castle = true;
		}
		else
		{
			//Workaround because from may be null if a piece is moved from a graveyard back onto the board (i.e. when starting a new game)
			MoveLogEntry entry = new(from?.ToVector() ?? default, to.ToVector(), piece.Type, piece.Team);
			//TODO: Evaluate if disambiguation is necessary and set entry.File and entry.Rank to true as needed
			//TODO: Detect if a capture occurred and set entry.Capture to true as needed
			moveLog.AddEntry(entry);
			EmitSignal(SignalName.PieceHasMoved);
		}
		
		DetectCheck();
	}
}
