namespace Chess.Autoload;

public class MoveLogEntry(BoardVector from, BoardVector to, Piece piece, Teams team)
{
	private const string CastleKingside = "0-0";
	private const string CastleQueenside = "0-0-0";
	
	public BoardVector From { get; set; } = from;
	public BoardVector To { get; set; } = to;
	public Piece Piece { get; set; } = piece;
	public Teams Team { get; set; } = team;
	public bool Capture { get; set; }
	public bool File { get; set; }
	public bool Rank { get; set; }
	public bool Castle { get; set; }

	public override string ToString()
	{
		string notation;
		if(Castle)
			notation = From.File - To.File > 0 ? CastleQueenside : CastleKingside;
		else
		{
			var moveIsCapture = string.Empty;
			if(Capture)
				moveIsCapture = "x";
			
			var disambiguation = string.Empty;
			if((File && Rank) || (Piece == Piece.Pawn && Capture))
				disambiguation = From.ToString().ToLower();
			else if(File && Piece != Piece.Pawn)
				disambiguation = (From.Rank + 1).ToString();
			else if(Rank && Piece != Piece.Pawn)
				disambiguation = ((File)From.File).ToString().ToLower();
			
			notation = $"{disambiguation}{moveIsCapture}{To.ToString().ToLower()}";
		}
		
		var icon = UnicodePiece.ByTeamPiece(Team, Piece);
		return $"{icon}{notation}";
	}
}
