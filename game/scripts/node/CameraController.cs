using Godot;

namespace Chess.Nodes;

[GlobalClass]
public partial class CameraController : Node3D
{
	[Export]
	private Vector2 Acceleration { get; set; } = new Vector2(20.0f, 20.0f);
	
	[Export]
	private Vector2 Sensitivity { get; set; } = new Vector2(0.12f, 0.12f);
	
	[Export]
	private Vector2 VerticalLimit { get; set; } = new Vector2(-55.0f, 75.0f);
	
	private Camera3D camera;
	private bool rotating;
	private Vector2 rotation;
	
	public override void _Input(InputEvent evt)
	{
		if(evt is InputEventMouseButton iemb)
		{
			if(iemb.ButtonIndex == MouseButton.Right)
			{
				rotating = iemb.Pressed;
				
				if(rotating)
					Input.MouseMode = Input.MouseModeEnum.Captured;
				else
					Input.MouseMode = Input.MouseModeEnum.Visible;
			}
		}
		
		if(rotating && evt is InputEventMouseMotion iemm)
		{
			rotation.X -= iemm.Relative.Y * Sensitivity.X;
			rotation.Y -= iemm.Relative.X * Sensitivity.Y;
		}
	}

	public override void _PhysicsProcess(double delta)
	{
		rotation.X = Mathf.Clamp(
			rotation.X,
			VerticalLimit.X,
			VerticalLimit.Y
		);
		
		var degrees = RotationDegrees;
		
		degrees.X = Mathf.Lerp(
			degrees.X,
			rotation.X,
			(float)delta * Acceleration.X
		);
		
		degrees.Y = Mathf.Lerp(
			degrees.Y,
			rotation.Y,
			(float)delta * Acceleration.Y
		);
		
		RotationDegrees = degrees;
	}
	
	public override void _Ready() => camera = GetChild<Camera3D>(0);
}
