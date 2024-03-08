using Godot;
using Chess.Nodes;

namespace Chess.Cpu;

public partial class CpuPlayer : Node
{
	public Teams Team { get; set; }
	
	private readonly Brain brain = new();
}
