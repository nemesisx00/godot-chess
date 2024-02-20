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
			var collider = ray.GetCollider();
			if(collider is ChessPiece cp)
			{
				if(cp.Team != king.Team)
				{
					var colliderCell = cp.GetParentOrNull<BoardCell>();
					if(colliderCell is not null)
						check = canPieceCauseCheck(
							cp,
							DirectionNames.IsCardinal(ray.Name),
							kingCell - colliderCell
						);
				}
			}
		}
		
		if(!check)
		{
			foreach(var knight in board.Pieces.Where(p => p.Team != king.Team && p.Type == Piece.Knight && p.GetParent() is BoardCell))
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
		if(mover.Type != Piece.King && king.Team == mover.Team)
		{
			var kingCell = king.GetParent<BoardCell>();
			
			if(mover.GetParent() is BoardCell moverCell)
			{
				var diff = kingCell - moverCell;
				string direction = DirectionNames.ByDifference(diff);
				
				if(!string.IsNullOrEmpty(direction))
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
						{
							var pieceCell = cp.GetParent<BoardCell>();
							var diff2 = kingCell - pieceCell;
							
							//If the opposing piece is between the king and the mover, the mover cannot cause check by moving.
							if(diff2 < diff)
								check = canPieceCauseCheck(cp, cardinal, diff);
						}
					}
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
	private static bool canPieceCauseCheck(ChessPiece piece, bool cardinal, BoardVector diff)
	{
		bool check;
		if (cardinal)
			check = piece.Type == Piece.Queen || piece.Type == Piece.Rook;
		else
		{
			check = piece.Type == Piece.Bishop || piece.Type == Piece.Queen;
			if(!check)
			{
				check = piece.Type == Piece.Pawn && diff.File == 1
					&& (
						(piece.Team == Teams.White && diff.Rank == 1)
						|| (piece.Team == Teams.Black && diff.Rank == -1)
					);
			}
		}
		return check;
	}
}
