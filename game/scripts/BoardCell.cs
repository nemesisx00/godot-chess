using Godot;

namespace Chess;

[GlobalClass]
public partial class BoardCell : Area3D
{
	private static class NodePaths
	{
		public static readonly NodePath Indicator = "%Indicator";
	}
	
	[Signal]
	public delegate void ClickedEventHandler(BoardCell cell);
	
	[Export]
	public File File { get; set; }
	
	[Export]
	public Rank Rank { get; set; }
	
	public MeshInstance3D Indicator { get; private set; }
	
	[Export]
	private Material indicatorMaterial;
	
	[Export]
	private Material indicatorHighlight;
	
	private bool listeningForClicks;
	
	public override void _InputEvent(Camera3D camera, InputEvent evt, Vector3 position, Vector3 normal, int shapeIdx)
	{
		if(listeningForClicks && evt is InputEventMouseButton iemb && iemb.ButtonIndex == MouseButton.Left && iemb.Pressed)
			EmitSignal(SignalName.Clicked, this);
	}
	
	public override void _MouseEnter()
	{
		Indicator.MaterialOverride = indicatorHighlight;
	}
	
	public override void _MouseExit()
	{
		Indicator.MaterialOverride = indicatorMaterial;
	}
	
	public override void _Ready()
	{
		Indicator = GetNode<MeshInstance3D>(NodePaths.Indicator);
		Indicator.MaterialOverride = indicatorMaterial;
	}
	
	public void ListenForClicks(bool active) => listeningForClicks = active;
	
	public void ToggleIndicator(bool? force = null)
	{
		var doShow = force ?? !Indicator.Visible;
		if(doShow)
			Indicator.Show();
		else
			Indicator.Hide();
	}
}
