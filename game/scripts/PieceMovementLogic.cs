using System;
using System.Collections.Generic;
using System.Linq;

namespace Chess;

public static class PieceMovementLogic
{
	public static List<BoardCell> GetValidCells(ChessPiece piece, List<BoardCell> cells)
	{
		List<BoardCell> possibles = [];
		
		var currentCell = piece.GetParent<BoardCell>();
		cells.Where(cell => ValidateMovement(piece, cell))
			.ToList()
			.ForEach(possibles.Add);
		
		if(piece.Type == Piece.Knight)
		{
			possibles.Where(c => c.GetChildren().Where(child => child is ChessPiece cp && cp.Team == piece.Team).Any())
				.ToList()
				.ForEach(c => possibles.Remove(c));
		}
		else
			possibles = limitByRaycast(piece, possibles);
		
		possibles.Remove(currentCell);
		
		return possibles;
	}
	
	public static bool IsDestinationValid(ChessPiece piece, BoardCell destination, List<BoardCell> cells)
	{
		var validCells = GetValidCells(piece, cells);
		return validCells.Contains(destination);
	}
	
	public static bool ValidateMovement(ChessPiece piece, BoardCell destination)
	{
		return piece.Type switch
		{
			Piece.Bishop => bishop(piece.GetParent<BoardCell>(), destination),
			Piece.King => king(piece.GetParent<BoardCell>(), destination),
			Piece.Knight => knight(piece.GetParent<BoardCell>(), destination),
			Piece.Pawn => pawn(piece.GetParent<BoardCell>(), destination, piece.Team),
			Piece.Queen => queen(piece.GetParent<BoardCell>(), destination),
			Piece.Rook => rook(piece.GetParent<BoardCell>(), destination),
			_ => false
		};
	}
	
	private static bool bishop(BoardCell currentCell, BoardCell destination)
	{
		var fileDiff = Math.Abs((int)currentCell.File - (int)destination.File);
		var rankDiff = Math.Abs((int)currentCell.Rank - (int)destination.Rank);
		return fileDiff != 0 && fileDiff == rankDiff;
	}
	
	private static bool king(BoardCell currentCell, BoardCell destination)
	{
		var fileDiff = Math.Abs((int)currentCell.File - (int)destination.File);
		var rankDiff = Math.Abs((int)currentCell.Rank - (int)destination.Rank);
		return fileDiff <= 1 && rankDiff <= 1;
	}
	
	private static bool knight(BoardCell currentCell, BoardCell destination)
	{
		var fileDiff = Math.Abs((int)currentCell.File - (int)destination.File);
		var rankDiff = Math.Abs((int)currentCell.Rank - (int)destination.Rank);
		return fileDiff == 1 && rankDiff == 2
			|| fileDiff == 2 && rankDiff == 1;
	}
	
	private static List<BoardCell> limitByRaycast(ChessPiece piece, List<BoardCell> possibles)
	{
		var result = possibles;
		piece.Rays.ForEach(ray => {
			ray.ForceRaycastUpdate();
			
			var collider = ray.GetCollider();
			if(collider is ChessPiece cp)
			{
				var blockingCell = cp.GetParent<BoardCell>();
				var collisionIsValidCell = result.Contains(blockingCell);
				
				Func<BoardCell, bool> whereClause = cell => false;
				switch(ray.Name)
				{
					case DirectionNames.East:
						whereClause = cell => cell.Rank == blockingCell.Rank && cell.File >= blockingCell.File;
						break;
					
					case DirectionNames.North:
						whereClause = cell => cell.File == blockingCell.File && cell.Rank >= blockingCell.Rank;
						break;
					
					case DirectionNames.NorthEast:
						whereClause = cell => Math.Abs(cell.File - blockingCell.File) == Math.Abs(cell.Rank - blockingCell.Rank)
							&& cell.File >= blockingCell.File
							&& cell.Rank >= blockingCell.Rank;
						break;
					
					case DirectionNames.NorthWest:
						whereClause = cell => Math.Abs(cell.File - blockingCell.File) == Math.Abs(cell.Rank - blockingCell.Rank)
							&& cell.File <= blockingCell.File
							&& cell.Rank >= blockingCell.Rank;
						break;
					
					case DirectionNames.South:
						whereClause = cell => cell.File == blockingCell.File && cell.Rank <= blockingCell.Rank;
						break;
					
					case DirectionNames.SouthEast:
						whereClause = cell => Math.Abs(cell.File - blockingCell.File) == Math.Abs(cell.Rank - blockingCell.Rank)
							&& cell.File >= blockingCell.File
							&& cell.Rank <= blockingCell.Rank;
						break;
					
					case DirectionNames.SouthWest:
						whereClause = cell => Math.Abs(cell.File - blockingCell.File) == Math.Abs(cell.Rank - blockingCell.Rank)
							&& cell.File <= blockingCell.File
							&& cell.Rank <= blockingCell.Rank;
						break;
					
					case DirectionNames.West:
						whereClause = cell => cell.Rank == blockingCell.Rank && cell.File <= blockingCell.File;
						break;
				}
				
				result.Where(whereClause)
					.ToList()
					.ForEach(cell => result.Remove(cell));
				
				if(collisionIsValidCell && cp.Team != piece.Team)
					result.Add(blockingCell);
			}
		});
		
		return result;
	}
	
	private static bool pawn(BoardCell currentCell, BoardCell destination, Teams team)
	{
		var fileDiff = Math.Abs((int)currentCell.File - (int)destination.File);
		var rankDiff = (int)currentCell.Rank - (int)destination.Rank;
		
		return fileDiff == 0 && (
			team == Teams.White && (rankDiff == -1 || currentCell.Rank == Rank.Two && destination.Rank == Rank.Four)
			|| team == Teams.Black && (rankDiff == 1 || (currentCell.Rank == Rank.Seven && destination.Rank == Rank.Five))
		);
	}
	
	private static bool queen(BoardCell currentCell, BoardCell destination)
	{
		var fileDiff = Math.Abs((int)currentCell.File - (int)destination.File);
		var rankDiff = Math.Abs((int)currentCell.Rank - (int)destination.Rank);
		return fileDiff == 0 || rankDiff == 0 || fileDiff == rankDiff;
	}
	
	private static bool rook(BoardCell currentCell, BoardCell destination)
	{
		var fileDiff = Math.Abs((int)currentCell.File - (int)destination.File);
		var rankDiff = Math.Abs((int)currentCell.Rank - (int)destination.Rank);
		return fileDiff == 0 || rankDiff == 0;
	}
}
