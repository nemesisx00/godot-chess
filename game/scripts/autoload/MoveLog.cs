using System.Collections.Generic;
using System.Linq;
using Godot;

namespace Chess.Autoload;

public partial class MoveLog : Node
{
	public static readonly NodePath NodePath = new("/root/MoveLog");
	
	[Signal]
	public delegate void LogClearedEventHandler();
	
	[Signal]
	public delegate void MoveLoggedEventHandler();
	
	public int CurrentIndex { get; set; }
	public List<MoveLogEntry> Entries { get; set; } = [];
	public MoveLogEntry MostRecentEntry => Entries.LastOrDefault();
	
	public void AddEntry(BoardVector from, BoardVector to, Piece piece, Team team)
		=> AddEntry(new(from, to, piece, team));
	
	public void AddEntry(MoveLogEntry entry)
	{
		Entries.Add(entry);
		CurrentIndex = Entries.Count - 1;
		EmitSignal(SignalName.MoveLogged);
	}
	
	public void Clear()
	{
		Entries.Clear();
		CurrentIndex = 0;
		EmitSignal(SignalName.LogCleared);
	}
	
	public void ForceUpdate() => EmitSignal(SignalName.MoveLogged);
	
	public MoveLogEntry StepBack()
	{
		if(CurrentIndex >= 0)
			CurrentIndex--;
		return Entries[CurrentIndex];
	}
	
	public MoveLogEntry StepForward()
	{
		var count = Entries.Count - 1;
		if(CurrentIndex < count)
			CurrentIndex++;
		else
			CurrentIndex = count;
		
		return Entries[CurrentIndex];
	}
}
