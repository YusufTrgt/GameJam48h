using Godot;
using System;

// Component Scripts sind Klassen die man direkt in die Welt setzen kann
// d.h. man kann jetzt wie in diesem beispiel hier direkt eine kamera mit 
// custom code in die welt implementieren ohne das man dafuer ein script welches
// man auch woanders geben kann machen muss [sollte sauberere sein weil dadurch 
// die restlichen scripts auch bewusst woanders verwendet werden koennen]

// Kamera folgt dem Spieler smooth und zentriert

[GlobalClass]
public partial class FollowCameraComponent : Camera2D
{
	[Export] private CharacterBody2D player;
	[Export] private float deadzoneHeight = 50f;
	[Export] private float smoothing = 4.0f;
	
	// Optional: Aktiviere dies für Look-Ahead Effekt beim Sprinten
	[Export] private bool enableLookAhead = false;
	[Export] private float horizontalOffset = 50.0f;
	[Export] private float deadzoneWidth = 15.0f;
	
	private float currentSmoothingValue;
	private bool HasExitedXDeadzone = false;
	private float CurrentXOffset = 0f;
	private float SelectedXVelocity;
	private bool FollowingPlayer = false;
	private bool isInitialized = false;

	public override void _Ready()
	{
		currentSmoothingValue = smoothing;
		
		// Initialisiere Kamera-Position direkt auf dem Player
		if (player != null)
		{
			GlobalPosition = player.GlobalPosition;
			isInitialized = true;
		}
	}

	public override void _PhysicsProcess(double delta)
	{
		// Fallback für erste Frame
		if (!isInitialized && player != null)
		{
			GlobalPosition = player.GlobalPosition;
			isInitialized = true;
		}
		
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

		// === VERTIKALE BEWEGUNG (mit Deadzone) ===
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
			CamPos.Y = Mathf.Lerp(CamPos.Y, PlayerPos.Y + deadzoneHeight, currentSmoothingValue * DeltaSeconds);
		
		// === HORIZONTALE BEWEGUNG ===
		if (enableLookAhead)
		{
			// Look-Ahead Effekt (original Code)
			float RightDeadzone = CamPos.X + deadzoneWidth;
			float LeftDeadzone = CamPos.X - deadzoneWidth;

			float PlayerVelX = GetPlayerVelocityX();
			float velocityThreshold = 10.0f;
			
			if (Mathf.Abs(PlayerVelX) <= velocityThreshold)
			{
				currentSmoothingValue = 2.0f;
				HasExitedXDeadzone = false;
				CurrentXOffset = 0.0f;
			}
			else
			{
				currentSmoothingValue = smoothing;
			}

			if (!HasExitedXDeadzone || !GMath.SameSign(PlayerVelX, SelectedXVelocity))
			{
				if (PlayerPos.X > RightDeadzone)
				{
					CurrentXOffset = horizontalOffset;
					SelectedXVelocity = 1.0f;
					HasExitedXDeadzone = true;
				}
				else if (PlayerPos.X < LeftDeadzone)
				{
					CurrentXOffset = -horizontalOffset;
					SelectedXVelocity = -1.0f;
					HasExitedXDeadzone = true;
				}
			}

			CamPos.X = Mathf.Lerp(CamPos.X, PlayerPos.X + CurrentXOffset, currentSmoothingValue * DeltaSeconds);
		}
		else
		{
			// Einfache zentrierte Kamera (Standard)
			CamPos.X = Mathf.Lerp(CamPos.X, PlayerPos.X, smoothing * DeltaSeconds);
		}
		
		GlobalPosition = new Vector2(CamPos.X, CamPos.Y);
	}
	
	// prueft ob der spieler den boden beruehrt
	private bool PlayerIsGrounded()
	{
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
