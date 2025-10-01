using Godot;
using System;

public partial class Muenze : Area2D
{
	[Export] public int CoinValue = 1; // Wie viel die Münze wert ist
	
	// Optional: Sound effect beim Aufsammeln
	// [Export] public AudioStream collectSound;
	// private AudioStreamPlayer2D audioPlayer;

	public override void _Ready()
	{
		// Verbinde das Signal für Kollision
		BodyEntered += OnBodyEntered;
		
		// Optional: Audio Player setup
		// if (collectSound != null)
		// {
		//     audioPlayer = new AudioStreamPlayer2D();
		//     audioPlayer.Stream = collectSound;
		//     AddChild(audioPlayer);
		// }
	}

	private void OnBodyEntered(Node2D body)
	{
		// Prüfe ob es der Player ist
		if (body is Player player)
		{
			// Sammle die Münze ein
			CollectCoin(player);
		}
	}

	private void CollectCoin(Player player)
	{
		GD.Print($"+{CoinValue} coin(s)");
		
		// Finde das UI und update den Counter
		var ui = GetTree().Root.GetNode<PlayerUI>("Game/PlayerUI");
		if (ui != null)
		{
			ui.AddCoin(CoinValue);
		}
		
		// TODO: Sound abspielen
		// if (audioPlayer != null)
		// {
		//     audioPlayer.Play();
		//     // Warte bis Sound fertig ist, dann lösche
		//     await ToSignal(audioPlayer, AudioStreamPlayer2D.SignalName.Finished);
		// }
		
		// Optional: Particle Effect oder Animation vor dem Löschen
		// PlayCollectAnimation();
		
		// Lösche die Münze
		QueueFree();
	}
	
	// Optional: Collect Animation
	// private async void PlayCollectAnimation()
	// {
	//     // Fade out oder Scale animation
	//     Tween tween = CreateTween();
	//     tween.TweenProperty(this, "scale", Vector2.Zero, 0.3f);
	//     await ToSignal(tween, Tween.SignalName.Finished);
	// }
}
