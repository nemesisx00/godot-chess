using System;
using System.Collections.Generic;
using System.Linq;

namespace Chess;

public static class CheckLogic
{
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
					{
						if(cardinal)
							check = cp.Type == Piece.Queen || cp.Type == Piece.Rook;
						else
						{
							check = cp.Type == Piece.Bishop || cp.Type == Piece.Queen;
							if(!check)
							{
								var fileDist = Math.Abs(kingCell.File - colliderCell.File);
								var rankDist = kingCell.Rank - colliderCell.Rank;
								
								check = cp.Type == Piece.Pawn && fileDist == 1
									&& (
										(cp.Team == Teams.White && rankDist == 1)
										|| (cp.Team == Teams.Black && rankDist == -1)
									);
							}
						}
					}
				}
			}
		}
		
		if(!check)
		{
			foreach(var knight in board.Pieces.Where(p => p.Team != king.Team && p.Type == Piece.Knight))
			{
				if(check)
					break;
				
				check = PieceMovementLogic.GetValidCells(knight, board.Cells).Contains(kingCell);
			}
		}
		
		return check;
	}
}
