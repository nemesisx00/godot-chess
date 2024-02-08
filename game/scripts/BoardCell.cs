using Godot;

namespace Chess;

[GlobalClass]
public partial class BoardCell : Area3D
{
	[Signal]
	public delegate void ClickedEventHandler(BoardCell cell);
	
	[Export]
	public File File { get; set; }
	
	[Export]
	public Rank Rank { get; set; }
	
	private bool active;
	
	public override void _InputEvent(Camera3D camera, InputEvent evt, Vector3 position, Vector3 normal, int shapeIdx)
	{
		if(evt is InputEventMouseButton iemb && iemb.ButtonIndex == MouseButton.Left && iemb.Pressed)
			EmitSignal(SignalName.Clicked, this);
	}
}
