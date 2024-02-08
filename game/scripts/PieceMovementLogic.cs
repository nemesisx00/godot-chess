using System;

namespace Chess;

public static class PieceMovementLogic
{
	public static bool IsDestinationValid(ChessPiece piece, BoardCell destination)
	{
		return piece.Type switch
		{
			Piece.Bishop => bishop(piece, destination),
			Piece.King => king(piece, destination),
			Piece.Knight => knight(piece, destination),
			Piece.Pawn => pawn(piece, destination),
			Piece.Queen => queen(piece, destination),
			Piece.Rook => rook(piece, destination),
			_ => false
		};
	}
	
	private static bool bishop(ChessPiece piece, BoardCell destination)
	{
		var fileDiff = Math.Abs((int)piece.File - (int)destination.File);
		var rankDiff = Math.Abs((int)piece.Rank - (int)destination.Rank);
		return fileDiff != 0 && fileDiff == rankDiff;
	}
	
	private static bool king(ChessPiece piece, BoardCell destination)
	{
		var fileDiff = Math.Abs((int)piece.File - (int)destination.File);
		var rankDiff = Math.Abs((int)piece.Rank - (int)destination.Rank);
		return fileDiff <= 1 && rankDiff <= 1;
	}
	
	private static bool knight(ChessPiece piece, BoardCell destination)
	{
		var fileDiff = Math.Abs((int)piece.File - (int)destination.File);
		var rankDiff = Math.Abs((int)piece.Rank - (int)destination.Rank);
		return fileDiff == 1 && rankDiff == 2
			|| fileDiff == 2 && rankDiff == 1;
	}
	
	private static bool pawn(ChessPiece piece, BoardCell destination)
	{
		var fileDiff = Math.Abs((int)piece.File - (int)destination.File);
		var rankDiff = (int)piece.Rank - (int)destination.Rank;
		
		return fileDiff == 0 && (
			piece.Team == Teams.White && (rankDiff == -1 || piece.Rank == Rank.Two && destination.Rank == Rank.Four)
			|| piece.Team == Teams.Black && (rankDiff == 1 || (piece.Rank == Rank.Seven && destination.Rank == Rank.Five))
		);
	}
	
	private static bool queen(ChessPiece piece, BoardCell destination)
	{
		var fileDiff = Math.Abs((int)piece.File - (int)destination.File);
		var rankDiff = Math.Abs((int)piece.Rank - (int)destination.Rank);
		return fileDiff == 0 || rankDiff == 0 || fileDiff == rankDiff;
	}
	
	private static bool rook(ChessPiece piece, BoardCell destination)
	{
		var fileDiff = Math.Abs((int)piece.File - (int)destination.File);
		var rankDiff = Math.Abs((int)piece.Rank - (int)destination.Rank);
		return fileDiff == 0 || rankDiff == 0;
	}
}
