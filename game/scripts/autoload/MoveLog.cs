using System.Collections.Generic;
using System.Linq;
using Godot;

namespace Chess.Autoload;

public partial class MoveLog : Node
{
	public static readonly NodePath NodePath = new("/root/MoveLog");
	
	[Signal]
	public delegate void MoveLoggedEventHandler();
	
	List<MoveLogEntry> Entries { get; set; } = [];
	
	public MoveLogEntry MostRecentEntry => Entries.LastOrDefault();
	
	public void AddEntry(BoardVector from, BoardVector to, Piece piece, Teams team)
		=> AddEntry(new(from, to, piece, team));
	
	public void AddEntry(MoveLogEntry entry)
	{
		Entries.Add(entry);
		EmitSignal(SignalName.MoveLogged);
	}
	
	public void Clear() => Entries.Clear();
}
