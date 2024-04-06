using System.Linq;
using Chess.Autoload;
using Godot;

namespace Chess.Nodes;

public partial class MoveLogView : ScrollContainer
{
	private static class NodePaths
	{
		public static readonly NodePath Animation = new("AnimationPlayer");
		public static readonly NodePath Content = new("%Content");
		public static readonly NodePath ToggleUi = new("%ToggleUi");
	}
	
	private static class AnimationNames
	{
		public static readonly StringName Offscreen = new("offscreen");
		public static readonly StringName SlideIn = new("slideIn");
		public static readonly StringName SlideOut = new("slideOut");
	}
	
	public bool EnableLogUpdates { get; set; }
	
	private AnimationPlayer animation;
	private VBoxContainer content;
	private MoveLog moveLog;
	private bool open;
	private Button toggleUi;
	
	public override void _Ready()
	{
		animation = GetNode<AnimationPlayer>(NodePaths.Animation);
		content = GetNode<VBoxContainer>(NodePaths.Content);
		moveLog = GetNode<MoveLog>(MoveLog.NodePath);
		toggleUi = GetNode<Button>(NodePaths.ToggleUi);
		
		animation.AnimationFinished += animationFinished;
		moveLog.LogCleared += clearLog;
		moveLog.MoveLogged += renderLog;
		toggleUi.Pressed += pressToggleUi;
		
		animation.Play(AnimationNames.Offscreen);
	}
	
	private void clearLog() => content.GetChildren().Cast<Control>().ToList().ForEach(n => {
		n.Hide();
		n.QueueFree();
	});
	
	private void renderLog()
	{
		if(EnableLogUpdates)
		{
			clearLog();
			for(var i = 0; i < moveLog.Entries.Count; i++)
			{
				var entry = moveLog.Entries[i];
				if(entry.Team == Team.White)
				{
					var label = new LogEntryLabel
					{
						MoveNumber = (i / 2) + 1,
						White = entry
					};
					content.AddChild(label);
					label.Refresh();
				}
				else
				{
					var label = content.GetChildren()
						.Cast<LogEntryLabel>()
						.ToList()
						.Last();
					
					label.Black = entry;
					label.Refresh();
				}
			};
		}
	}
	
	private void pressToggleUi()
	{
		if(open)
			animation.Play(AnimationNames.SlideOut);
		else
			animation.Play(AnimationNames.SlideIn);
		
		open = !open;
	}
	
	private void animationFinished(StringName animationName)
	{
		if(AnimationNames.SlideIn.Equals(animationName))
			toggleUi.Text = ">";
		else if(AnimationNames.SlideOut.Equals(animationName) || AnimationNames.Offscreen.Equals(animationName))
			toggleUi.Text = "<";
	}
}
