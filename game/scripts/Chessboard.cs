using System.Linq;
using Godot;

namespace Chess;

public partial class Chessboard : Node3D
{
	[Signal]
	public delegate void CellClickedEventHandler(BoardCell cell);
	
	[Export]
	public Material OverrideMaterial { get; set; }
	
	public bool CellSelection { get; set; }
	
	private Node3D piece;
	
	public static void MovePiece(ChessPiece piece, BoardCell cell, bool teleport = false)
	{
		if(teleport)
			Utility.SetGlobalOrigin(piece, cell.GlobalTransform.Origin);
		else
			piece.Destination = cell.GlobalTransform.Origin;
		
		piece.File = cell.File;
		piece.Rank = cell.Rank;
	}
	
	public override void _Ready()
	{
		ReloadTextures();
		
		var children = GetChildren();
		foreach(var child in children)
		{
			if(child is Node3D file)
			{
				foreach(var cell in file.GetChildren().Cast<BoardCell>())
				{
					cell.Clicked += handleCellClicked;
				}
			}
		}
	}
	
	private void handleCellClicked(BoardCell cell)
	{
		if(CellSelection)
			EmitSignal(SignalName.CellClicked, cell);
	}
	
	public void ReloadTextures()
	{
		if(OverrideMaterial is not null)
		{
			var mesh = GetChild<MeshInstance3D>(0);
			mesh.MaterialOverride = OverrideMaterial;
		}
	}
	
	public void MovePiece(File file, Rank rank, ChessPiece piece, bool teleport = false)
	{
		var cell = GetNode<Node3D>(file.ToString())
			.GetNode<BoardCell>($"{(int)rank + 1}");
		
		MovePiece(piece, cell, teleport);
	}
}
