using System.Collections.Generic;
using System.Linq;
using Godot;

namespace Chess;

public partial class ChessPiece : CharacterBody3D
{
	private static class NodePaths
	{
		public static readonly NodePath Rays = "%Rays";
		public static readonly NodePath SelectionIndicator = "%SelectionIndicator";
	}
	
	private static readonly NodePath GlobalPositionPath = "global_position";
	
	[Signal]
	public delegate void ClickedEventHandler(ChessPiece piece);
	
	[Export]
	public Material OverrideMaterial { get; set; }
	
	[Export(PropertyHint.Range, ".1,1,.1")]
	private float moveDuration = .2f;
	
	[Export]
	public bool DiagonalMovement { get; set; }
	
	[Export]
	public Piece Type { get; set; }
	
	public Teams Team { get; set; }
	
	public Vector3 Destination
	{
		get => destination;
		
		set
		{
			destination = value;
			doMovementTween();
		}
	}
	
	public List<RayCast3D> Rays { get; private set; } = [];
	
	private Vector3 destination;
	
	private bool isFalling = true;
	private bool listeningForClicks = true;
	private Tween tween;
	private MeshInstance3D selectionIndicator;
	
	public override void _InputEvent(Camera3D camera, InputEvent evt, Vector3 position, Vector3 normal, int shapeIdx)
	{
		if(listeningForClicks && evt is InputEventMouseButton iemb && iemb.ButtonIndex == MouseButton.Left && iemb.Pressed)
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
	
	public override void _Ready()
	{
		Name = $"{Team}{Type}";
		
		if(Team == Teams.White)
		{
			var angle = Mathf.Tau / 2;
			GetChild<MeshInstance3D>(0).RotateObjectLocal(Vector3.Up, angle);
			GetChild<CollisionShape3D>(1).RotateObjectLocal(Vector3.Up, angle);
		}
		
		ReloadTextures();
		
		selectionIndicator = GetNode<MeshInstance3D>(NodePaths.SelectionIndicator);
		
		GetNodeOrNull(NodePaths.Rays)?
			.GetChildren()
			.Cast<RayCast3D>()
			.ToList()
			.ForEach(Rays.Add);
	}
	
	public void ReloadTextures()
	{
		if(OverrideMaterial is not null)
		{
			var mesh = GetChild<MeshInstance3D>(0);
			mesh.MaterialOverride = OverrideMaterial;
		}
	}
	
	public void ListenForClicks(bool active) => listeningForClicks = active;
	public void StartFalling() => isFalling = true;
	
	public void ToggleSelected(bool? force = null)
	{
		var doShow = force ?? !selectionIndicator.Visible;
		if(doShow)
			selectionIndicator.Show();
		else
			selectionIndicator.Hide();
	}
	
	private void doMovementTween()
	{
		if(!Vector3.Zero.IsEqualApprox(Destination))
		{
			var current = GetParent<BoardCell>();
			var time = current.GlobalPosition.DistanceTo(Destination) * moveDuration;
			
			tween?.Kill();
			tween = CreateTween()
				.BindNode(this)
				.SetEase(Tween.EaseType.InOut)
				.SetLoops(1)
				.SetParallel(DiagonalMovement)
				.SetTrans(Tween.TransitionType.Linear);
			
			tween.Finished += handleTweenFinished;
			
			if(!Mathf.IsEqualApprox(GlobalPosition.X, Destination.X))
			{
				var one = Destination;
				one.X = GlobalPosition.X;
				tween.TweenProperty(this, GlobalPositionPath, one, time);
			}
			
			tween.TweenProperty(this, GlobalPositionPath, Destination, time);
		}
	}
	
	private void handleTweenFinished()
	{
		Destination = Vector3.Zero;
		isFalling = true;
	}
}
