using System.Collections.Generic;
using System.Linq;
using Chess.Autoload;
using Chess.Gameplay;
using Chess.Nodes;

namespace Chess.Cpu;

public class Brain
{
	/*
	Steps to move:
		1) Select piece
		2) Get valid moves for piece
		3) Select destination
		4) Move the piece
	*/
	
	public Chessboard Board { get; set; }
	public MoveLog MoveLog { get; set; }
	
	public BoardCell SelectDestination(ChessPiece mover)
	{
		var moves = MoveLogic.GetValidCells(mover, Board, MoveLog);
		var scored = scoreMoves(moves);
		
		return scored
			.Aggregate((highest, pair) => pair.Value > highest.Value ? pair : highest)
			.Key;
	}
	
	private static Dictionary<BoardCell, int> scoreMoves(List<BoardCell> moves)
	{
		Dictionary<BoardCell, int> scored = [];
		
		int score;
		moves.ForEach(m => {
			score = 1;
			
			var query = m.GetChildren().Where(c => c is ChessPiece);
			if(query.Any())
			{
				//Incentivize capturing over non-capturing moves
				score++;
				
				var piece = query.Cast<ChessPiece>().FirstOrDefault();
				switch(piece.Type)
				{
					case Piece.Bishop:
						score += 2;
						break;
					
					case Piece.Knight:
					case Piece.Rook:
						score++;
						break;
					
					case Piece.Queen:
						score += 3;
						break;
				}
			}
			
			//Predict check, give priority to putting the opponent in check
			
			scored.Add(m, score);
		});
		
		return scored;
	}
}
