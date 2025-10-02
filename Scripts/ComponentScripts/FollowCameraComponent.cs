using Godot;
using System;

// Vereinfachte Kamera die den Spieler direkt folgt ohne Deadzone oder Smoothing

[GlobalClass]
public partial class FollowCameraComponent : Camera2D
{
	[Export] private CharacterBody2D player;
	
	// Optional: Offset falls du die Kamera leicht verschieben willst
	[Export] private Vector2 cameraOffset = Vector2.Zero;

	public override void _Ready()
	{
		// Initialisiere Kamera-Position direkt auf dem Player
		if (player != null)
		{
			GlobalPosition = player.GlobalPosition + cameraOffset;
		}
	}

	public override void _PhysicsProcess(double delta)
	{
		if (player == null) 
			return;

		// Setze die Kamera-Position direkt auf die Player-Position (kein Smoothing)
		GlobalPosition = player.GlobalPosition + cameraOffset;
	}
}
