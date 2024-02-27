using Godot;
using Chess.Autoload;

namespace Chess.Nodes;

public partial class LogEntryLabel : HBoxContainer
{
	public int MoveNumber { get; set; }
	public MoveLogEntry White { get; set; }
	public MoveLogEntry Black { get; set; }
	
	private Label num;
	private TextureRect icon1;
	private Label white;
	private TextureRect icon2;
	private Label black;
	
	public override void _Ready()
	{
		num = new();
		icon1 = new TextureRect();
		white = new();
		icon2 = new TextureRect();
		black = new();
		
		icon1.ExpandMode = TextureRect.ExpandModeEnum.IgnoreSize;
		icon1.StretchMode = TextureRect.StretchModeEnum.KeepAspectCentered;
		icon1.CustomMinimumSize = new(64, 64);
		
		icon2.ExpandMode = TextureRect.ExpandModeEnum.IgnoreSize;
		icon2.StretchMode = TextureRect.StretchModeEnum.KeepAspectCentered;
		icon2.CustomMinimumSize = new(64, 64);
		
		AddChild(num);
		AddChild(icon1);
		AddChild(white);
		AddChild(icon2);
		AddChild(black);
	}
	
	public void Refresh()
	{
		num.Text = $"{MoveNumber}.";
		white.Text = $"{White}";
		black.Text = $"{Black}";
		
		if(White is not null)
			icon1.Texture = GD.Load<Texture2D>(ResourcePaths.PieceIcon(White.Team, White.Piece));
		
		if(Black is not null)
			icon2.Texture = GD.Load<Texture2D>(ResourcePaths.PieceIcon(Black.Team, Black.Piece));
	}
}
