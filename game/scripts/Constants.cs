using System;
using Godot;

namespace Chess;

public static class Actions
{
	public static readonly StringName DeselectPiece = "DeselectPiece";
}

public static class DirectionNames
{
	public const string East = "East";
	public const string North = "North";
	public const string NorthEast = "NorthEast";
	public const string NorthWest = "NorthWest";
	public const string South = "South";
	public const string SouthEast = "SouthEast";
	public const string SouthWest = "SouthWest";
	public const string West = "West";
	
	public static bool IsCardinal(string name)
		=> East.Equals(name)
			|| North.Equals(name)
			|| South.Equals(name)
			|| West.Equals(name);
	
	public static string ByDifference(int fileDiff, int rankDiff)
	{
		var direction = string.Empty;
		if(fileDiff == 0)
		{
			if(rankDiff > 0)
				direction = South;
			else
				direction = North;
		}
		else if(rankDiff == 0)
		{
			if(fileDiff > 0)
				direction = West;
			else
				direction = East;
		}
		else if(Math.Abs(fileDiff) == Math.Abs(rankDiff))
		{
			if(fileDiff > 0)
			{
				if(rankDiff > 0)
					direction = SouthWest;
				else
					direction = NorthWest;
			}
			else
			{
				if(rankDiff > 0)
					direction = SouthEast;
				else
					direction = NorthEast;
			}
		}
		return direction;
	}
}

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
