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
		public static readonly NodePath GameOver = new("%GameOver");
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
	private GameOver gameOver;
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
		gameOver = GetNode<GameOver>(NodePaths.GameOver);
		gameState = GetNode<GameState>(GameState.NodePath);
		mainMenu = GetNode<MainMenu>(NodePaths.MainMenu);
		moveLog = GetNode<MoveLog>(MoveLog.NodePath);
		moveLogView = GetNode<MoveLogView>(NodePaths.MoveLogView);
		whiteGraveyard = GetNode<Graveyard>(NodePaths.WhiteGraveyard);
		
		board.Capture += handleCapture;
		board.CellClicked += handleCellClicked;
		board.Checkmate += handleCheckmate;
		board.PieceHasMoved += handlePieceHasMoved;
		gameOver.StartNewGame += handleStartNewGame;
		gameState.StartNextTurn += handleStartNextTurn;
		mainMenu.StartNewGame += handleStartNewGame;
		
		generatePieces();
		
		handleStartNextTurn(gameState.CurrentPlayer);
		
		//Set up the initial game and ui state manually, since toggleMainMenu is
		//configured more to handle opening and closing the menu during play.
		mainMenu.Show();
		moveLogView.Hide();
		gameState.Status = GameStatus.NotStarted;
		board.DisableAllCellSelection();
		board.DisableAllPieceSelection();
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
		if(defender.Team == Team.Black)
			whiteGraveyard.BuryPiece(defender);
		else
			blackGraveyard.BuryPiece(defender);
		
		//Ensure that the check status is removed if the king captures the piece that put it in check
		if(attacker.Type == Piece.King)
			board.DetectCheck();
	}
	
	private void handleCellClicked(BoardCell cell)
	{
		if(selectedPiece is not null && MoveLogic.IsDestinationValid(selectedPiece, cell, board, moveLog))
			board.MovePiece(selectedPiece, cell);
	}
	
	private void handleCheckmate(Team winner)
	{
		deselectSelectedPiece();
		board.DisableAllCellSelection();
		board.DisableAllPieceSelection();
		
		gameState.Status = winner == gameState.PlayerTeam
			? GameStatus.Victory
			: GameStatus.Loss;
		
		moveLogView.Hide();
		gameOver.Show();
	}
	
	private void handlePieceClicked(ChessPiece piece)
	{
		deselectSelectedPiece();
		selectedPiece = piece;
		selectedPiece.ToggleSelected(true);
		board.EnableCellSelection(gameState.CurrentPlayer, true);
		
		var moves = MoveLogic.GetValidCells(piece, board, moveLog);
		
		if(piece.Type == Piece.King)
			CheckLogic.FilterKingMovesForCheck(piece, board, moveLog, ref moves);
		else
			CheckLogic.FilterMovesToProtectKing(piece, board, ref moves);
		
		moves.ForEach(c => c.ToggleIndicator(true));
		
		if(board.Pieces.Where(p => p.Type == Piece.King && p.Team == piece.Team).FirstOrDefault() is ChessPiece king
			&& king.GetParentOrNull<BoardCell>() is BoardCell cell && cell.InCheck)
		{
			cell.Hoverable = false;
		}
	}
	
	private void handlePieceHasMoved()
	{
		if(gameState.Status == GameStatus.Reseting)
		{
			moveLogView.EnableLogUpdates = false;
			piecesReset++;
			
			if(piecesReset >= 32)
			{
				moveLog.Clear();
				board.Pieces.ForEach(p => p.HasMoved = false);
				moveLogView.EnableLogUpdates = true;
				gameState.CurrentPlayer = Team.Black;
				gameState.Status = GameStatus.Playing;
				piecesReset = 0;
				deselectSelectedPiece();
				gameState.EndTurn();
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
		mainMenu.Hide();
		moveLogView.Show();
		gameOver.Hide();
		
		gameState.CurrentPlayer = Team.White;
		gameState.Status = GameStatus.Reseting;
		board.ResetPieces();
	}
	
	private void handleStartNextTurn(Team activePlayer)
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
			piece.Team = Team.White;
			board.AddPiece(piece);
			board.MovePiece(File.C, Rank.One, piece, true);
			piece.Clicked += handlePieceClicked;
			
			piece = packedScene.Instantiate<ChessPiece>();
			piece.OverrideMaterial = materialWhite;
			piece.Team = Team.White;
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
			piece.Team = Team.White;
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
			piece.Team = Team.White;
			board.AddPiece(piece);
			board.MovePiece(File.B, Rank.One, piece, true);
			piece.Clicked += handlePieceClicked;
			
			piece = packedScene.Instantiate<ChessPiece>();
			piece.OverrideMaterial = materialWhite;
			piece.Team = Team.White;
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
			piece.Team = Team.White;
			board.AddPiece(piece);
			board.MovePiece(File.A, Rank.Two, piece, true);
			piece.Clicked += handlePieceClicked;
			
			piece = packedScene.Instantiate<ChessPiece>();
			piece.OverrideMaterial = materialWhite;
			piece.Team = Team.White;
			piece.PieceNumber = 2;
			board.AddPiece(piece);
			board.MovePiece(File.B, Rank.Two, piece, true);
			piece.Clicked += handlePieceClicked;
			
			piece = packedScene.Instantiate<ChessPiece>();
			piece.OverrideMaterial = materialWhite;
			piece.Team = Team.White;
			piece.PieceNumber = 3;
			board.AddPiece(piece);
			board.MovePiece(File.C, Rank.Two, piece, true);
			piece.Clicked += handlePieceClicked;
			
			piece = packedScene.Instantiate<ChessPiece>();
			piece.OverrideMaterial = materialWhite;
			piece.Team = Team.White;
			piece.PieceNumber = 4;
			board.AddPiece(piece);
			board.MovePiece(File.D, Rank.Two, piece, true);
			piece.Clicked += handlePieceClicked;
			
			piece = packedScene.Instantiate<ChessPiece>();
			piece.OverrideMaterial = materialWhite;
			piece.Team = Team.White;
			piece.PieceNumber = 5;
			board.AddPiece(piece);
			board.MovePiece(File.E, Rank.Two, piece, true);
			piece.Clicked += handlePieceClicked;
			
			piece = packedScene.Instantiate<ChessPiece>();
			piece.OverrideMaterial = materialWhite;
			piece.Team = Team.White;
			piece.PieceNumber = 6;
			board.AddPiece(piece);
			board.MovePiece(File.F, Rank.Two, piece, true);
			piece.Clicked += handlePieceClicked;
			
			piece = packedScene.Instantiate<ChessPiece>();
			piece.OverrideMaterial = materialWhite;
			piece.Team = Team.White;
			piece.PieceNumber = 7;
			board.AddPiece(piece);
			board.MovePiece(File.G, Rank.Two, piece, true);
			piece.Clicked += handlePieceClicked;
			
			piece = packedScene.Instantiate<ChessPiece>();
			piece.OverrideMaterial = materialWhite;
			piece.Team = Team.White;
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
			piece.Team = Team.White;
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
			piece.Team = Team.White;
			board.AddPiece(piece);
			board.MovePiece(File.A, Rank.One, piece, true);
			piece.Clicked += handlePieceClicked;
			
			piece = packedScene.Instantiate<ChessPiece>();
			piece.OverrideMaterial = materialWhite;
			piece.Team = Team.White;
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
			
			if(gameState.Status == GameStatus.Loss || gameState.Status == GameStatus.Stalemate || gameState.Status == GameStatus.Victory)
				gameOver.Show();
			else
			{
				gameState.Status = GameStatus.Playing;
				
				if(selectedPiece is null)
					board.EnablePieceSelection(gameState.CurrentPlayer);
				else
					board.EnableCellSelection(gameState.CurrentPlayer);
			}
		}
		else
		{
			mainMenu.Show();
			moveLogView.Hide();
			
			if(gameState.Status != GameStatus.Playing)
				gameOver.Hide();
			else
			{
				gameState.Status = GameStatus.Paused;
				board.DisableAllCellSelection();
				board.DisableAllPieceSelection();
			}
		}
	}
}
