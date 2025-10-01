using Godot;
using System;
// using GMath;

// Component Scripts sind Klassen die man direkt in die Welt setzen kann
// d.h. man kann jetzt wie in diesem beispiel hier direkt eine kamera mit 
// custom code in die welt implementieren ohne das man dafuer ein script welches
// man auch woanders geben kann machen muss [sollte sauberere sein weil dadurch 
// die restlichen scripts auch bewusst woanders verwendet werden koennen]

// Kamera bewegt sich basierend auch auf der richtung des spielers

[GlobalClass]
public partial class FollowCameraComponent : Camera2D
{
	[Export] public CharacterBody2D Player;
	[Export] public float DeadzoneY = 30;
	[Export] public Vector2 LookAhead = new Vector2(100, 0);
	[Export] public float SmoothSpeed = 5f;

	private Vector2 targetPosition;
	private bool verticalLock = false; // Lock nur fuer die y achsen Deadzone
	
	public override void _Ready()
	{
		if (Player == null)
			return;

		targetPosition = GlobalPosition;
	}

	public override void _PhysicsProcess(double delta)
	{
		if (Player == null) return;

		Vector2 offset = Player.GlobalPosition - GlobalPosition;

		// y deadzone checken
		bool outsideDeadzoneY = Mathf.Abs(offset.Y) > DeadzoneY;

		// Lock aktivieren, wenn Deadzone verlassen wird
		if (outsideDeadzoneY)
			verticalLock = true;

		// Zielposition initialisieren
		targetPosition = GlobalPosition;

		// X-Achse: immer direkt mit Player + Look-Ahead
		targetPosition.X = Player.GlobalPosition.X + LookAhead.X * Mathf.Sign(Player.Velocity.X);

		// Y-Achse: nur bewegen, wenn Deadzone verlassen oder Lock aktiv
		if (outsideDeadzoneY || verticalLock)
		{
			targetPosition.Y = Player.GlobalPosition.Y - Mathf.Sign(offset.Y) * DeadzoneY;
			targetPosition.Y += LookAhead.Y * Mathf.Sign(Player.Velocity.Y);
		}

		// Wenn Spieler stoppt, einfach weiter zur letzten targetPosition interpolieren
		Vector2 velocity = Player.Velocity.Length() < 0.1f ? Vector2.Zero : Player.Velocity;

		// Smooth bewegen zur targetPosition
		GlobalPosition = GMath.VInterpTo(GlobalPosition, targetPosition, (float)delta, SmoothSpeed);
	}
}
