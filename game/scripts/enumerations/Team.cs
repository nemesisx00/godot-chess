namespace Chess;

public enum Team
{
	Black,
	White,
}

public static class TeamExtensions
{
	public static Team NextTeam(this Team self) => (Team)(((int)self + 1) % 2);
}
