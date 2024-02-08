using Godot;

namespace Chess;

public partial class ChessPiece : CharacterBody3D
{
	[Signal]
	public delegate void ClickedEventHandler(ChessPiece piece);
	
	[Export]
	public Material OverrideMaterial { get; set; }
	
	[Export(PropertyHint.Range, ".1,1,.1")]
	private float moveDuration = .2f;
	
	private static readonly NodePath GlobalPositionPath = "global_position";
	
	public bool DiagonalMovement { get; set; }
	public File File { get; set; }
	public Rank Rank { get; set; }
	public Teams Team { get; set; }
	
	public Vector3 Destination
	{
		get => destination;
		set
		{
			destination = value;
			
			if(!Vector3.Zero.IsEqualApprox(destination))
			{
				tween?.Kill();
				tween = CreateTween()
					.BindNode(this)
					.SetEase(Tween.EaseType.InOut)
					.SetLoops(1)
					.SetParallel(DiagonalMovement)
					.SetTrans(Tween.TransitionType.Linear);
				
				tween.Finished += handleTweenFinished;
				
				if(!Mathf.IsEqualApprox(GlobalPosition.X, destination.X))
				{
					var one = destination;
					one.X = GlobalPosition.X;
					var two = destination;
					
					tween.TweenProperty(this, GlobalPositionPath, one, moveDuration);
					tween.TweenProperty(this, GlobalPositionPath, two, moveDuration);
				}
				else
					tween.TweenProperty(this, GlobalPositionPath, destination, moveDuration);
			}
		}
	}
	
	private Vector3 destination;
	
	private bool isFalling = true;
	private Tween tween;
	private bool mouseActive;
	
	public override void _InputEvent(Camera3D camera, InputEvent evt, Vector3 position, Vector3 normal, int shapeIdx)
	{
		if(evt is InputEventMouseButton iemb && iemb.ButtonIndex == MouseButton.Left && iemb.Pressed)
			EmitSignal(SignalName.Clicked, this);
	}
	
	public override void _PhysicsProcess(double delta)
	{
		var velocity = Velocity;
		
		if(isFalling)
		{
			velocity.Y -= GlobalSettings.Gravity * (float)delta;
			
			if(IsOnFloor())
				isFalling = false;
		}
		
		Velocity = velocity;
		
		if(!Vector3.Zero.IsEqualApprox(Velocity))
			MoveAndSlide();
	}
	
	public override void _Ready() => ReloadTextures();
	
	public void ReloadTextures()
	{
		if(OverrideMaterial is not null)
		{
			var mesh = GetChild<MeshInstance3D>(0);
			mesh.MaterialOverride = OverrideMaterial;
		}
	}
	
	public void StartFalling() => isFalling = true;
	
	private void handleTweenFinished()
	{
		Destination = Vector3.Zero;
		isFalling = true;
	}
}
