using Godot;

namespace Chess.Nodes;

public partial class Credits : Control
{
	private static class NodePaths
	{
		public static readonly NodePath GodotLicense = new("%GodotLicenseText");
	}
	
	public override void _Ready()
	{
		GetNode<Label>(NodePaths.GodotLicense).Text = Engine.GetLicenseText();
	}
}
