using Godot;
using System;
using System.Collections.Generic;

public partial class PlayerUI : CanvasLayer
{
	// Stamina Bar Elemente
	private ProgressBar staminaBar;
	
	// Herzen Container
	private HBoxContainer heartsContainer;
	private List<TextureRect> hearts = new List<TextureRect>();
	
	// Referenz zum Player
	private Player player;
	
	// Farben für die Bars
	private StyleBoxFlat staminaBarStyle;
	
	public override void _Ready()
	{
		// Hole UI Elemente
		staminaBar = GetNode<ProgressBar>("StaminaBarBackground/StaminaBar");
		heartsContainer = GetNode<HBoxContainer>("HeartsContainer");
		
		// Erstelle Styles für die Stamina Bar
		CreateStaminaBarStyle();
		
		// Erstelle die Herzen
		CreateHearts();
		
		// Finde den Player in der Szene
		CallDeferred(nameof(FindPlayer));
	}
	
	private void FindPlayer()
	{
		player = GetTree().Root.GetNode<Player>("Game/CharacterBody2D");
		if (player == null)
		{
			GD.PrintErr("Player nicht gefunden! Stelle sicher, dass der Player Node 'CharacterBody2D' heißt.");
		}
	}
	
	private void CreateStaminaBarStyle()
	{
		// Stamina Bar Style (Blau)
		staminaBarStyle = new StyleBoxFlat();
		staminaBarStyle.BgColor = new Color(0.2f, 0.5f, 1.0f); // Schönes Blau
		staminaBarStyle.CornerRadiusTopLeft = 4;
		staminaBarStyle.CornerRadiusTopRight = 4;
		staminaBarStyle.CornerRadiusBottomLeft = 4;
		staminaBarStyle.CornerRadiusBottomRight = 4;
		staminaBar.AddThemeStyleboxOverride("fill", staminaBarStyle);
		
		// Background Style (Dunkel)
		var bgStyle = new StyleBoxFlat();
		bgStyle.BgColor = new Color(0.15f, 0.15f, 0.15f, 0.8f);
		bgStyle.CornerRadiusTopLeft = 4;
		bgStyle.CornerRadiusTopRight = 4;
		bgStyle.CornerRadiusBottomLeft = 4;
		bgStyle.CornerRadiusBottomRight = 4;
		
		var staminaBg = GetNode<ProgressBar>("StaminaBarBackground");
		staminaBg.AddThemeStyleboxOverride("fill", bgStyle);
	}
	
	private void CreateHearts()
	{
		// Erstelle 5 Herzen (kleiner)
		for (int i = 0; i < 5; i++)
		{
			Label heart = new Label();
			heart.Text = "❤";
			heart.AddThemeColorOverride("font_color", new Color(1, 0, 0)); // Rot
			heart.AddThemeFontSizeOverride("font_size", 24); // Kleiner: 24 statt 32
			heartsContainer.AddChild(heart);
			
			// Speichere als TextureRect für später (wir nutzen Label mit Emoji)
			// hearts.Add(heart); // Nicht nötig, da wir direkt auf Children zugreifen
		}
	}
	
	public override void _Process(double delta)
	{
		if (player != null)
		{
			UpdateUI();
		}
	}
	
	private void UpdateUI()
	{
		// Update Stamina Bar
		float staminaPercent = player.GetStaminaPercent();
		staminaBar.Value = staminaPercent * 100;
		
		// Update Herzen (5 Herzen = 100 HP, also 20 HP pro Herz)
		int fullHearts = Mathf.CeilToInt(player.GetCurrentHealth() / 20.0f);
		fullHearts = Mathf.Clamp(fullHearts, 0, 5);
		
		// Update die Sichtbarkeit der Herzen
		for (int i = 0; i < heartsContainer.GetChildCount(); i++)
		{
			Label heart = heartsContainer.GetChild<Label>(i);
			if (i < fullHearts)
			{
				heart.Modulate = new Color(1, 0, 0, 1); // Volles rotes Herz
			}
			else
			{
				heart.Modulate = new Color(0.3f, 0.3f, 0.3f, 0.5f); // Leeres/graues Herz
			}
		}
	}
}
