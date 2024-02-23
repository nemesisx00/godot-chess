using System.Linq;
using Godot;
using Chess.Autoload;
using Chess.Gameplay;

namespace Chess.Nodes;

public partial class Game : Node3D
{
	private static class NodePaths
	{
		public static readonly NodePath BlackGraveyard = new("BlackGraveyard");
		public static readonly NodePath Board = new("Chessboard");
		public static readonly NodePath CameraMount = new("CameraMount");
		public static readonly NodePath WhiteGraveyard = new("WhiteGraveyard");
	}
	
	[Export(PropertyHint.Range, "0.1,2,0.05")]
	private float cameraRotationSpeed = 0.25f;
	
	private Chessboard board;
	private Node3D cameraMount;
	
	private ChessPiece selectedPiece = null;
	
	private GameState gameState;
	private Graveyard blackGraveyard;
	private Graveyard whiteGraveyard;
	
	public override void _Process(double delta)
	{
		if(Input.IsActionJustPressed(Actions.DeselectPiece))
		{
			selectedPiece?.ToggleSelected(false);
			selectedPiece = null;
			board.Cells.Where(c => c.Indicator.Visible && !c.InCheck)
				.ToList()
				.ForEach(c => c.ToggleIndicator(false));
			board.EnablePieceSelection(gameState.CurrentPlayer);
		}
	}
	
	public override void _Ready()
	{
		blackGraveyard = GetNode<Graveyard>(NodePaths.BlackGraveyard);
		board = GetNode<Chessboard>(NodePaths.Board);
		cameraMount = GetNode<Node3D>(NodePaths.CameraMount);
		gameState = GetNode<GameState>(GameState.NodePath);
		whiteGraveyard = GetNode<Graveyard>(NodePaths.WhiteGraveyard);
		
		board.Capture += handleCapture;
		board.CellClicked += handleCellClicked;
		board.PieceHasMoved += handlePieceHasMoved;
		gameState.StartNextTurn += handleStartNextTurn;
		
		generatePieces();
		
		handleStartNextTurn(gameState.CurrentPlayer);
	}
	
	private void handleStartNextTurn(Teams activePlayer)
	{
		board.DisableAllPieceSelection();
		board.EnablePieceSelection(activePlayer);
	}
	
	private void handleCapture(ChessPiece attacker, ChessPiece defender)
	{
		if(defender.Team == Teams.Black)
			whiteGraveyard.BuryPiece(defender);
		else
			blackGraveyard.BuryPiece(defender);
	}
	
	private void handleCellClicked(BoardCell cell)
	{
		if(selectedPiece is not null && MoveLogic.IsDestinationValid(selectedPiece, cell, board))
			board.MovePiece(selectedPiece, cell);
	}
	
	private void handlePieceClicked(ChessPiece piece)
	{
		selectedPiece?.ToggleSelected(false);
		selectedPiece = piece;
		selectedPiece.ToggleSelected(true);
		board.EnableCellSelection(gameState.CurrentPlayer);
		
		MoveLogic.GetValidCells(piece, board)
			.ForEach(c => c.ToggleIndicator(true));
	}
	
	private void handlePieceHasMoved()
	{
		if(selectedPiece is not null)
		{
			selectedPiece.ToggleSelected(false);
			selectedPiece = null;
			board.Cells.Where(c => c.Indicator.Visible && !c.InCheck)
				.ToList()
				.ForEach(c => c.ToggleIndicator(false));
			gameState.EndTurn();
		}
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
			piece.Clicked += handlePieceClicked;
			
			piece = packedScene.Instantiate<ChessPiece>();
			piece.OverrideMaterial = materialWhite;
			piece.Team = Teams.White;
			board.AddPiece(piece);
			board.MovePiece(File.F, Rank.One, piece, true);
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
			piece.Clicked += handlePieceClicked;
			
			piece = packedScene.Instantiate<ChessPiece>();
			piece.OverrideMaterial = materialWhite;
			piece.Team = Teams.White;
			board.AddPiece(piece);
			board.MovePiece(File.G, Rank.One, piece, true);
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
