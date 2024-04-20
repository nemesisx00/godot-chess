using System.Collections.Generic;
using System.Linq;
using Godot;

namespace Chess.Nodes;

public partial class ActionMapper : GridContainer
{
	[Signal]
	public delegate void ListeningForInputEventHandler(ActionMapper node, bool listening);
	
	[Export]
	private Action action;
	
	private static readonly Vector2 MinimumButtonSize = new(100, 40);
	
	private readonly List<InputEvent> inputEvents = [];
	private InputEvent eventToRemove;
	private bool listening;
	private StyleBox buttonStyle;
	
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
		buttonStyle = GD.Load<StyleBox>($"{ResourcePaths.Assets}/ButtonStyle.tres");
		
		refreshEvents();
		refreshUi();
	}
	
	private void createAddRemove()
	{
		HBoxContainer row = new()
		{
			SizeFlagsHorizontal = SizeFlags.Fill,
		};
		
		row.AddThemeConstantOverride(GodotPaths.OverrideSeparation, 15);
		
		Button add = new()
		{
			CustomMinimumSize = MinimumButtonSize,
			MouseDefaultCursorShape = CursorShape.PointingHand,
			SizeFlagsHorizontal = SizeFlags.ExpandFill,
			Text = "+",
		};
		
		Utility.ApplyButtonStyleOverrides(add, buttonStyle);
		
		Button remove = new()
		{
			CustomMinimumSize = MinimumButtonSize,
			MouseDefaultCursorShape = CursorShape.PointingHand,
			SizeFlagsHorizontal = SizeFlags.ExpandFill,
			Text = "-",
		};
		
		Utility.ApplyButtonStyleOverrides(remove, buttonStyle);
		
		row.AddChild(remove);
		row.AddChild(add);
		AddChild(row);
		
		add.Pressed += handleAddPrsesed;
		remove.Pressed += handleRemovePressed;
	}
	
	private void createButton(string text)
	{
		Button node = new()
		{
			CustomMinimumSize = MinimumButtonSize,
			MouseDefaultCursorShape = CursorShape.PointingHand,
			SizeFlagsHorizontal = SizeFlags.ExpandFill,
			Text = text,
		};
		
		Utility.ApplyButtonStyleOverrides(node, buttonStyle);
		
		AddChild(node);
		
		node.Pressed += () => handlePress(node);
	}
	
	private void handleAddPrsesed()
	{
		listening = true;
		EmitSignal(SignalName.ListeningForInput, this, listening);
	}
	
	private void handlePress(Button node)
	{
		var actionName = Actions.From(action);
		var index = node.GetIndex();
		
		if(index < inputEvents.Count)
		{
			var inputEvent = inputEvents[index];
			
			//Sanity check
			if(InputMap.ActionHasEvent(actionName, inputEvent))
				eventToRemove = inputEvent;
			
			listening = true;
			EmitSignal(SignalName.ListeningForInput, this, listening);
		}
	}
	
	private void handleRemovePressed()
	{
		var last = inputEvents.LastOrDefault();
		if(last is not null)
		{
			var actionName = Actions.From(action);
			InputMap.ActionEraseEvent(actionName, last);
			
			refreshEvents();
			refreshUi();
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
		
		foreach(var ie in inputEvents)
		{
			createButton(ie.AsText());
		}
		
		createAddRemove();
	}
	
	private void replaceEvent(InputEvent newEvent)
	{
		var actionName = Actions.From(action);
		
		if(eventToRemove is not null)
			InputMap.ActionEraseEvent(actionName, eventToRemove);
		
		InputMap.ActionAddEvent(actionName, newEvent);
	}
}
