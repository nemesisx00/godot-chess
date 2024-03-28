using Godot;
using System.Collections.Generic;
using System.Linq;

namespace Chess.Nodes;

public partial class PieceProxy : Node3D
{
	private static class NodePaths
	{
		public static readonly NodePath Rays = "%Rays";
	}
	
	public List<RayCast3D> Rays { get; private set; } = [];
	
	public override void _Ready()
	{
		var node = GetNode<Node3D>(NodePaths.Rays);
		foreach(var ray in node.GetChildren().Cast<RayCast3D>())
		{
			Rays.Add(ray);
		}
	}
}
