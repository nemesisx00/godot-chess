using Godot;

namespace Chess.Nodes;

[GlobalClass]
public partial class BoardCell : Area3D
{
	private static class NodePaths
	{
		public static readonly NodePath Indicator = "%Indicator";
	}
	
	public static BoardVector operator -(BoardCell a, BoardCell b)
		=> new(a.File - b.File, a.Rank - b.Rank);
	
	[Signal]
	public delegate void ClickedEventHandler(BoardCell cell);
	
	[Export]
	public File File { get; set; }
	
	[Export]
	public Rank Rank { get; set; }
	
	public MeshInstance3D Indicator { get; private set; }
	public bool InCheck
	{
		get => inCheck;
		set
		{
			inCheck = value;
			
			if(Indicator.MaterialOverride != indicatorHighlight)
				Indicator.MaterialOverride = inCheck ? checkMaterial : indicatorMaterial;
			
			if(inCheck)
				Indicator.Show();
			else
				Indicator.Hide();
		}
	}
	
	[Export]
	private Material indicatorMaterial;
	
	[Export]
	private Material indicatorHighlight;
	
	[Export]
	private Material checkMaterial;
	
	private bool inCheck;
	private bool listeningForClicks;
	
	public override void _InputEvent(Camera3D camera, InputEvent evt, Vector3 position, Vector3 normal, int shapeIdx)
	{
		if(listeningForClicks && evt is InputEventMouseButton iemb && iemb.ButtonIndex == MouseButton.Left && iemb.Pressed)
			EmitSignal(SignalName.Clicked, this);
	}
	
	public override void _MouseEnter() => Indicator.MaterialOverride = indicatorHighlight;
	public override void _MouseExit() => Indicator.MaterialOverride = InCheck ? checkMaterial : indicatorMaterial;
	
	public override void _Ready()
	{
		Indicator = GetNode<MeshInstance3D>(NodePaths.Indicator);
		Indicator.MaterialOverride = InCheck ? checkMaterial : indicatorMaterial;
	}
	
	public void ListenForClicks(bool active) => listeningForClicks = active;
	
	public BoardVector ToVector() => new((int)File, (int)Rank);
	
	public void ToggleIndicator(bool? force = null)
	{
		var doShow = force ?? !Indicator.Visible;
		if(doShow)
			Indicator.Show();
		else
			Indicator.Hide();
	}
}
