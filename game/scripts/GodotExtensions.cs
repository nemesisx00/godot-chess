using Godot;

namespace Chess;

public static class GodotExtensions
{
	public static bool IsEqualApprox(this Vector3 self, Vector3 other, float tolerance) => self.IsEqualApprox(other, (double)tolerance);
	public static bool IsEqualApprox(this Vector3 self, Vector3 other, double tolerance)
	{
		return Mathf.IsEqualApprox(self.X, other.X, tolerance)
			&& Mathf.IsEqualApprox(self.X, other.X, tolerance)
			&& Mathf.IsEqualApprox(self.X, other.X, tolerance);
	}
	
	public static bool IsEqualApprox(this Transform3D self, Transform3D other, float tolerance) => self.IsEqualApprox(other, (double)tolerance);
	public static bool IsEqualApprox(this Transform3D self, Transform3D other, double tolerance)
	{
		return self.Origin.IsEqualApprox(other.Origin, tolerance)
			&& self.Basis.X.IsEqualApprox(other.Basis.X, tolerance)
			&& self.Basis.Y.IsEqualApprox(other.Basis.Y, tolerance)
			&& self.Basis.Z.IsEqualApprox(other.Basis.Z, tolerance);
	}
}
