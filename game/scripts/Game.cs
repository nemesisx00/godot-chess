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
	
	public override void _Ready()
	{
		board = GetNode<Chessboard>(NodePaths.Board);
		cameraMount = GetNode<Node3D>(NodePaths.CameraMount);
		
		board.CellClicked += handleCellClicked;
		
		generatePieces();
	}
	
	public override void _PhysicsProcess(double delta)
	{
		//cameraMount.RotateObjectLocal(Vector3.Up, (float)delta * cameraRotationSpeed);
	}
	
	private void handleCellClicked(BoardCell cell)
	{
		if(selectedPiece is not null)
		{
			Chessboard.MovePiece(selectedPiece, cell);
			selectedPiece = null;
			board.CellSelection = false;
		}
	}
	
	private void handlePieceClicked(ChessPiece piece)
	{
		selectedPiece = piece;
		board.CellSelection = true;
	}
	
	private void generatePieces()
	{
		ChessPiece piece;
		
		var materialWhite = GD.Load<StandardMaterial3D>(ChessMaterials.White);
		
		var packedScene = GD.Load<PackedScene>($"{ResourcePaths.Nodes}/Bishop.tscn");
		if(packedScene.CanInstantiate())
		{
			piece = packedScene.Instantiate<ChessPiece>();
			piece.Name = "WhiteBishop1";
			piece.OverrideMaterial = materialWhite;
			piece.DiagonalMovement = true;
			piece.Team = Teams.White;
			board.AddChild(piece);
			board.MovePiece(File.C, Rank.One, piece, true);
			piece.RotateObjectLocal(Vector3.Up, Mathf.Tau / 2);
			piece.Clicked += handlePieceClicked;
			
			piece = packedScene.Instantiate<ChessPiece>();
			piece.Name = "WhiteBishop2";
			piece.OverrideMaterial = materialWhite;
			piece.DiagonalMovement = true;
			piece.Team = Teams.White;
			board.AddChild(piece);
			board.MovePiece(File.F, Rank.One, piece, true);
			piece.RotateObjectLocal(Vector3.Up, Mathf.Tau / 2);
			piece.Clicked += handlePieceClicked;
			
			piece = packedScene.Instantiate<ChessPiece>();
			piece.Name = "BlackBishop1";
			piece.DiagonalMovement = true;
			board.AddChild(piece);
			board.MovePiece(File.C, Rank.Eight, piece, true);
			piece.Clicked += handlePieceClicked;
			
			piece = packedScene.Instantiate<ChessPiece>();
			piece.Name = "BlackBishop2";
			piece.DiagonalMovement = true;
			board.AddChild(piece);
			board.MovePiece(File.F, Rank.Eight, piece, true);
			piece.Clicked += handlePieceClicked;
		}
		
		packedScene = GD.Load<PackedScene>($"{ResourcePaths.Nodes}/King.tscn");
		if(packedScene.CanInstantiate())
		{
			piece = packedScene.Instantiate<ChessPiece>();
			piece.Name = "WhiteKing";
			piece.OverrideMaterial = materialWhite;
			piece.DiagonalMovement = true;
			piece.Team = Teams.White;
			board.AddChild(piece);
			board.MovePiece(File.E, Rank.One, piece, true);
			piece.Clicked += handlePieceClicked;
			
			piece = packedScene.Instantiate<ChessPiece>();
			piece.Name = "BlackKing";
			piece.DiagonalMovement = true;
			board.AddChild(piece);
			board.MovePiece(File.E, Rank.Eight, piece, true);
			piece.Clicked += handlePieceClicked;
		}
		
		packedScene = GD.Load<PackedScene>($"{ResourcePaths.Nodes}/Knight.tscn");
		if(packedScene.CanInstantiate())
		{
			piece = packedScene.Instantiate<ChessPiece>();
			piece.Name = "WhiteKnight1";
			piece.OverrideMaterial = materialWhite;
			piece.Team = Teams.White;
			board.AddChild(piece);
			board.MovePiece(File.B, Rank.One, piece, true);
			piece.RotateObjectLocal(Vector3.Up, Mathf.Tau / 2);
			piece.Clicked += handlePieceClicked;
			
			piece = packedScene.Instantiate<ChessPiece>();
			piece.Name = "WhiteKnight2";
			piece.OverrideMaterial = materialWhite;
			piece.Team = Teams.White;
			board.AddChild(piece);
			board.MovePiece(File.G, Rank.One, piece, true);
			piece.RotateObjectLocal(Vector3.Up, Mathf.Tau / 2);
			piece.Clicked += handlePieceClicked;
			
			piece = packedScene.Instantiate<ChessPiece>();
			piece.Name = "BlackKnight1";
			board.AddChild(piece);
			board.MovePiece(File.B, Rank.Eight, piece, true);
			piece.Clicked += handlePieceClicked;
			
			piece = packedScene.Instantiate<ChessPiece>();
			piece.Name = "BlackKnight2";
			board.AddChild(piece);
			board.MovePiece(File.G, Rank.Eight, piece, true);
			piece.Clicked += handlePieceClicked;
		}
		
		packedScene = GD.Load<PackedScene>($"{ResourcePaths.Nodes}/Pawn.tscn");
		if(packedScene.CanInstantiate())
		{
			piece = packedScene.Instantiate<ChessPiece>();
			piece.Name = "WhitePawn1";
			piece.OverrideMaterial = materialWhite;
			piece.DiagonalMovement = true;
			piece.Team = Teams.White;
			board.AddChild(piece);
			board.MovePiece(File.A, Rank.Two, piece, true);
			piece.Clicked += handlePieceClicked;
			
			piece = packedScene.Instantiate<ChessPiece>();
			piece.Name = "WhitePawn2";
			piece.OverrideMaterial = materialWhite;
			piece.DiagonalMovement = true;
			piece.Team = Teams.White;
			board.AddChild(piece);
			board.MovePiece(File.B, Rank.Two, piece, true);
			piece.Clicked += handlePieceClicked;
			
			piece = packedScene.Instantiate<ChessPiece>();
			piece.Name = "WhitePawn3";
			piece.OverrideMaterial = materialWhite;
			piece.DiagonalMovement = true;
			piece.Team = Teams.White;
			board.AddChild(piece);
			board.MovePiece(File.C, Rank.Two, piece, true);
			piece.Clicked += handlePieceClicked;
			
			piece = packedScene.Instantiate<ChessPiece>();
			piece.Name = "WhitePawn4";
			piece.OverrideMaterial = materialWhite;
			piece.DiagonalMovement = true;
			piece.Team = Teams.White;
			board.AddChild(piece);
			board.MovePiece(File.D, Rank.Two, piece, true);
			piece.Clicked += handlePieceClicked;
			
			piece = packedScene.Instantiate<ChessPiece>();
			piece.Name = "WhitePawn5";
			piece.OverrideMaterial = materialWhite;
			piece.DiagonalMovement = true;
			piece.Team = Teams.White;
			board.AddChild(piece);
			board.MovePiece(File.E, Rank.Two, piece, true);
			piece.Clicked += handlePieceClicked;
			
			piece = packedScene.Instantiate<ChessPiece>();
			piece.Name = "WhitePawn6";
			piece.OverrideMaterial = materialWhite;
			piece.DiagonalMovement = true;
			piece.Team = Teams.White;
			board.AddChild(piece);
			board.MovePiece(File.F, Rank.Two, piece, true);
			piece.Clicked += handlePieceClicked;
			
			piece = packedScene.Instantiate<ChessPiece>();
			piece.Name = "WhitePawn7";
			piece.OverrideMaterial = materialWhite;
			piece.DiagonalMovement = true;
			piece.Team = Teams.White;
			board.AddChild(piece);
			board.MovePiece(File.G, Rank.Two, piece, true);
			piece.Clicked += handlePieceClicked;
			
			piece = packedScene.Instantiate<ChessPiece>();
			piece.Name = "WhitePawn8";
			piece.OverrideMaterial = materialWhite;
			piece.DiagonalMovement = true;
			piece.Team = Teams.White;
			board.AddChild(piece);
			board.MovePiece(File.H, Rank.Two, piece, true);
			piece.Clicked += handlePieceClicked;
			
			piece = packedScene.Instantiate<ChessPiece>();
			piece.Name = "BlackPawn1";
			piece.DiagonalMovement = true;
			board.AddChild(piece);
			board.MovePiece(File.A, Rank.Seven, piece, true);
			piece.Clicked += handlePieceClicked;
			
			piece = packedScene.Instantiate<ChessPiece>();
			piece.Name = "BlackPawn2";
			piece.DiagonalMovement = true;
			board.AddChild(piece);
			board.MovePiece(File.B, Rank.Seven, piece, true);
			piece.Clicked += handlePieceClicked;
			
			piece = packedScene.Instantiate<ChessPiece>();
			piece.Name = "BlackPawn3";
			piece.DiagonalMovement = true;
			board.AddChild(piece);
			board.MovePiece(File.C, Rank.Seven, piece, true);
			piece.Clicked += handlePieceClicked;
			
			piece = packedScene.Instantiate<ChessPiece>();
			piece.Name = "BlackPawn4";
			piece.DiagonalMovement = true;
			board.AddChild(piece);
			board.MovePiece(File.D, Rank.Seven, piece, true);
			piece.Clicked += handlePieceClicked;
			
			piece = packedScene.Instantiate<ChessPiece>();
			piece.Name = "BlackPawn5";
			piece.DiagonalMovement = true;
			board.AddChild(piece);
			board.MovePiece(File.E, Rank.Seven, piece, true);
			piece.Clicked += handlePieceClicked;
			
			piece = packedScene.Instantiate<ChessPiece>();
			piece.Name = "BlackPawn6";
			piece.DiagonalMovement = true;
			board.AddChild(piece);
			board.MovePiece(File.F, Rank.Seven, piece, true);
			piece.Clicked += handlePieceClicked;
			
			piece = packedScene.Instantiate<ChessPiece>();
			piece.Name = "BlackPawn7";
			piece.DiagonalMovement = true;
			board.AddChild(piece);
			board.MovePiece(File.G, Rank.Seven, piece, true);
			piece.Clicked += handlePieceClicked;
			
			piece = packedScene.Instantiate<ChessPiece>();
			piece.Name = "BlackPawn8";
			piece.DiagonalMovement = true;
			board.AddChild(piece);
			board.MovePiece(File.H, Rank.Seven, piece, true);
			piece.Clicked += handlePieceClicked;
		}
		
		packedScene = GD.Load<PackedScene>($"{ResourcePaths.Nodes}/Queen.tscn");
		if(packedScene.CanInstantiate())
		{
			piece = packedScene.Instantiate<ChessPiece>();
			piece.Name = "WhiteQueen";
			piece.OverrideMaterial = materialWhite;
			piece.DiagonalMovement = true;
			piece.Team = Teams.White;
			board.AddChild(piece);
			board.MovePiece(File.D, Rank.One, piece, true);
			piece.Clicked += handlePieceClicked;
			
			piece = packedScene.Instantiate<ChessPiece>();
			piece.Name = "BlackQueen";
			piece.DiagonalMovement = true;
			board.AddChild(piece);
			board.MovePiece(File.D, Rank.Eight, piece, true);
			piece.Clicked += handlePieceClicked;
		}
		
		packedScene = GD.Load<PackedScene>($"{ResourcePaths.Nodes}/Rook.tscn");
		if(packedScene.CanInstantiate())
		{
			piece = packedScene.Instantiate<ChessPiece>();
			piece.Name = "WhiteRook1";
			piece.OverrideMaterial = materialWhite;
			piece.Team = Teams.White;
			board.AddChild(piece);
			board.MovePiece(File.A, Rank.One, piece, true);
			piece.Clicked += handlePieceClicked;
			
			piece = packedScene.Instantiate<ChessPiece>();
			piece.Name = "WhiteRook2";
			piece.OverrideMaterial = materialWhite;
			piece.Team = Teams.White;
			board.AddChild(piece);
			board.MovePiece(File.H, Rank.One, piece, true);
			piece.Clicked += handlePieceClicked;
			
			piece = packedScene.Instantiate<ChessPiece>();
			piece.Name = "BlackRook1";
			board.AddChild(piece);
			board.MovePiece(File.A, Rank.Eight, piece, true);
			piece.Clicked += handlePieceClicked;
			
			piece = packedScene.Instantiate<ChessPiece>();
			piece.Name = "BlackRook2";
			board.AddChild(piece);
			board.MovePiece(File.H, Rank.Eight, piece, true);
			piece.Clicked += handlePieceClicked;
		}
	}
}
