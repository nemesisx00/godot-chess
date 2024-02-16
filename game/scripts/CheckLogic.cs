using System;
using System.Linq;

namespace Chess;

public static class CheckLogic
{
	/**
	<summary>
	Detect if a king is in check.
	</summary>
	*/
	public static bool IsInCheck(ChessPiece king, Chessboard board)
	{
		var check = false;
		var kingCell = king.GetParent<BoardCell>();
		
		foreach(var ray in king.Rays)
		{
			if(check)
				break;
			
			ray.ForceRaycastUpdate();
			
			var cardinal = DirectionNames.IsCardinal(ray.Name);
			
			var collider = ray.GetCollider();
			if(collider is ChessPiece cp)
			{
				if(cp.Team != king.Team)
				{
					var colliderCell = cp.GetParentOrNull<BoardCell>();
					if(colliderCell is not null)
						check = canPieceCauseCheck(cp, cardinal, Math.Abs(kingCell.File - colliderCell.File), kingCell.Rank - colliderCell.Rank);
				}
			}
		}
		
		if(!check)
		{
			foreach(var knight in board.Pieces.Where(p => p.Team != king.Team && p.Type == Piece.Knight))
			{
				if(check)
					break;
				
				check = PieceMovementLogic.GetValidCells(knight, board).Contains(kingCell);
			}
		}
		
		return check;
	}
	
	/**
	<summary>
	Predict if moving a piece will put the player's own king in check.
	</summary>
	*/
	public static bool PredictCheck(Chessboard board, ChessPiece mover)
		=> PredictCheck(board.Pieces.Where(p => p.Type == Piece.King && p.Team == mover.Team).First(), mover);
	
	/**
	<summary>
	Predict if moving a piece will put the player's own king in check.
	</summary>
	*/
	public static bool PredictCheck(ChessPiece king, ChessPiece mover)
	{
		//Pre-emptive evaluation to see if moving the `mover` piece will put their own `king` in check
		var check = false;
		
		//Sanity check
		if(king != mover && king.Team == mover.Team)
		{
			var kingCell = king.GetParent<BoardCell>();
			var moverCell = mover.GetParent<BoardCell>();
			
			var fileDiff = kingCell.File - moverCell.File;
			var rankDiff = kingCell.Rank - moverCell.Rank;
			
			string direction = DirectionNames.ByDifference(fileDiff, rankDiff);
			if(!String.IsNullOrEmpty(direction))
			{
				var ray = king.Rays.Where(r => r.Name.Equals(direction)).FirstOrDefault();
				if(ray is not null)
				{
					var cardinal = DirectionNames.IsCardinal(direction);
					
					ray.AddException(mover);
					ray.ForceRaycastUpdate();
					ray.RemoveException(mover);
					
					var collider = ray.GetCollider();
					if(collider is ChessPiece cp && cp.Team != king.Team)
						check = canPieceCauseCheck(cp, cardinal, fileDiff, rankDiff);
				}
			}
		}
		
		return check;
	}
	
	/**
	<summary>
	Evaluate if a given piece, based on its allowed means of movement, could cause check.
	</summary>
	*/
	private static bool canPieceCauseCheck(ChessPiece piece, bool cardinal, int fileDiff, int rankDiff)
	{
		var check = false;
		if(cardinal)
			check = piece.Type == Piece.Queen || piece.Type == Piece.Rook;
		else
		{
			check = piece.Type == Piece.Bishop || piece.Type == Piece.Queen;
			if(!check)
			{
				check = piece.Type == Piece.Pawn && fileDiff == 1
					&& (
						(piece.Team == Teams.White && rankDiff == 1)
						|| (piece.Team == Teams.Black && rankDiff == -1)
					);
			}
		}
		return check;
	}
}
