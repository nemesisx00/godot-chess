using System.Collections.Generic;
using Godot;

namespace Chess.Nodes;

public partial class ActionMapper : GridContainer
{
	[Signal]
	public delegate void ListeningForInputEventHandler(ActionMapper node, bool listening);
	
	[Export]
	private Action action;
	
	private readonly List<InputEvent> inputEvents = [];
	private InputEvent eventToRemove;
	private bool listening;
	
	public override void _Input(InputEvent evt)
	{
		if(listening && (evt is InputEventKey || evt is InputEventMouseButton))
		{
			replaceEvent(evt);
			refreshEvents();
			refreshUi();
			
			eventToRemove = null;
			listening = false;
			EmitSignal(SignalName.ListeningForInput, this, listening);
		}
	}
	
	public override void _Ready()
	{
		refreshEvents();
		refreshUi();
	}
	
	private void handlePress(Button node)
	{
		var actionName = Actions.From(action);
		//Node index must match the index of the corresponding input event
		var inputEvent = inputEvents[node.GetIndex()];
		
		//Sanity check
		if(InputMap.ActionHasEvent(actionName, inputEvent))
		{
			eventToRemove = inputEvent;
			listening = true;
			EmitSignal(SignalName.ListeningForInput, this, listening);
		}
	}
	
	private void refreshEvents()
	{
		var actionName = Actions.From(action);
		if(actionName is not null)
		{
			inputEvents.Clear();
			
			foreach(var inputEvent in InputMap.ActionGetEvents(actionName))
			{
				inputEvents.Add(inputEvent);
			}
		}
	}
	
	private void refreshUi()
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
	
	private void replaceEvent(InputEvent newEvent)
	{
		var actionName = Actions.From(action);
		InputMap.ActionEraseEvent(actionName, eventToRemove);
		InputMap.ActionAddEvent(actionName, newEvent);
	}
}
