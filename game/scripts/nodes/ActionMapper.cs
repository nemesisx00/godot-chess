using System.Collections.Generic;
using System.Linq;
using Godot;

namespace Chess.Nodes;

public partial class ActionMapper : GridContainer
{
	[Export]
	private Action action;
	
	private readonly List<InputEvent> inputEvents = [];
	
	public override void _Ready()
	{
		var actionName = Actions.From(action);
		if(actionName is not null)
		{
			foreach(var inputEvent in InputMap.ActionGetEvents(actionName))
			{
				inputEvents.Add(inputEvent);
			}
		}
		
		refreshMappings();
	}
	
	private void handlePress(Button node)
	{
		var actionName = Actions.From(action);
		//Node index must match the index of the corresponding input event
		var inputEvent = inputEvents[node.GetIndex()];
		
		//Sanity check
		if(InputMap.ActionHasEvent(actionName, inputEvent))
		{
			GD.Print($"Clicked button for {actionName} currently mapped to {inputEvent.AsText()}");
			//Set up listening for a new input to replace the existing event
		}
	}
	
	private void refreshMappings()
	{
		foreach(var node in GetChildren())
		{
			node.QueueFree();
		}
		
		List<Button> mappingNodes = [];
		foreach(var ie in inputEvents)
		{
			Button node = new()
			{
				MouseDefaultCursorShape = CursorShape.PointingHand,
				Text = ie.AsText(),
			};
			
			AddChild(node);
			
			node.Pressed += () => handlePress(node);
		}
	}
}
