using Godot;

namespace Chess.Nodes;

public partial class MainMenu : MarginContainer
{
	private static class NodePaths
	{
		public static readonly NodePath Credits = new("%Credits");
		public static readonly NodePath CreditsButton = new("%CreditsButton");
		public static readonly NodePath NewGame = new("%NewGame");
		public static readonly NodePath Quit = new("%Quit");
	}
	
	[Signal]
	public delegate void StartNewGameEventHandler();
	
	public bool CreditsVisible => credits.Visible;
	
	private HBoxContainer mainRow;
	private Credits credits;
	
	public override void _UnhandledInput(InputEvent evt)
	{
		if(credits.Visible && evt is InputEventKey iek && iek.IsReleased())
		{
			credits.Hide();
			mainRow.Show();
		}
	}
	
	public override void _Ready()
	{
		credits = GetNode<Credits>(NodePaths.Credits);
		mainRow = GetChild<HBoxContainer>(0);
		
		GetNode<Button>(NodePaths.CreditsButton).Pressed += pressedCredits;
		GetNode<Button>(NodePaths.NewGame).Pressed += pressedNewGame;
		GetNode<Button>(NodePaths.Quit).Pressed += pressedQuit;
	}
	
	private void pressedCredits()
	{
		mainRow.Hide();
		credits.Show();
	}
	
	private void pressedNewGame() => EmitSignal(SignalName.StartNewGame);
	private void pressedQuit() => GetTree().Quit();
}
