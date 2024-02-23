using Godot;

namespace Chess.Nodes;

public partial class MainMenu : MarginContainer
{
	private static class NodePaths
	{
		public static readonly NodePath NewGame = new("%NewGame");
		public static readonly NodePath Quit = new("%Quit");
	}
	
	[Signal]
	public delegate void StartNewGameEventHandler();
	
	public override void _Ready()
	{
		GetNode<Button>(NodePaths.NewGame).Pressed += pressedNewGame;
		GetNode<Button>(NodePaths.Quit).Pressed += pressedQuit;
	}
	
	private void pressedNewGame() => EmitSignal(SignalName.StartNewGame);
	private void pressedQuit() => GetTree().Quit();
}
