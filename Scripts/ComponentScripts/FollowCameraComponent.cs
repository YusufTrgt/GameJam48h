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
	[Export] private CharacterBody2D player;
	[Export] private float horizontalOffset = 100f;
	[Export] private float deadzoneHeight = 50f;
	[Export] private float deadzoneWidth = 70f;
	[Export] private float smoothing = 0.1f;
	
	private bool HasExitedXDeadzone = false;
	private float CurrentXOffset = 0f;
	private float SelectedXVelocity;
	private bool FollowingPlayer = false;

	public override void _Ready()
	{
		CalculatePlayerOffsetAndFollow(0.0f, false);
	}

	public override void _PhysicsProcess(double delta)
	{
		CalculatePlayerOffsetAndFollow((float)delta);
	}
	
	// sorgt fuer das folgen des spielers
	// hat auch eine deadzone in der y achse sodass nicht bei jedem kleinen sprung die kamera sich bewegt
	private void CalculatePlayerOffsetAndFollow(float DeltaSeconds, bool addDeltaToCalculation = true)
	{
		 if (player == null) 
			return;

		Vector2 PlayerPos = player.GlobalPosition;
		Vector2 CamPos = GlobalPosition;

		float UpperDeadzone = CamPos.Y - deadzoneHeight;

		// wenn die deadzone ueberschritten wird bewegt sich die kamera nach oben
		if (PlayerPos.Y < UpperDeadzone)
		{
			CamPos.Y = PlayerPos.Y + deadzoneHeight;
			FollowingPlayer = true;
		}
		// wenn wieder innerhalb der deadzone dann folgt die kamera den spieler bis er landed
		else if (FollowingPlayer && PlayerIsGrounded())
		{
			FollowingPlayer = false;
		}

		// Wenn FollowingPlayer aktiv -> leicht nachziehen
		if (FollowingPlayer)
			CamPos.Y = Mathf.Lerp(CamPos.Y, PlayerPos.Y + deadzoneHeight, smoothing * DeltaSeconds);
		
		 // --- Horizontale Deadzone mit Offset ---
		float RightDeadzone = CamPos.X + deadzoneWidth;
		float LeftDeadzone = CamPos.X - deadzoneWidth;

		float PlayerVelX = GetPlayerVelocityX();
		if (Mathf.Abs(PlayerVelX) <= 0.03)
		{
			HasExitedXDeadzone = false;
			CurrentXOffset = 0.0f;
		}

		GD.Print("player x vel : ", PlayerVelX, " applied x vel : ", SelectedXVelocity);

		if (!HasExitedXDeadzone || !GMath.SameSign(PlayerVelX, SelectedXVelocity))
		{
			if (PlayerPos.X > RightDeadzone)
			{
				CurrentXOffset = horizontalOffset; // Offset nach rechts
				SelectedXVelocity = 1.0f;
				HasExitedXDeadzone = true;
			}
			else if (PlayerPos.X < LeftDeadzone)
			{
				CurrentXOffset = -horizontalOffset; // Offset nach links
				SelectedXVelocity = -1.0f;
				HasExitedXDeadzone = true;
			}
			//else
			//{
				//// Spieler in Deadzone → Offset nur zurücksetzen, wenn er stillsteht
				//if (Mathf.Abs(PlayerVelX) < 0.01f)
					//CurrentXOffset = 0f;
			//}
		}
		

		// Kamera X folgt dem Spieler mit Offset
		CamPos.X = Mathf.Lerp(CamPos.X, PlayerPos.X + CurrentXOffset, smoothing * DeltaSeconds);
		
		GlobalPosition = new Vector2(CamPos.X, CamPos.Y);
		
		GD.Print(HasExitedXDeadzone);
	}
	
	// prueft ob der spieler den boden beruehrt
	private bool PlayerIsGrounded()
	{
		// Beispiel: prüft, ob der Spieler am Boden ist
		if (player != null)
		{
			return player.IsOnFloor();
		}
		return true; // fallback wert
	}
	
	// bewegt sich der spieler ?
	private float GetPlayerVelocityX()
	{
		if (player != null)
		{
			return player.Velocity.X;
		}
		return 0f;
	}
}
