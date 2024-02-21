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
	public Teams Team { get; set; }
	
	[Export]
	public Material OverrideMaterial { get; set; }
	
	private MeshInstance3D mesh;
	
	public override void _Ready()
	{
		mesh = GetNode<MeshInstance3D>(NodePaths.Mesh);
		
		if(OverrideMaterial is not null)
			mesh.MaterialOverride = OverrideMaterial;
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
