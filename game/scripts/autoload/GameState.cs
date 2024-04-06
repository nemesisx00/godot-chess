using Godot;

namespace Chess.Autoload;

public partial class GameState : Node
{
	[Signal]
	public delegate void StartNextTurnEventHandler(Team activePlayer);
	
	public static readonly NodePath NodePath = new("/root/GameState");
	
	public Team CurrentPlayer { get; set; } = Team.White;
	public Team PlayerTeam { get; set; } = Team.White;
	public GameStatus Status { get; set; }
	
	public void EndTurn()
	{
		CurrentPlayer = (Team)(((int)CurrentPlayer + 1) % 2);
		EmitSignal(SignalName.StartNextTurn, (int)CurrentPlayer);
	}
}
