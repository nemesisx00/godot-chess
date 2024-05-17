using Godot;
using Chess.Nodes;

namespace Chess.Gameplay;

public static class GameBoardLogic
{
	public static Chessboard.CaptureEventHandler HandleCapture { get; set; }
	public static Chessboard.CellClickedEventHandler HandleCellClicked { get; set; }
	public static Chessboard.CheckmateEventHandler HandleCheckmate { get; set; }
	public static Chessboard.PieceHasMovedEventHandler HandlePieceHasMoved { get; set; }
	public static ChessPiece.ClickedEventHandler HandlePieceClicked { get; set; }
	
	public static void GeneratePieces(Chessboard board)
	{
		ChessPiece piece;
		
		var materialWhite = GD.Load<StandardMaterial3D>(ChessMaterials.White);
		
		var packedScene = GD.Load<PackedScene>($"{ResourcePaths.Pieces}/Bishop.tscn");
		if(packedScene.CanInstantiate())
		{
			piece = packedScene.Instantiate<ChessPiece>();
			piece.OverrideMaterial = materialWhite;
			piece.Team = Team.White;
			board.AddPiece(piece);
			board.MovePiece(File.C, Rank.One, piece, true);
			piece.Clicked += HandlePieceClicked;
			
			piece = packedScene.Instantiate<ChessPiece>();
			piece.OverrideMaterial = materialWhite;
			piece.Team = Team.White;
			piece.PieceNumber = 2;
			board.AddPiece(piece);
			board.MovePiece(File.F, Rank.One, piece, true);
			piece.Clicked += HandlePieceClicked;
			
			piece = packedScene.Instantiate<ChessPiece>();
			board.AddPiece(piece);
			board.MovePiece(File.C, Rank.Eight, piece, true);
			piece.Clicked += HandlePieceClicked;
			
			piece = packedScene.Instantiate<ChessPiece>();
			piece.PieceNumber = 2;
			board.AddPiece(piece);
			board.MovePiece(File.F, Rank.Eight, piece, true);
			piece.Clicked += HandlePieceClicked;
		}
		
		packedScene = GD.Load<PackedScene>($"{ResourcePaths.Pieces}/King.tscn");
		if(packedScene.CanInstantiate())
		{
			piece = packedScene.Instantiate<ChessPiece>();
			piece.OverrideMaterial = materialWhite;
			piece.Team = Team.White;
			board.AddPiece(piece);
			board.MovePiece(File.E, Rank.One, piece, true);
			piece.Clicked += HandlePieceClicked;
			
			piece = packedScene.Instantiate<ChessPiece>();
			board.AddPiece(piece);
			board.MovePiece(File.E, Rank.Eight, piece, true);
			piece.Clicked += HandlePieceClicked;
		}
		
		packedScene = GD.Load<PackedScene>($"{ResourcePaths.Pieces}/Knight.tscn");
		if(packedScene.CanInstantiate())
		{
			piece = packedScene.Instantiate<ChessPiece>();
			piece.OverrideMaterial = materialWhite;
			piece.Team = Team.White;
			board.AddPiece(piece);
			board.MovePiece(File.B, Rank.One, piece, true);
			piece.Clicked += HandlePieceClicked;
			
			piece = packedScene.Instantiate<ChessPiece>();
			piece.OverrideMaterial = materialWhite;
			piece.Team = Team.White;
			piece.PieceNumber = 2;
			board.AddPiece(piece);
			board.MovePiece(File.G, Rank.One, piece, true);
			piece.Clicked += HandlePieceClicked;
			
			piece = packedScene.Instantiate<ChessPiece>();
			board.AddPiece(piece);
			board.MovePiece(File.B, Rank.Eight, piece, true);
			piece.Clicked += HandlePieceClicked;
			
			piece = packedScene.Instantiate<ChessPiece>();
			piece.PieceNumber = 2;
			board.AddPiece(piece);
			board.MovePiece(File.G, Rank.Eight, piece, true);
			piece.Clicked += HandlePieceClicked;
		}
		
		packedScene = GD.Load<PackedScene>($"{ResourcePaths.Pieces}/Pawn.tscn");
		if(packedScene.CanInstantiate())
		{
			piece = packedScene.Instantiate<ChessPiece>();
			piece.OverrideMaterial = materialWhite;
			piece.Team = Team.White;
			board.AddPiece(piece);
			board.MovePiece(File.A, Rank.Two, piece, true);
			piece.Clicked += HandlePieceClicked;
			
			piece = packedScene.Instantiate<ChessPiece>();
			piece.OverrideMaterial = materialWhite;
			piece.Team = Team.White;
			piece.PieceNumber = 2;
			board.AddPiece(piece);
			board.MovePiece(File.B, Rank.Two, piece, true);
			piece.Clicked += HandlePieceClicked;
			
			piece = packedScene.Instantiate<ChessPiece>();
			piece.OverrideMaterial = materialWhite;
			piece.Team = Team.White;
			piece.PieceNumber = 3;
			board.AddPiece(piece);
			board.MovePiece(File.C, Rank.Two, piece, true);
			piece.Clicked += HandlePieceClicked;
			
			piece = packedScene.Instantiate<ChessPiece>();
			piece.OverrideMaterial = materialWhite;
			piece.Team = Team.White;
			piece.PieceNumber = 4;
			board.AddPiece(piece);
			board.MovePiece(File.D, Rank.Two, piece, true);
			piece.Clicked += HandlePieceClicked;
			
			piece = packedScene.Instantiate<ChessPiece>();
			piece.OverrideMaterial = materialWhite;
			piece.Team = Team.White;
			piece.PieceNumber = 5;
			board.AddPiece(piece);
			board.MovePiece(File.E, Rank.Two, piece, true);
			piece.Clicked += HandlePieceClicked;
			
			piece = packedScene.Instantiate<ChessPiece>();
			piece.OverrideMaterial = materialWhite;
			piece.Team = Team.White;
			piece.PieceNumber = 6;
			board.AddPiece(piece);
			board.MovePiece(File.F, Rank.Two, piece, true);
			piece.Clicked += HandlePieceClicked;
			
			piece = packedScene.Instantiate<ChessPiece>();
			piece.OverrideMaterial = materialWhite;
			piece.Team = Team.White;
			piece.PieceNumber = 7;
			board.AddPiece(piece);
			board.MovePiece(File.G, Rank.Two, piece, true);
			piece.Clicked += HandlePieceClicked;
			
			piece = packedScene.Instantiate<ChessPiece>();
			piece.OverrideMaterial = materialWhite;
			piece.Team = Team.White;
			piece.PieceNumber = 8;
			board.AddPiece(piece);
			board.MovePiece(File.H, Rank.Two, piece, true);
			piece.Clicked += HandlePieceClicked;
			
			piece = packedScene.Instantiate<ChessPiece>();
			board.AddPiece(piece);
			board.MovePiece(File.A, Rank.Seven, piece, true);
			piece.Clicked += HandlePieceClicked;
			
			piece = packedScene.Instantiate<ChessPiece>();
			piece.PieceNumber = 2;
			board.AddPiece(piece);
			board.MovePiece(File.B, Rank.Seven, piece, true);
			piece.Clicked += HandlePieceClicked;
			
			piece = packedScene.Instantiate<ChessPiece>();
			piece.PieceNumber = 3;
			board.AddPiece(piece);
			board.MovePiece(File.C, Rank.Seven, piece, true);
			piece.Clicked += HandlePieceClicked;
			
			piece = packedScene.Instantiate<ChessPiece>();
			piece.PieceNumber = 4;
			board.AddPiece(piece);
			board.MovePiece(File.D, Rank.Seven, piece, true);
			piece.Clicked += HandlePieceClicked;
			
			piece = packedScene.Instantiate<ChessPiece>();
			piece.PieceNumber = 5;
			board.AddPiece(piece);
			board.MovePiece(File.E, Rank.Seven, piece, true);
			piece.Clicked += HandlePieceClicked;
			
			piece = packedScene.Instantiate<ChessPiece>();
			piece.PieceNumber = 6;
			board.AddPiece(piece);
			board.MovePiece(File.F, Rank.Seven, piece, true);
			piece.Clicked += HandlePieceClicked;
			
			piece = packedScene.Instantiate<ChessPiece>();
			piece.PieceNumber = 7;
			board.AddPiece(piece);
			board.MovePiece(File.G, Rank.Seven, piece, true);
			piece.Clicked += HandlePieceClicked;
			
			piece = packedScene.Instantiate<ChessPiece>();
			piece.PieceNumber = 8;
			board.AddPiece(piece);
			board.MovePiece(File.H, Rank.Seven, piece, true);
			piece.Clicked += HandlePieceClicked;
		}
		
		packedScene = GD.Load<PackedScene>($"{ResourcePaths.Pieces}/Queen.tscn");
		if(packedScene.CanInstantiate())
		{
			piece = packedScene.Instantiate<ChessPiece>();
			piece.OverrideMaterial = materialWhite;
			piece.Team = Team.White;
			board.AddPiece(piece);
			board.MovePiece(File.D, Rank.One, piece, true);
			piece.Clicked += HandlePieceClicked;
			
			piece = packedScene.Instantiate<ChessPiece>();
			board.AddPiece(piece);
			board.MovePiece(File.D, Rank.Eight, piece, true);
			piece.Clicked += HandlePieceClicked;
		}
		
		packedScene = GD.Load<PackedScene>($"{ResourcePaths.Pieces}/Rook.tscn");
		if(packedScene.CanInstantiate())
		{
			piece = packedScene.Instantiate<ChessPiece>();
			piece.OverrideMaterial = materialWhite;
			piece.Team = Team.White;
			board.AddPiece(piece);
			board.MovePiece(File.A, Rank.One, piece, true);
			piece.Clicked += HandlePieceClicked;
			
			piece = packedScene.Instantiate<ChessPiece>();
			piece.OverrideMaterial = materialWhite;
			piece.Team = Team.White;
			piece.PieceNumber = 2;
			board.AddPiece(piece);
			board.MovePiece(File.H, Rank.One, piece, true);
			piece.Clicked += HandlePieceClicked;
			
			piece = packedScene.Instantiate<ChessPiece>();
			board.AddPiece(piece);
			board.MovePiece(File.A, Rank.Eight, piece, true);
			piece.Clicked += HandlePieceClicked;
			
			piece = packedScene.Instantiate<ChessPiece>();
			piece.PieceNumber = 2;
			board.AddPiece(piece);
			board.MovePiece(File.H, Rank.Eight, piece, true);
			piece.Clicked += HandlePieceClicked;
		}
	}
}
