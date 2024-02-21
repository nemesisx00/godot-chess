using System;
using System.Collections.Generic;
using System.Linq;
using Chess.Nodes;

namespace Chess.Gameplay;

public static class CaptureLogic
{
	public static List<BoardCell> DetectCapturableCells(ChessPiece piece, Chessboard board)
	{
		return piece.Type switch
		{
			Piece.King => king(piece),
			Piece.Knight => knight(piece, board),
			Piece.Pawn => pawn(piece),
			_ => simpleRaycast(piece),
		};
	}
	
	private static List<BoardCell> king(ChessPiece piece)
	{
		List<BoardCell> capturables = [];
		
		var pieceCell = piece.GetParent<BoardCell>();
		piece.Rays.ForEach(ray => {
			ray.ForceRaycastUpdate();
			var collider = ray.GetCollider();
			if(collider is ChessPiece cp && cp.Team != piece.Team && cp.Type != Piece.King && cp.GetParentOrNull<BoardCell>() is BoardCell cell)
			{
				var diff = pieceCell - cell;
				if(Math.Abs(diff.File) <= 1 && Math.Abs(diff.Rank) <= 1)
					capturables.Add(cell);
			}
		});
		
		return capturables;
	}
	
	private static List<BoardCell> knight(ChessPiece piece, Chessboard board)
	{
		var currentCell = piece.GetParent<BoardCell>();
		List<BoardCell> capturables = [];
		board.Cells.Where(cell => PieceMovementLogic.ValidateMovement(piece, cell))
			.Where(c => c.GetChildren().Where(child => child is ChessPiece cp && cp.Team != piece.Team && cp.Type != Piece.King).Any())
			.ToList()
			.ForEach(capturables.Add);
		return capturables;
	}
	
	private static List<BoardCell> pawn(ChessPiece piece)
	{
		List<BoardCell> capturables = [];
		
		var directionName = piece.Team switch
		{
			Teams.Black => DirectionNames.South,
			_ => DirectionNames.North,
		};
		
		var expectedRank = piece.Team switch
		{
			Teams.Black => 1,
			_ => -1,
		};
		
		var pieceCell = piece.GetParent<BoardCell>();
		
		piece.Rays.Where(ray => ray.Name.ToString().Contains(directionName))
			.ToList()
			.ForEach(ray => {
				ray.ForceRaycastUpdate();
				var collider = ray.GetCollider();
				if(collider is ChessPiece cp && cp.Team != piece.Team && cp.Type != Piece.King && cp.GetParentOrNull<BoardCell>() is BoardCell cell)
				{
					var diff = pieceCell - cell;
					if(diff.Rank == expectedRank && Math.Abs(diff.File) == 1)
						capturables.Add(cell);
				}
			});
		
		//TODO: Implement En Passant
		//En Passant requires a move log to determine if the pawn to be captured had performed its initial two cell move on the preceding turn.
		
		return capturables;
	}
	
	private static List<BoardCell> simpleRaycast(ChessPiece piece)
	{
		List<BoardCell> capturables = [];
		
		piece.Rays.ForEach(ray => {
			ray.ForceRaycastUpdate();
			var collider = ray.GetCollider();
			if(collider is ChessPiece cp && cp.Team != piece.Team && cp.Type != Piece.King && cp.GetParentOrNull<BoardCell>() is BoardCell cell)
				capturables.Add(cell);
		});
		
		return capturables;
	}
}
