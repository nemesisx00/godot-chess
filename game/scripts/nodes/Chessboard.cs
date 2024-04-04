using System;
using System.Collections.Generic;
using System.Linq;
using Godot;
using Chess.Gameplay;
using Chess.Autoload;

namespace Chess.Nodes;

public partial class Chessboard : Node3D
{
	private static class NodePaths
	{
		public static readonly NodePath PieceProxy = new("%PieceProxy");
	}
	
	[Signal]
	public delegate void CaptureEventHandler(ChessPiece attacker, ChessPiece defender);
	
	[Signal]
	public delegate void CellClickedEventHandler(BoardCell cell);
	
	[Signal]
	public delegate void CheckmateEventHandler(Teams winner);
	
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
	public PieceProxy PieceProxy { get; private set; }
	
	private MoveLog moveLog;
	private Node3D piece;
	
	public override void _Ready()
	{
		moveLog = GetNode<MoveLog>(MoveLog.NodePath);
		PieceProxy = GetNode<PieceProxy>(NodePaths.PieceProxy);
		
		ReloadTextures();
		
		foreach(var child in GetChildren())
		{
			foreach(var n in child.GetChildren())
			{
				if(n is BoardCell cell)
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
	
	public void EnableCellSelection(Teams team, bool listenOnPieces = false)
	{
		EmitSignal(SignalName.ListenOnCells, true);
		EmitSignal(SignalName.ListenOnPieces, listenOnPieces, (int)team);
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
			.ForEach(k => k.GetParent<BoardCell>().InCheck = CheckLogic.IsInCheck(k, this, moveLog));
		
		if(moveLog.MostRecentEntry is not null && CheckLogic.DetectCheckmateForTeam(moveLog.MostRecentEntry.Team.NextTeam(), this, moveLog))
			EmitSignal(SignalName.Checkmate, (int)moveLog.MostRecentEntry.Team);
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
		var captured = cell.GetChildren()
			.Where(child => child is ChessPiece && child != piece)
			.Cast<ChessPiece>()
			.FirstOrDefault();
		var hasCaptured = captured is not null;
		
		if(teleport)
			Utility.SetGlobalOrigin(piece, cell.GlobalTransform.Origin);
		else
		{
			piece.SetDestination(cell, hasCaptured);
			tryCastling(piece, cell);
		}
		
		piece.Reparent(cell);
		
		if(hasCaptured)
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
	
	private void handleMovementFinished(BoardCell from, BoardCell to, ChessPiece piece, bool capture)
	{
		//Is this piece the rook who was moved immediately after the king moved as a part of castling?
		if(piece.Type == Piece.Rook && moveLog.MostRecentEntry.Piece == Piece.King && moveLog.MostRecentEntry.Team == piece.Team
			&& Math.Abs(moveLog.MostRecentEntry.From.File - moveLog.MostRecentEntry.To.File) == 2)
		{
			moveLog.MostRecentEntry.Castle = true;
			moveLog.ForceUpdate();
		}
		else
		{
			var piecesQuery = Pieces.Where(p => p != piece && p.Team == piece.Team && p.Type == piece.Type);
			
			//En Passant will not be detected by the normal means of capturing so check for it here when appropriate.
			if(piece.Type == Piece.Pawn && !capture && from?.File != to.File)
			{
				var previous = Cells.Where(c => c.File == to.File && c.Rank == to.Rank + (piece.Team == Teams.Black ? 1 : -1))
					.FirstOrDefault();
				
				if(previous is not null && previous.GetChildren().Where(c => c is ChessPiece cp && cp.Type == Piece.Pawn && cp.Team != piece.Team).FirstOrDefault() is ChessPiece cp)
				{
					capture = true;
					EmitSignal(SignalName.Capture, piece, cp);
				}
			}
			
			//Workaround because from may be null if a piece is moved from a graveyard back onto the board (i.e. when starting a new game)
			MoveLogEntry entry = new(from?.ToVector() ?? default, to.ToVector(), piece.Type, piece.Team)
			{
				Capture = capture,
				File = piecesQuery.Where(p => p.GetParentOrNull<BoardCell>() is BoardCell otherCell && from?.File == otherCell.File).Any(),
				Rank = piecesQuery.Where(p => p.GetParentOrNull<BoardCell>() is BoardCell otherCell && from?.Rank == otherCell.Rank).Any(),
				FirstMove = !piece.HasMoved,
			};
			
			moveLog.AddEntry(entry);
			piece.HasMoved = true;
			EmitSignal(SignalName.PieceHasMoved);
		}
		
		DetectCheck();
	}
}
