using System;
using Chess.Nodes;

namespace Chess;

public struct BoardVector(int file, int rank) : IEquatable<BoardVector>
{
	public int File { get; set; } = file;
	public int Rank { get; set; } = rank;
	
	public readonly int Magnitude => File + Rank;
	
	public BoardVector(BoardCell cell) : this((int)cell.File, (int)cell.Rank) {}
	
	public readonly bool Equals(BoardVector other) => File == other.File && Rank == other.Rank;
	public readonly override bool Equals(object obj) => obj is BoardVector bv && Equals(bv);
	public readonly override int GetHashCode() => HashCode.Combine(File, Rank);
	public readonly override string ToString() => $"{(File)File}{Rank + 1}";
	
	public static BoardVector operator -(BoardVector a, BoardVector b) => new(a.File - b.File, a.Rank - b.Rank);
	public static BoardVector operator +(BoardVector a, BoardVector b) => new(a.File + b.File, a.Rank + b.Rank);
	public static bool operator <(BoardVector a, BoardVector b) => a.File + a.Rank < b.File + b.Rank;
	public static bool operator <=(BoardVector a, BoardVector b) => a.File + a.Rank <= b.File + b.Rank;
	public static bool operator >(BoardVector a, BoardVector b) => a.File + a.Rank > b.File + b.Rank;
	public static bool operator >=(BoardVector a, BoardVector b) => a.File + a.Rank >= b.File + b.Rank;
	public static bool operator ==(BoardVector a, BoardVector b) => a.Equals(b);
	public static bool operator !=(BoardVector a, BoardVector b) => !a.Equals(b);
	
	public readonly bool EastOf(BoardVector other) => Rank == other.Rank && File > other.File;
	public readonly bool NorthOf(BoardVector other) => Rank > other.Rank && File == other.File;
	public readonly bool NorthEastOf(BoardVector other) => Rank > other.Rank && File > other.File;
	public readonly bool NorthWestOf(BoardVector other) => Rank > other.Rank && File < other.File;
	public readonly bool SouthOf(BoardVector other) => Rank < other.Rank && File == other.File;
	public readonly bool SouthEastOf(BoardVector other) => Rank < other.Rank && File > other.File;
	public readonly bool SouthWestOf(BoardVector other) => Rank < other.Rank && File < other.File;
	public readonly bool WestOf(BoardVector other) => Rank == other.Rank && File < other.File;
}
