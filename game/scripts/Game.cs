using Godot;

namespace Chess;

public partial class Game : Node3D
{
	private static class NodePaths
	{
		public static readonly NodePath Board = new("Chessboard");
		public static readonly NodePath CameraMount = new("CameraMount");
	}
	
	[Export(PropertyHint.Range, "0.1,2,0.05")]
	private float cameraRotationSpeed = 0.25f;
	
	private Chessboard board;
	private Node3D cameraMount;
	
	private ChessPiece selectedPiece = null;
	
	public override void _Process(double delta)
	{
		if(Input.IsActionJustPressed(Actions.DeselectPiece))
		{
			selectedPiece = null;
			board.EnablePieceSelection();
		}
	}
	
	public override void _Ready()
	{
		board = GetNode<Chessboard>(NodePaths.Board);
		cameraMount = GetNode<Node3D>(NodePaths.CameraMount);
		
		board.CellClicked += handleCellClicked;
		
		generatePieces();
	}
	
	private void handleCellClicked(BoardCell cell)
	{
		if(selectedPiece is not null && PieceMovementLogic.IsDestinationValid(selectedPiece, cell))
		{
			Chessboard.MovePiece(selectedPiece, cell);
			selectedPiece = null;
			board.EnablePieceSelection();
		}
	}
	
	private void handlePieceClicked(ChessPiece piece)
	{
		selectedPiece = piece;
		board.EnableCellSelection();
	}
	
	private void generatePieces()
	{
		ChessPiece piece;
		
		var materialWhite = GD.Load<StandardMaterial3D>(ChessMaterials.White);
		
		var packedScene = GD.Load<PackedScene>($"{ResourcePaths.Nodes}/Bishop.tscn");
		if(packedScene.CanInstantiate())
		{
			piece = packedScene.Instantiate<ChessPiece>();
			piece.OverrideMaterial = materialWhite;
			piece.Team = Teams.White;
			board.AddPiece(piece);
			board.MovePiece(File.C, Rank.One, piece, true);
			piece.RotateObjectLocal(Vector3.Up, Mathf.Tau / 2);
			piece.Clicked += handlePieceClicked;
			
			piece = packedScene.Instantiate<ChessPiece>();
			piece.OverrideMaterial = materialWhite;
			piece.Team = Teams.White;
			board.AddPiece(piece);
			board.MovePiece(File.F, Rank.One, piece, true);
			piece.RotateObjectLocal(Vector3.Up, Mathf.Tau / 2);
			piece.Clicked += handlePieceClicked;
			
			piece = packedScene.Instantiate<ChessPiece>();
			board.AddPiece(piece);
			board.MovePiece(File.C, Rank.Eight, piece, true);
			piece.Clicked += handlePieceClicked;
			
			piece = packedScene.Instantiate<ChessPiece>();
			board.AddPiece(piece);
			board.MovePiece(File.F, Rank.Eight, piece, true);
			piece.Clicked += handlePieceClicked;
		}
		
		packedScene = GD.Load<PackedScene>($"{ResourcePaths.Nodes}/King.tscn");
		if(packedScene.CanInstantiate())
		{
			piece = packedScene.Instantiate<ChessPiece>();
			piece.OverrideMaterial = materialWhite;
			piece.Team = Teams.White;
			board.AddPiece(piece);
			board.MovePiece(File.E, Rank.One, piece, true);
			piece.Clicked += handlePieceClicked;
			
			piece = packedScene.Instantiate<ChessPiece>();
			board.AddPiece(piece);
			board.MovePiece(File.E, Rank.Eight, piece, true);
			piece.Clicked += handlePieceClicked;
		}
		
		packedScene = GD.Load<PackedScene>($"{ResourcePaths.Nodes}/Knight.tscn");
		if(packedScene.CanInstantiate())
		{
			piece = packedScene.Instantiate<ChessPiece>();
			piece.OverrideMaterial = materialWhite;
			piece.Team = Teams.White;
			board.AddPiece(piece);
			board.MovePiece(File.B, Rank.One, piece, true);
			piece.RotateObjectLocal(Vector3.Up, Mathf.Tau / 2);
			piece.Clicked += handlePieceClicked;
			
			piece = packedScene.Instantiate<ChessPiece>();
			piece.OverrideMaterial = materialWhite;
			piece.Team = Teams.White;
			board.AddPiece(piece);
			board.MovePiece(File.G, Rank.One, piece, true);
			piece.RotateObjectLocal(Vector3.Up, Mathf.Tau / 2);
			piece.Clicked += handlePieceClicked;
			
			piece = packedScene.Instantiate<ChessPiece>();
			board.AddPiece(piece);
			board.MovePiece(File.B, Rank.Eight, piece, true);
			piece.Clicked += handlePieceClicked;
			
			piece = packedScene.Instantiate<ChessPiece>();
			board.AddPiece(piece);
			board.MovePiece(File.G, Rank.Eight, piece, true);
			piece.Clicked += handlePieceClicked;
		}
		
		packedScene = GD.Load<PackedScene>($"{ResourcePaths.Nodes}/Pawn.tscn");
		if(packedScene.CanInstantiate())
		{
			piece = packedScene.Instantiate<ChessPiece>();
			piece.OverrideMaterial = materialWhite;
			piece.Team = Teams.White;
			board.AddPiece(piece);
			board.MovePiece(File.A, Rank.Two, piece, true);
			piece.Clicked += handlePieceClicked;
			
			piece = packedScene.Instantiate<ChessPiece>();
			piece.OverrideMaterial = materialWhite;
			piece.Team = Teams.White;
			board.AddPiece(piece);
			board.MovePiece(File.B, Rank.Two, piece, true);
			piece.Clicked += handlePieceClicked;
			
			piece = packedScene.Instantiate<ChessPiece>();
			piece.OverrideMaterial = materialWhite;
			piece.Team = Teams.White;
			board.AddPiece(piece);
			board.MovePiece(File.C, Rank.Two, piece, true);
			piece.Clicked += handlePieceClicked;
			
			piece = packedScene.Instantiate<ChessPiece>();
			piece.OverrideMaterial = materialWhite;
			piece.Team = Teams.White;
			board.AddPiece(piece);
			board.MovePiece(File.D, Rank.Two, piece, true);
			piece.Clicked += handlePieceClicked;
			
			piece = packedScene.Instantiate<ChessPiece>();
			piece.OverrideMaterial = materialWhite;
			piece.Team = Teams.White;
			board.AddPiece(piece);
			board.MovePiece(File.E, Rank.Two, piece, true);
			piece.Clicked += handlePieceClicked;
			
			piece = packedScene.Instantiate<ChessPiece>();
			piece.OverrideMaterial = materialWhite;
			piece.Team = Teams.White;
			board.AddPiece(piece);
			board.MovePiece(File.F, Rank.Two, piece, true);
			piece.Clicked += handlePieceClicked;
			
			piece = packedScene.Instantiate<ChessPiece>();
			piece.OverrideMaterial = materialWhite;
			piece.Team = Teams.White;
			board.AddPiece(piece);
			board.MovePiece(File.G, Rank.Two, piece, true);
			piece.Clicked += handlePieceClicked;
			
			piece = packedScene.Instantiate<ChessPiece>();
			piece.OverrideMaterial = materialWhite;
			piece.Team = Teams.White;
			board.AddPiece(piece);
			board.MovePiece(File.H, Rank.Two, piece, true);
			piece.Clicked += handlePieceClicked;
			
			piece = packedScene.Instantiate<ChessPiece>();
			board.AddPiece(piece);
			board.MovePiece(File.A, Rank.Seven, piece, true);
			piece.Clicked += handlePieceClicked;
			
			piece = packedScene.Instantiate<ChessPiece>();
			board.AddPiece(piece);
			board.MovePiece(File.B, Rank.Seven, piece, true);
			piece.Clicked += handlePieceClicked;
			
			piece = packedScene.Instantiate<ChessPiece>();
			board.AddPiece(piece);
			board.MovePiece(File.C, Rank.Seven, piece, true);
			piece.Clicked += handlePieceClicked;
			
			piece = packedScene.Instantiate<ChessPiece>();
			board.AddPiece(piece);
			board.MovePiece(File.D, Rank.Seven, piece, true);
			piece.Clicked += handlePieceClicked;
			
			piece = packedScene.Instantiate<ChessPiece>();
			board.AddPiece(piece);
			board.MovePiece(File.E, Rank.Seven, piece, true);
			piece.Clicked += handlePieceClicked;
			
			piece = packedScene.Instantiate<ChessPiece>();
			board.AddPiece(piece);
			board.MovePiece(File.F, Rank.Seven, piece, true);
			piece.Clicked += handlePieceClicked;
			
			piece = packedScene.Instantiate<ChessPiece>();
			board.AddPiece(piece);
			board.MovePiece(File.G, Rank.Seven, piece, true);
			piece.Clicked += handlePieceClicked;
			
			piece = packedScene.Instantiate<ChessPiece>();
			board.AddPiece(piece);
			board.MovePiece(File.H, Rank.Seven, piece, true);
			piece.Clicked += handlePieceClicked;
		}
		
		packedScene = GD.Load<PackedScene>($"{ResourcePaths.Nodes}/Queen.tscn");
		if(packedScene.CanInstantiate())
		{
			piece = packedScene.Instantiate<ChessPiece>();
			piece.OverrideMaterial = materialWhite;
			piece.Team = Teams.White;
			board.AddPiece(piece);
			board.MovePiece(File.D, Rank.One, piece, true);
			piece.Clicked += handlePieceClicked;
			
			piece = packedScene.Instantiate<ChessPiece>();
			board.AddPiece(piece);
			board.MovePiece(File.D, Rank.Eight, piece, true);
			piece.Clicked += handlePieceClicked;
		}
		
		packedScene = GD.Load<PackedScene>($"{ResourcePaths.Nodes}/Rook.tscn");
		if(packedScene.CanInstantiate())
		{
			piece = packedScene.Instantiate<ChessPiece>();
			piece.OverrideMaterial = materialWhite;
			piece.Team = Teams.White;
			board.AddPiece(piece);
			board.MovePiece(File.A, Rank.One, piece, true);
			piece.Clicked += handlePieceClicked;
			
			piece = packedScene.Instantiate<ChessPiece>();
			piece.OverrideMaterial = materialWhite;
			piece.Team = Teams.White;
			board.AddPiece(piece);
			board.MovePiece(File.H, Rank.One, piece, true);
			piece.Clicked += handlePieceClicked;
			
			piece = packedScene.Instantiate<ChessPiece>();
			board.AddPiece(piece);
			board.MovePiece(File.A, Rank.Eight, piece, true);
			piece.Clicked += handlePieceClicked;
			
			piece = packedScene.Instantiate<ChessPiece>();
			board.AddPiece(piece);
			board.MovePiece(File.H, Rank.Eight, piece, true);
			piece.Clicked += handlePieceClicked;
		}
	}
}
