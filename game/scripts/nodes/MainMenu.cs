using Godot;

namespace Chess.Nodes;

public partial class MainMenu : MarginContainer
{
	private static class NodePaths
	{
		public static readonly NodePath Credits = new("%Credits");
		public static readonly NodePath CreditsButton = new("%CreditsButton");
		public static readonly NodePath NewGame = new("%NewGame");
		public static readonly NodePath OptionsButton = new("%OptionsButton");
		public static readonly NodePath OptionsMenu = new("%OptionsMenu");
		public static readonly NodePath Quit = new("%Quit");
	}
	
	[Signal]
	public delegate void StartNewGameEventHandler();
	
	public bool CreditsVisible => credits.Visible;
	
	private HBoxContainer mainRow;
	private Credits credits;
	private OptionsMenu optionsMenu;
	
	public override void _ExitTree()
	{
		optionsMenu.RequestHide -= showMainMenu;
		
		base._ExitTree();
	}
	
	public override void _Ready()
	{
		credits = GetNode<Credits>(NodePaths.Credits);
		mainRow = GetChild<HBoxContainer>(0);
		optionsMenu = GetNode<OptionsMenu>(NodePaths.OptionsMenu);
		
		GetNode<Button>(NodePaths.CreditsButton).Pressed += pressedCredits;
		GetNode<Button>(NodePaths.NewGame).Pressed += pressedNewGame;
		GetNode<Button>(NodePaths.OptionsButton).Pressed += pressedOptions;
		GetNode<Button>(NodePaths.Quit).Pressed += pressedQuit;
		
		optionsMenu.RequestHide += showMainMenu;
	}
	
	public override void _UnhandledInput(InputEvent evt)
	{
		if(credits.Visible && evt is InputEventKey iek && iek.IsReleased())
			showMainMenu();
	}
	
	private void pressedCredits()
	{
		mainRow.Hide();
		credits.Show();
	}
	
	private void pressedNewGame() => EmitSignal(SignalName.StartNewGame);
	
	private void pressedOptions()
	{
		mainRow.Hide();
		optionsMenu.Show();
	}
	
	private void pressedQuit() => GetTree().Quit();
	
	private void showMainMenu()
	{
		credits.Hide();
		optionsMenu.Hide();
		optionsMenu.ResetActiveTab();
		mainRow.Show();
	}
}
