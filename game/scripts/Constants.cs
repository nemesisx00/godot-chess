using Godot;

namespace Chess;

public static class ChessMaterials
{
	public const string Black = $"{ResourcePaths.Chess}material-piece-black-4k.tres";
	public const string Board = $"{ResourcePaths.Chess}material-board-4k.tres";
	public const string White = $"{ResourcePaths.Chess}material-piece-white-4k.tres";
}

public static class GlobalSettings
{
	public static readonly float Gravity = (float)ProjectSettings.GetSetting("physics/3d/default_gravity");
}

public static class ResourcePaths
{
	public const string Assets = "res://assets/";
	public const string Chess = $"{Assets}chess/";
	public const string Nodes = "res://nodes/";
}
