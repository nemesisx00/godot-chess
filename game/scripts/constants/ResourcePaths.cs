namespace Chess;

public static class ResourcePaths
{
	public const string Assets = "res://assets/";
	public const string Chess = $"{Assets}chess/";
	public const string Nodes = "res://nodes/";
	public const string Icons = $"{Assets}icons/";
	
	public static string PieceIcon(Team team, Piece piece) => $"{Icons}{piece}-{team}-marble-iso.png".ToLower();
}
