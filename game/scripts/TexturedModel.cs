using Godot;

namespace Chess;

public partial class TexturedModel : Node3D
{
	[Export]
	public Material OverrideMaterial { get; set; }
	
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
