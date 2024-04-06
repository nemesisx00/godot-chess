using System;

namespace Chess;

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
	
	public static string ByDifference(BoardVector diff)
		=> ByDifference(diff.File, diff.Rank);
	
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
