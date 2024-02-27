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
		public static readonly NodePath MainMenu = new("%MainMenu");
		public static readonly NodePath MoveLogView = new("%MoveLogView");
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
	
	private MainMenu mainMenu;
	private MoveLog moveLog;
	private MoveLogView moveLogView;
	private int piecesReset;
	
	public override void _UnhandledInput(InputEvent evt)
	{
		if(!mainMenu.Visible && evt.IsActionPressed(Actions.DeselectPiece, false, true))
		{
			deselectSelectedPiece();
			board.EnablePieceSelection(gameState.CurrentPlayer);
		}
		else if(!mainMenu.CreditsVisible && evt.IsActionPressed(Actions.ToggleMenu, false, true))
			toggleMainMenu();
	}
	
	public override void _Ready()
	{
		blackGraveyard = GetNode<Graveyard>(NodePaths.BlackGraveyard);
		board = GetNode<Chessboard>(NodePaths.Board);
		cameraMount = GetNode<Node3D>(NodePaths.CameraMount);
		gameState = GetNode<GameState>(GameState.NodePath);
		mainMenu = GetNode<MainMenu>(NodePaths.MainMenu);
		moveLog = GetNode<MoveLog>(MoveLog.NodePath);
		moveLogView = GetNode<MoveLogView>(NodePaths.MoveLogView);
		whiteGraveyard = GetNode<Graveyard>(NodePaths.WhiteGraveyard);
		
		board.Capture += handleCapture;
		board.CellClicked += handleCellClicked;
		board.PieceHasMoved += handlePieceHasMoved;
		gameState.StartNextTurn += handleStartNextTurn;
		mainMenu.StartNewGame += handleStartNewGame;
		
		generatePieces();
		
		handleStartNextTurn(gameState.CurrentPlayer);
		toggleMainMenu();
	}
	
	private void deselectSelectedPiece()
	{
		selectedPiece?.ToggleSelected(false);
		selectedPiece = null;
		board.Cells.Where(c => c.Indicator.Visible && !c.InCheck)
			.ToList()
			.ForEach(c => c.ToggleIndicator(false));
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
		if(selectedPiece is not null && MoveLogic.IsDestinationValid(selectedPiece, cell, board, moveLog))
			board.MovePiece(selectedPiece, cell);
	}
	
	private void handlePieceClicked(ChessPiece piece)
	{
		deselectSelectedPiece();
		selectedPiece = piece;
		selectedPiece.ToggleSelected(true);
		board.EnableCellSelection(gameState.CurrentPlayer);
		
		MoveLogic.GetValidCells(piece, board, moveLog)
			.ForEach(c => c.ToggleIndicator(true));
	}
	
	private void handlePieceHasMoved()
	{
		if(gameState.Status == GameStatus.Reseting)
		{
			if(moveLogView.EnableLogUpdates)
				moveLogView.EnableLogUpdates = false;
			
			piecesReset++;
			if(piecesReset >= 32)
			{
				moveLog.Clear();
				board.Pieces.ForEach(p => p.HasMoved = false);
				moveLogView.EnableLogUpdates = true;
				gameState.Status = GameStatus.Playing;
				piecesReset = 0;
			}
		}
		else
		{
			deselectSelectedPiece();
			gameState.EndTurn();
		}
	}
	
	private void handleStartNewGame()
	{
		toggleMainMenu();
		gameState.CurrentPlayer = Teams.White;
		gameState.Status = GameStatus.Reseting;
		board.ResetPieces();
	}
	
	private void handleStartNextTurn(Teams activePlayer)
	{
		board.DisableAllPieceSelection();
		board.EnablePieceSelection(activePlayer);
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
			piece.PieceNumber = 2;
			board.AddPiece(piece);
			board.MovePiece(File.F, Rank.One, piece, true);
			piece.Clicked += handlePieceClicked;
			
			piece = packedScene.Instantiate<ChessPiece>();
			board.AddPiece(piece);
			board.MovePiece(File.C, Rank.Eight, piece, true);
			piece.Clicked += handlePieceClicked;
			
			piece = packedScene.Instantiate<ChessPiece>();
			piece.PieceNumber = 2;
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
			piece.PieceNumber = 2;
			board.AddPiece(piece);
			board.MovePiece(File.G, Rank.One, piece, true);
			piece.Clicked += handlePieceClicked;
			
			piece = packedScene.Instantiate<ChessPiece>();
			board.AddPiece(piece);
			board.MovePiece(File.B, Rank.Eight, piece, true);
			piece.Clicked += handlePieceClicked;
			
			piece = packedScene.Instantiate<ChessPiece>();
			piece.PieceNumber = 2;
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
			piece.PieceNumber = 2;
			board.AddPiece(piece);
			board.MovePiece(File.B, Rank.Two, piece, true);
			piece.Clicked += handlePieceClicked;
			
			piece = packedScene.Instantiate<ChessPiece>();
			piece.OverrideMaterial = materialWhite;
			piece.Team = Teams.White;
			piece.PieceNumber = 3;
			board.AddPiece(piece);
			board.MovePiece(File.C, Rank.Two, piece, true);
			piece.Clicked += handlePieceClicked;
			
			piece = packedScene.Instantiate<ChessPiece>();
			piece.OverrideMaterial = materialWhite;
			piece.Team = Teams.White;
			piece.PieceNumber = 4;
			board.AddPiece(piece);
			board.MovePiece(File.D, Rank.Two, piece, true);
			piece.Clicked += handlePieceClicked;
			
			piece = packedScene.Instantiate<ChessPiece>();
			piece.OverrideMaterial = materialWhite;
			piece.Team = Teams.White;
			piece.PieceNumber = 5;
			board.AddPiece(piece);
			board.MovePiece(File.E, Rank.Two, piece, true);
			piece.Clicked += handlePieceClicked;
			
			piece = packedScene.Instantiate<ChessPiece>();
			piece.OverrideMaterial = materialWhite;
			piece.Team = Teams.White;
			piece.PieceNumber = 6;
			board.AddPiece(piece);
			board.MovePiece(File.F, Rank.Two, piece, true);
			piece.Clicked += handlePieceClicked;
			
			piece = packedScene.Instantiate<ChessPiece>();
			piece.OverrideMaterial = materialWhite;
			piece.Team = Teams.White;
			piece.PieceNumber = 7;
			board.AddPiece(piece);
			board.MovePiece(File.G, Rank.Two, piece, true);
			piece.Clicked += handlePieceClicked;
			
			piece = packedScene.Instantiate<ChessPiece>();
			piece.OverrideMaterial = materialWhite;
			piece.Team = Teams.White;
			piece.PieceNumber = 8;
			board.AddPiece(piece);
			board.MovePiece(File.H, Rank.Two, piece, true);
			piece.Clicked += handlePieceClicked;
			
			piece = packedScene.Instantiate<ChessPiece>();
			board.AddPiece(piece);
			board.MovePiece(File.A, Rank.Seven, piece, true);
			piece.Clicked += handlePieceClicked;
			
			piece = packedScene.Instantiate<ChessPiece>();
			piece.PieceNumber = 2;
			board.AddPiece(piece);
			board.MovePiece(File.B, Rank.Seven, piece, true);
			piece.Clicked += handlePieceClicked;
			
			piece = packedScene.Instantiate<ChessPiece>();
			piece.PieceNumber = 3;
			board.AddPiece(piece);
			board.MovePiece(File.C, Rank.Seven, piece, true);
			piece.Clicked += handlePieceClicked;
			
			piece = packedScene.Instantiate<ChessPiece>();
			piece.PieceNumber = 4;
			board.AddPiece(piece);
			board.MovePiece(File.D, Rank.Seven, piece, true);
			piece.Clicked += handlePieceClicked;
			
			piece = packedScene.Instantiate<ChessPiece>();
			piece.PieceNumber = 5;
			board.AddPiece(piece);
			board.MovePiece(File.E, Rank.Seven, piece, true);
			piece.Clicked += handlePieceClicked;
			
			piece = packedScene.Instantiate<ChessPiece>();
			piece.PieceNumber = 6;
			board.AddPiece(piece);
			board.MovePiece(File.F, Rank.Seven, piece, true);
			piece.Clicked += handlePieceClicked;
			
			piece = packedScene.Instantiate<ChessPiece>();
			piece.PieceNumber = 7;
			board.AddPiece(piece);
			board.MovePiece(File.G, Rank.Seven, piece, true);
			piece.Clicked += handlePieceClicked;
			
			piece = packedScene.Instantiate<ChessPiece>();
			piece.PieceNumber = 8;
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
			piece.PieceNumber = 2;
			board.AddPiece(piece);
			board.MovePiece(File.H, Rank.One, piece, true);
			piece.Clicked += handlePieceClicked;
			
			piece = packedScene.Instantiate<ChessPiece>();
			board.AddPiece(piece);
			board.MovePiece(File.A, Rank.Eight, piece, true);
			piece.Clicked += handlePieceClicked;
			
			piece = packedScene.Instantiate<ChessPiece>();
			piece.PieceNumber = 2;
			board.AddPiece(piece);
			board.MovePiece(File.H, Rank.Eight, piece, true);
			piece.Clicked += handlePieceClicked;
		}
	}
	
	private void toggleMainMenu()
	{
		if(mainMenu.Visible)
		{
			mainMenu.Hide();
			moveLogView.Show();
			
			gameState.Status = GameStatus.Paused;
			
			if(selectedPiece is null)
				board.EnablePieceSelection(gameState.CurrentPlayer);
			else
				board.EnableCellSelection(gameState.CurrentPlayer);
		}
		else
		{
			mainMenu.Show();
			moveLogView.Hide();
			gameState.Status = GameStatus.Playing;
			board.DisableAllCellSelection();
			board.DisableAllPieceSelection();
		}
	}
}
