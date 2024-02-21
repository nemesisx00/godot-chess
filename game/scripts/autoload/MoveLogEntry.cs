namespace Chess.Autoload;

public class MoveLogEntry(BoardVector from, BoardVector to, Piece piece, Teams team)
{
	public BoardVector From { get; set; } = from;
	public BoardVector To { get; set; } = to;
	public Piece Piece { get; set; } = piece;
	public Teams Team { get; set; } = team;
	public bool Capture { get; set; } = false;
	public bool File { get; set; } = false;
	public bool Rank { get; set; } = false;

	public override string ToString()
	{
		var moveIsCapture = string.Empty;
		if(Capture)
			moveIsCapture = "x";
		
		var disambiguation = string.Empty;
		if(File && Rank)
			disambiguation = From.ToString().ToLower();
		else if(File)
			disambiguation = ((File)From.File).ToString().ToLower();
		else if(Rank)
			disambiguation = From.Rank.ToString();
		
		var icon = UnicodePiece.ByTeamPiece(Team, Piece);
		
		return $"{icon}{disambiguation}{moveIsCapture}{To.ToString().ToLower()}";
	}
}
