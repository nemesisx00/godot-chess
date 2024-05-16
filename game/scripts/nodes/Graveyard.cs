using System;
using Godot;

namespace Chess.Nodes;

public partial class Graveyard : Node3D
{
	private static class NodePaths
	{
		public static readonly NodePath Mesh = new("Mesh");
	}
	
	[Export]
	public Team Team { get; set; }
	
	[Export]
	public Material OverrideMaterial { get; set; }
	
	public override void _Ready()
	{
		if(OverrideMaterial is not null)
			GetNode<MeshInstance3D>(NodePaths.Mesh).MaterialOverride = OverrideMaterial;
	}
	
	public void BuryPiece(ChessPiece deceased)
	{
		Random rand = new();
		
		var origin = Vector3.Up;
		origin.X += rand.NextSingle() - 0.5f;
		origin.Z += rand.NextSingle() * 4 - 2f;
		
		deceased.Reparent(this);
		var transform = deceased.Transform;
		transform.Origin = origin;
		deceased.Transform = transform;
		deceased.StartFalling();
	}
}
