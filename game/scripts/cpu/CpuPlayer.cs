using Godot;

namespace Chess.Cpu;

public partial class CpuPlayer : Node
{
	public Team Team { get; set; }
	
	private readonly Brain brain = new();
}
