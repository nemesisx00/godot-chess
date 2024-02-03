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
	
	private TexturedModel board;
	private Node3D cameraMount;
	
	public override void _Ready()
	{
		board = GetNode<TexturedModel>(NodePaths.Board);
		cameraMount = GetNode<Node3D>(NodePaths.CameraMount);
		
		generatePieces();
	}

	public override void _PhysicsProcess(double delta)
	{
		cameraMount.RotateObjectLocal(Vector3.Up, (float)delta * cameraRotationSpeed);
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
			board.AddChild(piece);
			BoardPosition.MoveToCoordinate(File.C, Rank.One, ref piece);
			
			piece = packedScene.Instantiate<ChessPiece>();
			piece.OverrideMaterial = materialWhite;
			piece.Team = Teams.White;
			board.AddChild(piece);
			BoardPosition.MoveToCoordinate(File.F, Rank.One, ref piece);
			
			piece = packedScene.Instantiate<ChessPiece>();
			board.AddChild(piece);
			BoardPosition.MoveToCoordinate(File.C, Rank.Eight, ref piece);
			
			piece = packedScene.Instantiate<ChessPiece>();
			board.AddChild(piece);
			BoardPosition.MoveToCoordinate(File.F, Rank.Eight, ref piece);
		}
		
		packedScene = GD.Load<PackedScene>($"{ResourcePaths.Nodes}/King.tscn");
		if(packedScene.CanInstantiate())
		{
			piece = packedScene.Instantiate<ChessPiece>();
			piece.OverrideMaterial = materialWhite;
			piece.Team = Teams.White;
			board.AddChild(piece);
			BoardPosition.MoveToCoordinate(File.E, Rank.One, ref piece);
			
			piece = packedScene.Instantiate<ChessPiece>();
			board.AddChild(piece);
			BoardPosition.MoveToCoordinate(File.E, Rank.Eight, ref piece);
		}
		
		packedScene = GD.Load<PackedScene>($"{ResourcePaths.Nodes}/Knight.tscn");
		if(packedScene.CanInstantiate())
		{
			piece = packedScene.Instantiate<ChessPiece>();
			piece.OverrideMaterial = materialWhite;
			piece.Team = Teams.White;
			board.AddChild(piece);
			BoardPosition.MoveToCoordinate(File.B, Rank.One, ref piece);
			
			piece = packedScene.Instantiate<ChessPiece>();
			piece.OverrideMaterial = materialWhite;
			piece.Team = Teams.White;
			board.AddChild(piece);
			BoardPosition.MoveToCoordinate(File.G, Rank.One, ref piece);
			
			piece = packedScene.Instantiate<ChessPiece>();
			board.AddChild(piece);
			BoardPosition.MoveToCoordinate(File.B, Rank.Eight, ref piece);
			
			piece = packedScene.Instantiate<ChessPiece>();
			board.AddChild(piece);
			BoardPosition.MoveToCoordinate(File.G, Rank.Eight, ref piece);
		}
		
		packedScene = GD.Load<PackedScene>($"{ResourcePaths.Nodes}/Pawn.tscn");
		if(packedScene.CanInstantiate())
		{
			piece = packedScene.Instantiate<ChessPiece>();
			piece.OverrideMaterial = materialWhite;
			piece.Team = Teams.White;
			board.AddChild(piece);
			BoardPosition.MoveToCoordinate(File.A, Rank.Two, ref piece);
			
			piece = packedScene.Instantiate<ChessPiece>();
			piece.OverrideMaterial = materialWhite;
			piece.Team = Teams.White;
			board.AddChild(piece);
			BoardPosition.MoveToCoordinate(File.B, Rank.Two, ref piece);
			
			piece = packedScene.Instantiate<ChessPiece>();
			piece.OverrideMaterial = materialWhite;
			piece.Team = Teams.White;
			board.AddChild(piece);
			BoardPosition.MoveToCoordinate(File.C, Rank.Two, ref piece);
			
			piece = packedScene.Instantiate<ChessPiece>();
			piece.OverrideMaterial = materialWhite;
			piece.Team = Teams.White;
			board.AddChild(piece);
			BoardPosition.MoveToCoordinate(File.D, Rank.Two, ref piece);
			
			piece = packedScene.Instantiate<ChessPiece>();
			piece.OverrideMaterial = materialWhite;
			piece.Team = Teams.White;
			board.AddChild(piece);
			BoardPosition.MoveToCoordinate(File.E, Rank.Two, ref piece);
			
			piece = packedScene.Instantiate<ChessPiece>();
			piece.OverrideMaterial = materialWhite;
			piece.Team = Teams.White;
			board.AddChild(piece);
			BoardPosition.MoveToCoordinate(File.F, Rank.Two, ref piece);
			
			piece = packedScene.Instantiate<ChessPiece>();
			piece.OverrideMaterial = materialWhite;
			piece.Team = Teams.White;
			board.AddChild(piece);
			BoardPosition.MoveToCoordinate(File.G, Rank.Two, ref piece);
			
			piece = packedScene.Instantiate<ChessPiece>();
			piece.OverrideMaterial = materialWhite;
			piece.Team = Teams.White;
			board.AddChild(piece);
			BoardPosition.MoveToCoordinate(File.H, Rank.Two, ref piece);
			
			piece = packedScene.Instantiate<ChessPiece>();
			board.AddChild(piece);
			BoardPosition.MoveToCoordinate(File.A, Rank.Seven, ref piece);
			
			piece = packedScene.Instantiate<ChessPiece>();
			board.AddChild(piece);
			BoardPosition.MoveToCoordinate(File.B, Rank.Seven, ref piece);
			
			piece = packedScene.Instantiate<ChessPiece>();
			board.AddChild(piece);
			BoardPosition.MoveToCoordinate(File.C, Rank.Seven, ref piece);
			
			piece = packedScene.Instantiate<ChessPiece>();
			board.AddChild(piece);
			BoardPosition.MoveToCoordinate(File.D, Rank.Seven, ref piece);
			
			piece = packedScene.Instantiate<ChessPiece>();
			board.AddChild(piece);
			BoardPosition.MoveToCoordinate(File.E, Rank.Seven, ref piece);
			
			piece = packedScene.Instantiate<ChessPiece>();
			board.AddChild(piece);
			BoardPosition.MoveToCoordinate(File.F, Rank.Seven, ref piece);
			
			piece = packedScene.Instantiate<ChessPiece>();
			board.AddChild(piece);
			BoardPosition.MoveToCoordinate(File.G, Rank.Seven, ref piece);
			
			piece = packedScene.Instantiate<ChessPiece>();
			board.AddChild(piece);
			BoardPosition.MoveToCoordinate(File.H, Rank.Seven, ref piece);
		}
		
		packedScene = GD.Load<PackedScene>($"{ResourcePaths.Nodes}/Queen.tscn");
		if(packedScene.CanInstantiate())
		{
			piece = packedScene.Instantiate<ChessPiece>();
			piece.OverrideMaterial = materialWhite;
			piece.Team = Teams.White;
			board.AddChild(piece);
			BoardPosition.MoveToCoordinate(File.D, Rank.One, ref piece);
			
			piece = packedScene.Instantiate<ChessPiece>();
			board.AddChild(piece);
			BoardPosition.MoveToCoordinate(File.D, Rank.Eight, ref piece);
		}
		
		packedScene = GD.Load<PackedScene>($"{ResourcePaths.Nodes}/Rook.tscn");
		if(packedScene.CanInstantiate())
		{
			piece = packedScene.Instantiate<ChessPiece>();
			piece.OverrideMaterial = materialWhite;
			piece.Team = Teams.White;
			board.AddChild(piece);
			BoardPosition.MoveToCoordinate(File.A, Rank.One, ref piece);
			
			piece = packedScene.Instantiate<ChessPiece>();
			piece.OverrideMaterial = materialWhite;
			piece.Team = Teams.White;
			board.AddChild(piece);
			BoardPosition.MoveToCoordinate(File.H, Rank.One, ref piece);
			
			piece = packedScene.Instantiate<ChessPiece>();
			board.AddChild(piece);
			BoardPosition.MoveToCoordinate(File.A, Rank.Eight, ref piece);
			
			piece = packedScene.Instantiate<ChessPiece>();
			board.AddChild(piece);
			BoardPosition.MoveToCoordinate(File.H, Rank.Eight, ref piece);
		}
	}
}
