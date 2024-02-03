using Godot;

namespace Chess;

public static class BoardPosition
{
	private const float PositionStep = 0.057f;
	private static readonly Vector3 A1 = new(-.2f, .03f, .2f);
	
	public static Vector3 FromCoordinate(File file, Rank rank)
	{
		var position = A1;
		
		position.X += PositionStep * (float)file;
		position.Z -= PositionStep * (float)rank;
		
		return position;
	}
	
	public static void MoveToCoordinate(File file, Rank rank, ref ChessPiece piece)
	{
		var origin = FromCoordinate(file, rank);
		var transform = piece.Transform;
		transform.Origin = origin;
		piece.Transform = transform;
	}
}
