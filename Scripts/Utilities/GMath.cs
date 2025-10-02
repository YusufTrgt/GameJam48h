using System;
using Godot;

public static class GMath
{
	// Interpolations Methode [saubere bewegung zwischen 2 punkten]
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
	
	// Interpolations Methode [saubere bewegung zwischen 2 2d Punkten]
	public static Vector2 VInterpTo(Vector2 A, Vector2 B, float DeltaSeconds,
	float InterpolationSpeed = 6)
	{
		 return new Vector2(
			FInterpTo(A.X, B.X, DeltaSeconds, InterpolationSpeed),
			FInterpTo(A.Y, B.Y, DeltaSeconds, InterpolationSpeed)
		);
	}
	
	// nimmt den wert und sieht die range. mapped es dann in die erwartete range runter
	public static float MapRangeClamped(float value, float inMin, float inMax, float outMin, float outMax)
	{
		if (inMax - inMin == 0f) // Division durch null verhindern
			return outMin;

		// Linear map
		float t = (value - inMin) / (inMax - inMin);

		// Clamp auf 0-1
		t = Mathf.Clamp(t, 0f, 1f);

		// Auf Ausgabebereich anwenden
		return outMin + t * (outMax - outMin);
	}
	
	// prueft ob zwei zahlen dieselben vorzeichen haben
	public static bool SameSign(float A, float B)
	{
		return (A * B) >= 0;
	}
}
