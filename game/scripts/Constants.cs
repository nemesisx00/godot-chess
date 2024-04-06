using System;
using Godot;
using Chess.Nodes;

namespace Chess;

public static class Actions
{
	public static readonly StringName DeselectPiece = "DeselectPiece";
	public static readonly StringName Interact = "Interact";
	public static readonly StringName RotateCamera = "RotateCamera";
	public static readonly StringName ToggleMenu = "ToggleMenu";
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

public static class ResourcePaths
{
	public const string Assets = "res://assets/";
	public const string Chess = $"{Assets}chess/";
	public const string Nodes = "res://nodes/";
	public const string Icons = $"{Assets}icons/";
	
	public static string PieceIcon(Team team, Piece piece) => $"{Icons}{piece}-{team}-marble-iso.png".ToLower();
}
