using Godot;

namespace Chess;

public static class Actions
{
	public static readonly StringName DeselectPiece = "DeselectPiece";
	public static readonly StringName Interact = "Interact";
	public static readonly StringName RotateCamera = "RotateCamera";
	public static readonly StringName ToggleMenu = "ToggleMenu";
	public static readonly StringName ToggleUi = "ToggleUi";
	
	public static StringName From(Action action)
	{
		return action switch
		{
			Action.DeselectPiece => DeselectPiece,
			Action.Interact => Interact,
			Action.RotateCamera => RotateCamera,
			Action.ToggleMenu => ToggleMenu,
			Action.ToggleUi => ToggleUi,
			_ => null,
		};
	}
}
