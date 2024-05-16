using Chess.Nodes;

namespace Chess;

public static class UnicodePiece
{
	public static char? ByPiece(ChessPiece piece) => ByTeamPiece(piece.Team, piece.Type);
	
	public static char? ByTeamPiece(Team team, Piece piece)
	{
		return team switch
		{
			Team.Black => piece switch
			{
				Piece.Bishop => BishopBlack,
				Piece.King => KingBlack,
				Piece.Knight => KnightBlack,
				Piece.Pawn => PawnBlack,
				Piece.Queen => QueenBlack,
				Piece.Rook => RookBlack,
				_ => null,
			},
			Team.White => piece switch
			{
				Piece.Bishop => BishopWhite,
				Piece.King => KingWhite,
				Piece.Knight => KnightWhite,
				Piece.Pawn => PawnWhite,
				Piece.Queen => QueenWhite,
				Piece.Rook => RookWhite,
				_ => null,
			},
			_ => null,
		};
	}
	
	public static readonly char KingWhite = '\u2654';
	public static readonly char QueenWhite = '\u2655';
	public static readonly char RookWhite = '\u2656';
	public static readonly char BishopWhite = '\u2657';
	public static readonly char KnightWhite = '\u2658';
	public static readonly char PawnWhite = '\u2659';
	public static readonly char KingBlack = '\u265A';
	public static readonly char QueenBlack = '\u265B';
	public static readonly char RookBlack = '\u265C';
	public static readonly char BishopBlack = '\u265D';
	public static readonly char KnightBlack = '\u265E';
	public static readonly char PawnBlack = '\u265F';
}
