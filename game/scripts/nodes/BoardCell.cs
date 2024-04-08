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
	
	public bool Hoverable { get; set; }
	public bool ListeningForClicks => listeningForClicks;
	
	[Export]
	private Material indicatorMaterial;
	
	[Export]
	private Material indicatorHighlight;
	
	[Export]
	private Material checkMaterial;
	
	private bool inCheck;
	private bool listeningForClicks;
	private bool mouseHovering;
	
	public override void _MouseEnter()
	{
		if(Hoverable)
			Indicator.MaterialOverride = indicatorHighlight;
		
		mouseHovering = true;
	}
	
	public override void _MouseExit()
	{
		Indicator.MaterialOverride = InCheck ? checkMaterial : indicatorMaterial;
		mouseHovering = false;
	}
	
	public override void _Process(double delta)
	{
		if(listeningForClicks && mouseHovering && Input.IsActionJustPressed(Actions.Interact))
			EmitSignal(SignalName.Clicked, this);
	}
	
	public override void _Ready()
	{
		Indicator = GetNode<MeshInstance3D>(NodePaths.Indicator);
		Indicator.MaterialOverride = InCheck ? checkMaterial : indicatorMaterial;
	}
	
	public void ListenForClicks(bool active)
	{
		Hoverable = active;
		listeningForClicks = active;
	}
	
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
