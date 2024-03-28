using System.Collections.Generic;
using System.Linq;
using Godot;
using Chess.Autoload;
using Chess.Nodes;
using System;

namespace Chess.Gameplay;

public static class CheckLogic
{
	public static Teams? DetectCheckmate(Chessboard board, MoveLog moveLog)
	{
		Teams? team = null;
		if(DetectCheckmateForTeam(Teams.Black, board, moveLog))
			team = Teams.Black;
		else if(DetectCheckmateForTeam(Teams.White, board, moveLog))
			team = Teams.White;
		return team;
	}
	
	public static bool DetectCheckmateForTeam(Teams team, Chessboard board, MoveLog moveLog)
	{
		List<BoardCell> allMoves = [];
		foreach(var piece in board.Pieces.Where(p => p.Team == team))
		{
			if(piece.Type == Piece.King)
			{
				if(!IsInCheck(piece, board, moveLog))
					return false;
				else
				{
					var moves = MoveLogic.GetValidCells(piece, board, moveLog);
					FilterKingMovesForCheck(piece, board, moveLog, ref moves);
					allMoves.AddRange(moves);
				}
			}
			else
			{
				var moves = MoveLogic.GetValidCells(piece, board, moveLog);
				FilterMovesToProtectKing(piece, board, ref moves);
				allMoves.AddRange(moves);
			}
		}
		
		return allMoves.Count == 0;
	}
	
	/**
	<summary>
	Filter out potential moves which would put the king into check.
	</summary>
	*/
	public static void FilterKingMovesForCheck(ChessPiece king, Chessboard board, MoveLog moveLog, ref List<BoardCell> potentialMoves)
	{
		var pm = potentialMoves;
		var opposingPieces = board.Pieces.Where(p => p.Team != king.Team).ToList();
		
		List<BoardCell> conflicts = [];
		opposingPieces.ForEach(op => {
			var moves = MoveLogic.GetValidCells(op, board, moveLog);
			moves.Where(bc => pm.Contains(bc) && !conflicts.Contains(bc))
				.ToList()
				.ForEach(bc => conflicts.Add(bc));
		});
		
		conflicts.ForEach(c => pm.Remove(c));
		conflicts.Clear();
		
		pm.ForEach(dest => {
			if(detectKingMoveIntoCheck(king, dest, board))
				conflicts.Add(dest);
		});
		
		conflicts.ForEach(c => pm.Remove(c));
		
		potentialMoves = pm;
	}
	
	/**
	<summary>
	Filter out potential moves which would not protect the king when the king is in check.
	</summary>
	*/
	public static void FilterMovesToProtectKing(ChessPiece piece, Chessboard board, ref List<BoardCell> potentialMoves)
	{
		var king = board.Pieces
			.Where(p => p.Team == piece.Team && p.Type == Piece.King)
			.FirstOrDefault();
		
		if(king is not null && king.GetParentOrNull<BoardCell>() is BoardCell kingCell && kingCell.InCheck)
		{
			List<BoardCell> moves = [];
			
			potentialMoves.ForEach(cell => {
				if(PredictCheckByCell(king, cell))
					moves.Add(cell);
			});
			
			potentialMoves = moves;
		}
	}
	
	/**
	<summary>
	Detect if a king is in check.
	</summary>
	*/
	public static bool IsInCheck(ChessPiece king, Chessboard board, MoveLog moveLog)
	{
		var check = false;
		var kingCell = king.GetParent<BoardCell>();
		
		foreach(var ray in king.Rays)
		{
			if(check)
				break;
			
			ray.ForceRaycastUpdate();
			
			if(ray.GetCollider() is ChessPiece cp)
			{
				if(cp.Team != king.Team)
				{
					var colliderCell = cp.GetParentOrNull<BoardCell>();
					if(colliderCell is not null)
						check = canPieceCauseCheck(
							cp,
							DirectionNames.IsCardinal(ray.Name),
							kingCell - colliderCell
						);
				}
			}
		}
		
		if(!check)
		{
			foreach(var knight in board.Pieces.Where(p => p.Team != king.Team && p.Type == Piece.Knight && p.GetParent() is BoardCell))
			{
				if(check)
					break;
				
				check = MoveLogic.GetValidCells(knight, board, moveLog).Contains(kingCell);
			}
		}
		
		return check;
	}
	
	/**
	<summary>
	Predict if moving a piece will put the player's own king in check.
	</summary>
	*/
	public static bool PredictCheck(Chessboard board, ChessPiece mover)
		=> PredictCheck(board.Pieces.Where(p => p.Type == Piece.King && p.Team == mover.Team).First(), mover);
	
	/**
	<summary>
	Predict if moving a piece will put the player's own king in check.
	</summary>
	*/
	public static bool PredictCheck(ChessPiece king, ChessPiece mover)
	{
		var check = false;
		
		if(mover.Type != Piece.King && king.Team == mover.Team
			&& king.GetParentOrNull<BoardCell>() is BoardCell kingCell
			&& mover.GetParentOrNull<BoardCell>() is BoardCell moverCell)
		{
			var diff = kingCell - moverCell;
			string direction = DirectionNames.ByDifference(diff);
			
			if(!string.IsNullOrEmpty(direction))
			{
				var ray = king.Rays
					.Where(r => r.Name.Equals(direction))
					.FirstOrDefault();
				
				if(ray is not null)
				{
					var cardinal = DirectionNames.IsCardinal(direction);
					
					ray.AddException(mover);
					ray.ForceRaycastUpdate();
					ray.RemoveException(mover);
					
					if(ray.GetCollider() is ChessPiece cp
						&& cp.Team != king.Team
						&& cp.GetParentOrNull<BoardCell>() is BoardCell pieceCell
						//If the opposing piece is between the king and the mover, the mover cannot cause check by moving.
						&& (kingCell - pieceCell) > diff)
					{
						check = canPieceCauseCheck(cp, cardinal, diff);
					}
				}
			}
		}
		
		return check;
	}
	
	/**
	<summary>
	Predict if a piece moving into a destination cell can protect the king from being in check.
	</summary>
	*/
	public static bool PredictCheckByCell(ChessPiece king, BoardCell destination)
	{
		var check = false;
		
		if(king.GetParentOrNull<BoardCell>() is BoardCell kingCell)
		{
			var diff = kingCell - destination;
			var direction = DirectionNames.ByDifference(diff);
			
			if(!string.IsNullOrEmpty(direction))
			{
				var ray = king.Rays
					.Where(r => r.Name.Equals(direction))
					.FirstOrDefault();
				
				if(ray is not null)
				{
					var cardinal = DirectionNames.IsCardinal(direction);
					ray.ForceRaycastUpdate();
					
					if(ray.GetCollider() is ChessPiece cp
						&& cp.Team != king.Team
						&& cp.GetParentOrNull<BoardCell>() is BoardCell pieceCell
						//Equal magnitudes implies the destination would capture the piece causing check
						&& (kingCell - pieceCell) >= diff)
					{
						var inline = direction switch
						{
							DirectionNames.East => destination.ToVector().EastOf(kingCell.ToVector()),
							DirectionNames.North => destination.ToVector().NorthOf(kingCell.ToVector()),
							DirectionNames.NorthEast => destination.ToVector().NorthEastOf(kingCell.ToVector()),
							DirectionNames.NorthWest => destination.ToVector().NorthWestOf(kingCell.ToVector()),
							DirectionNames.South => destination.ToVector().SouthOf(kingCell.ToVector()),
							DirectionNames.SouthEast => destination.ToVector().SouthEastOf(kingCell.ToVector()),
							DirectionNames.SouthWest => destination.ToVector().SouthWestOf(kingCell.ToVector()),
							DirectionNames.West => destination.ToVector().WestOf(kingCell.ToVector()),
							_ => false,
						};
						
						check = inline && canPieceCauseCheck(cp, cardinal, diff);
					}
				}
			}
		}
		
		return check;
	}
	
	/**
	<summary>
	Evaluate if a given piece, based on its allowed means of movement, could cause check.
	</summary>
	*/
	private static bool canPieceCauseCheck(ChessPiece piece, bool cardinal, BoardVector diff)
	{
		bool check;
		if (cardinal)
			check = piece.Type == Piece.Queen || piece.Type == Piece.Rook;
		else
		{
			check = piece.Type == Piece.Bishop || piece.Type == Piece.Queen;
			
			if(!check)
			{
				check = piece.Type == Piece.Pawn && diff.File == 1
					&& (
						(piece.Team == Teams.White && diff.Rank == 1)
						|| (piece.Team == Teams.Black && diff.Rank == -1)
					);
			}
			
			if(!check)
			{
				check = piece.Type == Piece.Knight
					&& (
						(Math.Abs(diff.File) == 1 && Math.Abs(diff.Rank) == 2)
						|| (Math.Abs(diff.File) == 2 && Math.Abs(diff.Rank) == 1)
					);
			}
		}
		return check;
	}
	
	/**
	<summary>
	Detect if the king would be in check after moving to the destination cell
	without actually moving the king.
	</summary>
	*/
	private static bool detectKingMoveIntoCheck(ChessPiece king, BoardCell destination, Chessboard board)
	{
		var check = false;
		
		if(king.GetParentOrNull<BoardCell>() is BoardCell kingCell)
		{
			var diff = kingCell - destination;
			var pieceToCapture = destination.GetChildren().Where(c => c is ChessPiece).FirstOrDefault() as ChessPiece;
			
			board.PieceProxy.GlobalPosition = destination.GlobalPosition;
			
			foreach(var ray in board.PieceProxy.Rays)
			{
				if(pieceToCapture is not null)
					ray.AddException(pieceToCapture);
				
				ray.ForceRaycastUpdate();
				
				if(pieceToCapture is not null)
					ray.RemoveException(pieceToCapture);
				
				if(ray.GetCollider() is ChessPiece cp
					&& cp.Team != king.Team
					&& cp.GetParentOrNull<BoardCell>() is BoardCell pieceCell)
				{
					check = canPieceCauseCheck(cp, DirectionNames.IsCardinal(ray.Name), diff);
				}
				
				if(check)
					break;
			}
			
			//Knights can't be detected via raycast
			if(!check)
			{
				foreach(var knight in board.Pieces
					.Where(cp => cp.Team != king.Team
						&& cp.Type == Piece.Knight
						&& cp.GetParentOrNull<BoardCell>() is not null))
				{
					check = canPieceCauseCheck(knight, false, knight.GetParent<BoardCell>() - destination);
					
					if(check)
						break;
				}
			}
		}
		
		return check;
	}
}
