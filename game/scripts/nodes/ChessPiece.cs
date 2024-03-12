using System.Collections.Generic;
using System.Linq;
using Godot;

namespace Chess.Nodes;

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
	
	[Signal]
	public delegate void MovementFinishedEventHandler(BoardCell from, BoardCell to, ChessPiece piece, bool capture);
	
	[Export]
	public Material OverrideMaterial { get; set; }
	
	[Export(PropertyHint.Range, ".1,1,.1")]
	private float moveDuration = .2f;
	
	[Export]
	public bool DiagonalMovement { get; set; }
	
	[Export]
	public Piece Type { get; set; }
	
	public Teams Team { get; set; }
	public bool HasMoved { get; set; }
	public int PieceNumber { get; set; } = 1;
	
	public BoardCell Destination
	{
		get => destination;
		
		set
		{
			destination = value;
			doMovementTween();
		}
	}
	
	public void SetDestination(BoardCell dest, bool capture = false)
	{
		willCapture = capture;
		Destination = dest;
	}
	
	public List<RayCast3D> Rays { get; private set; } = [];
	
	private BoardCell destination;
	
	private bool isFalling = true;
	private bool listeningForClicks = true;
	private Tween tween;
	private MeshInstance3D selectionIndicator;
	private bool willCapture;
	
	public override void _InputEvent(Camera3D camera, InputEvent evt, Vector3 position, Vector3 normal, int shapeIdx)
	{
		if(evt is InputEventMouseButton iemb && iemb.IsActionPressed(Actions.Interact))
		{
			if(listeningForClicks)
				EmitSignal(SignalName.Clicked, this);
			else if(GetParentOrNull<BoardCell>() is BoardCell cell)
				cell.EmitSignal(BoardCell.SignalName.Clicked, cell);
		}
	}
	
	public override void _MouseEnter()
	{
		if(GetParentOrNull<BoardCell>() is BoardCell cell)
			cell._MouseEnter();
	}
	
	public override void _MouseExit()
	{
		if(GetParentOrNull<BoardCell>() is BoardCell cell)
			cell._MouseExit();
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
		
		if(PieceNumber > 1)
			Name = $"{Name}{PieceNumber}";
		
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
	
	public void ListenForClicks(bool active, Teams team)
	{
		if(team == Team)
			listeningForClicks = active;
	}
	
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
		var dest = Destination?.GlobalTransform.Origin ?? Vector3.Zero;
		if(!Vector3.Zero.IsEqualApprox(dest))
		{
			var current = GetParent<Node3D>();
			var time = current.GlobalPosition.DistanceTo(dest) * moveDuration;
			
			tween?.Kill();
			tween = CreateTween()
				.BindNode(this)
				.SetEase(Tween.EaseType.InOut)
				.SetLoops(1)
				.SetParallel(DiagonalMovement)
				.SetTrans(Tween.TransitionType.Linear);
			
			if(!Mathf.IsEqualApprox(GlobalPosition.X, dest.X))
			{
				var one = dest;
				one.X = GlobalPosition.X;
				tween.TweenProperty(this, GlobalPositionPath, one, time);
			}
			
			tween.TweenProperty(this, GlobalPositionPath, dest, time);
			
			BoardCell from = null;
			if(current is BoardCell bc)
				from = bc;
			
			tween.Finished += () => EmitSignal(SignalName.MovementFinished, from, Destination, this, willCapture);
			tween.Finished += handleTweenFinished;
		}
	}
	
	private void handleTweenFinished()
	{
		Destination = null;
		isFalling = true;
		willCapture = false;
	}
}
