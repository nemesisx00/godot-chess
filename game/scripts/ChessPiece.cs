using Godot;

namespace Chess;

public partial class ChessPiece : CharacterBody3D
{
	[Export]
	public Material OverrideMaterial { get; set; }
	
	public Teams Team { get; set; }

	public override void _PhysicsProcess(double delta)
	{
		var velocity = Velocity;
		if(!IsOnFloor())
			velocity.Y -= GlobalSettings.Gravity * (float)delta;
		else
			velocity.Y = 0;
		
		Velocity = velocity;
		MoveAndSlide();
	}
	
	public override void _Ready()
	{
		ReloadTextures();
	}
	
	public void ReloadTextures()
	{
		if(OverrideMaterial is not null)
		{
			var mesh = GetChild<MeshInstance3D>(0);
			mesh.MaterialOverride = OverrideMaterial;
		}
	}
}
