using Godot;

namespace Chess.Nodes;

public partial class ControlOptions : Container
{
	private static class NodePaths
	{
		public static readonly NodePath ControlsList = new("%ControlsList");
		public static readonly NodePath PopupPanel = new("%Popup");
	}
	
	private GridContainer controlsList;
	private PanelContainer popup;
	
	//Custom signals need to be manually detached, at least for now.
	public override void _ExitTree()
	{
		configureSignals(false);
		
		base._ExitTree();
	}
	
	public override void _Ready()
	{
		controlsList = GetNode<GridContainer>(NodePaths.ControlsList);
		popup = GetNode<PanelContainer>(NodePaths.PopupPanel);
		
		configureSignals(true);
	}
	
	private void displayPopup(ActionMapper node, bool listening)
	{
		if(listening)
			popup.Show();
		else
			popup.Hide();
	}
	
	private void configureSignals(bool attach)
	{
		foreach(var child in controlsList.GetChildren())
		{
			if(child is ActionMapper am)
			{
				if(attach)
					am.ListeningForInput += displayPopup;
				else
					am.ListeningForInput -= displayPopup;
			}
		}
	}
}
