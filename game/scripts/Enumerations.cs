namespace Chess;

public enum File
{
	A,
	B,
	C,
	D,
	E,
	F,
	G,
	H,
}

public enum GameStatus
{
	NotStarted,
	Playing,
	Paused,
	Loss,
	Stalemate,
	Victory,
	Reseting,
}

public enum Piece
{
	Bishop,
	King,
	Knight,
	Pawn,
	Queen,
	Rook,
}

public enum Rank
{
	One,
	Two,
	Three,
	Four,
	Five,
	Six,
	Seven,
	Eight,
}

public enum Teams
{
	Black,
	White,
}
