using System;
using Godot;

public static class GMath
{
	// Interpolations Methode [sauber bewegung zwischen 2 punkten]
	public static float FInterpTo(float A, float B, float DeltaSeconds, 
	float InterpolationSpeed = 6)
	{
		if (InterpolationSpeed <= 0f || DeltaSeconds <= 0f)
			return B;

		float diff = B - A;
		if (Math.Abs(diff) < 1e-6f)
			return B;

		float delta = DeltaSeconds * InterpolationSpeed;
		
		if (delta > 1f) delta = 1f;
			return A + diff * delta;
	}
	
	public static Vector2 VInterpTo(Vector2 A, Vector2 B, float DeltaSeconds,
	float InterpolationSpeed = 6)
	{
		 return new Vector2(
			FInterpTo(A.X, B.X, DeltaSeconds, InterpolationSpeed),
			FInterpTo(A.Y, B.Y, DeltaSeconds, InterpolationSpeed)
		);
	}
	
	
}
