using Godot;
using System.Linq;

namespace Chess.Nodes;

public partial class OptionsMenu : Container
{
	private static class NodePaths
	{
		public static readonly NodePath AudioOptions = new("%AudioOptions");
		public static readonly NodePath AudioTab = new("%AudioTab");
		public static readonly NodePath BackToMainMenu = new("%BackToMainMenu");
		public static readonly NodePath ControlOptions = new("%ControlOptions");
		public static readonly NodePath ControlTab = new("%ControlsTab");
		public static readonly NodePath MenuContainer = new("%MenuContainer");
	}
	
	private enum MenuId
	{
		Audio,
		Control,
	}
	
	[Signal]
	public delegate void RequestHideEventHandler();
	
	private Container audioNode;
	private Button audioTab;
	private Button back;
	private Container controlNode;
	private Button controlTab;
	private Container menuContainer;
	
	public override void _Ready()
	{
		audioNode = GetNode<Container>(NodePaths.AudioOptions);
		audioTab = GetNode<Button>(NodePaths.AudioTab);
		back = GetNode<Button>(NodePaths.BackToMainMenu);
		controlNode = GetNode<Container>(NodePaths.ControlOptions);
		controlTab = GetNode<Button>(NodePaths.ControlTab);
		menuContainer = GetNode<Container>(NodePaths.MenuContainer);
		
		audioTab.Pressed += () => showMenu(MenuId.Audio);
		back.Pressed += () => EmitSignal(SignalName.RequestHide);
		controlTab.Pressed += () => showMenu(MenuId.Control);
	}
	
	public void ResetActiveTab() => showMenu();
	
	private void showMenu(MenuId id = 0)
	{
		foreach(var child in menuContainer.GetChildren().Cast<Control>())
		{
			child.Hide();
		}
		
		switch(id)
		{
			case MenuId.Audio:
				audioNode.Show();
				break;
			
			case MenuId.Control:
				controlNode.Show();
				break;
		}
	}
}
