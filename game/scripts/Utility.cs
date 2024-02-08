using Godot;

namespace Chess;

public static class Utility
{
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
