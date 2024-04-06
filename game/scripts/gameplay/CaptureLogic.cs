using System;
using System.Collections.Generic;
using System.Linq;
using Chess.Autoload;
using Chess.Nodes;

namespace Chess.Gameplay;

public static class CaptureLogic
{
	public static List<BoardCell> DetectCapturableCells(ChessPiece piece, Chessboard board, MoveLog moveLog)
	{
		return piece.Type switch
		{
			Piece.King => king(piece),
			Piece.Knight => knight(piece, board),
			Piece.Pawn => pawn(piece, board, moveLog),
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
		board.Cells.Where(cell => MoveLogic.ValidateMovement(piece, cell))
			.Where(c => c.GetChildren().Where(child => child is ChessPiece cp && cp.Team != piece.Team && cp.Type != Piece.King).Any())
			.ToList()
			.ForEach(capturables.Add);
		return capturables;
	}
	
	private static List<BoardCell> pawn(ChessPiece piece, Chessboard board, MoveLog moveLog)
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
		
		piece.Rays.Where(ray => ray.Name.ToString() == DirectionNames.East || ray.Name.ToString() == DirectionNames.West)
			.ToList()
			.ForEach(ray => {
				ray.ForceRaycastUpdate();
				var collider = ray.GetCollider();
				
				if(moveLog.MostRecentEntry?.Piece == Piece.Pawn && moveLog.MostRecentEntry?.FirstMove == true
					&& collider is ChessPiece cp && cp.Team != piece.Team && cp.Type == Piece.Pawn && cp.GetParentOrNull<BoardCell>() is BoardCell cell
					&& moveLog.MostRecentEntry?.To == cell.ToVector() && Math.Abs((moveLog.MostRecentEntry?.From - moveLog.MostRecentEntry?.To)?.Rank ?? 0) == 2)
				{
					var dest = board.Cells.Where(bc => bc.File == cell.File && bc.Rank == cell.Rank + (expectedRank * -1))
						.FirstOrDefault();
					
					if(dest is not null)
						capturables.Add(dest);
				}
			});
		
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
