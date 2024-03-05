using Chess.Autoload;
using Godot;

namespace Chess.Nodes;

public partial class GameOver : Control
{
	private static class NodePaths
	{
		public static readonly NodePath Status = new("%Status");
		public static readonly NodePath NewGame = new("%NewGame");
	}
	
	private static class StatusTexts
	{
		public const string Victory = "You win!";
		public const string Loss = "You lose!";
		public const string Stalemate = "Stalemate!";
	}
	
	[Signal]
	public delegate void StartNewGameEventHandler();
	
	private Label statusLabel;
	
	public override void _Ready()
	{
		statusLabel = GetNode<Label>(NodePaths.Status);
		
		GetNode<Button>(NodePaths.NewGame).Pressed += handleNewGame;
	}
	
	public new void Show()
	{
		statusLabel.Text = GetNode<GameState>(GameState.NodePath).Status switch
		{
			GameStatus.Loss => StatusTexts.Loss,
			GameStatus.Stalemate => StatusTexts.Stalemate,
			GameStatus.Victory => StatusTexts.Victory,
			_ => string.Empty,
		};
		
		base.Show();
	}
	
	private void handleNewGame() => EmitSignal(SignalName.StartNewGame);
}
