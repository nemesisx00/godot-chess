using System.Linq;
using System.Text;
using Godot;

namespace Chess.Nodes;

public partial class GameUi : MarginContainer
{
	private static class NodePaths
	{
		public static readonly NodePath CameraInput = new("%CameraInput");
		public static readonly NodePath Deselect = new("%Deselect");
		public static readonly NodePath DeselectInput = new("%DeselectInput");
		public static readonly NodePath InteractInput = new("%InteractInput");
	}
	
	[Signal]
	public delegate void DeselectPressedEventHandler();
	
	private static string formatActionText(InputEvent action)
	{
		var text = action.AsText()
			.Split(' ')
			.FirstOrDefault(string.Empty);
		
		if(action is InputEventMouseButton iemb)
		{
			switch(iemb.ButtonIndex)
			{
				case MouseButton.Left:
					text = "LMB";
					break;
				case MouseButton.Right:
					text = "RMB";
					break;
				case MouseButton.Middle:
					text = "MMB";
					break;
			}
		}
		
		return text;
	}
	
	private static string refreshInputLabel(StringName action)
	{
		StringBuilder text = new();
		
		foreach(var inputEvent in InputMap.ActionGetEvents(action))
		{
			if(text.Length > 0)
				text.Append(" or ");
			text.Append(formatActionText(inputEvent));
		}
		
		return text.ToString();
	}
	
	private Label cameraInput;
	private Button deselect;
	private Label deselectInput;
	private Label interactInput;
	
	public override void _Ready()
	{
		cameraInput = GetNode<Label>(NodePaths.CameraInput);
		deselectInput = GetNode<Label>(NodePaths.DeselectInput);
		interactInput = GetNode<Label>(NodePaths.InteractInput);
		
		GetNode<Button>(NodePaths.Deselect).Pressed += () => EmitSignal(SignalName.DeselectPressed);
		
		refreshInputLabels();
	}
	
	public new void Show()
	{
		refreshInputLabels();
		base.Show();
	}
	
	private void refreshInputLabels()
	{
		cameraInput.Text = refreshInputLabel(Actions.RotateCamera);
		deselectInput.Text = refreshInputLabel(Actions.DeselectPiece);
		interactInput.Text = refreshInputLabel(Actions.Interact);
	}
}
