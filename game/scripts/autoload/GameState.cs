using Godot;

namespace Chess.Autoload;

public partial class GameState : Node
{
	[Signal]
	public delegate void StartNextTurnEventHandler(Teams activePlayer);
	
	public static readonly NodePath NodePath = new("/root/GameState");
	
	public Teams CurrentPlayer { get; set; } = Teams.White;
	public Teams PlayerTeam { get; set; } = Teams.White;
	public GameStatus Status { get; set; }
	
	public void EndTurn()
	{
		CurrentPlayer = (Teams)(((int)CurrentPlayer + 1) % 2);
		EmitSignal(SignalName.StartNextTurn, (int)CurrentPlayer);
	}
}
