using Godot;
using System;

public partial class ScoreSystemManager : Node
{
	// Aktueller Punktestand
	private int score = 0;

	public ScoreSystemManager() { }
	public ScoreSystemManager(int StartingScore)
	{
		score = StartingScore;
	}

	// Punkte hinzufügen
	public void AddPoints(int points)
	{
		score += points;
	}

	// Punktestand zurückgeben
	public int GetScore()
	{
		return score;
	}

	// Score zurücksetzen [falls notwendig]
	public void ResetScore()
	{
		score = 0;
	}
}
