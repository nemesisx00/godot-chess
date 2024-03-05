using System.Collections.Generic;
using System.Text;
using Godot;
using Chess.Nodes;

namespace Chess;

public static class Utility
{
	public static void PrintMoveList(List<BoardCell> moves)
	{
		StringBuilder sb = new('[');
		moves.ForEach(move => {
			if(sb.Length > 1)
				sb.Append(", ");
			sb.Append(move.Name);
		});
		sb.Append(']');
		GD.Print(sb);
	}
	
	public static void SetOrigin<T>(T node, Vector3 origin)
		where T: Node3D
	{
		var transform = node.Transform;
		transform.Origin = origin;
		node.Transform = transform;
	}
	
	public static void SetGlobalOrigin<T>(T node, Vector3 origin)
		where T: Node3D
	{
		var transform = node.GlobalTransform;
		transform.Origin = origin;
		node.GlobalTransform = transform;
	}
}
