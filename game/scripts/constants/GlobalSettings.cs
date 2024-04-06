using Godot;

namespace Chess;

public static class GlobalSettings
{
	public static readonly float Gravity = (float)ProjectSettings.GetSetting("physics/3d/default_gravity");
}
