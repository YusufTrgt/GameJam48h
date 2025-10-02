using Godot;
using System;
using System.Collections.Generic;

public partial class PlayerUI : CanvasLayer
{
	private ProgressBar staminaBar;
	private HBoxContainer heartsContainer;
	private Label coinCounterLabel;
	private Player player;
	private int coinCount = 0;
	private StyleBoxFlat staminaBarStyle;
	
	public override void _Ready()
	{
		staminaBar = GetNode<ProgressBar>("PlayerStatsVBox/StaminaBarBackground/StaminaBar");
		heartsContainer = GetNode<HBoxContainer>("PlayerStatsVBox/HeartsContainer");
		coinCounterLabel = GetNode<Label>("PlayerStatsVBox/Coin_ScoreContainer/HeightAdjustment__ScoreToCoinAmount/CoinContainer/CoinCount");
		
		SetupCoinSprite();
		CreateStaminaBarStyle();
		CreateHearts();
		UpdateCoinCounter();
		
		CallDeferred(nameof(FindPlayer));
		
		GameEvents.Instance.CoinCountChanged += OnCoinCountChanged;
	}
	
	private void FindPlayer()
	{
		player = GetTree().GetFirstNodeInGroup("player") as Player;
		if (player == null)
		{
			GD.PrintErr("Player not found!");
		}
	}
	
	private void CreateStaminaBarStyle()
	{
		staminaBarStyle = new StyleBoxFlat();
		staminaBarStyle.BgColor = new Color(0.2f, 0.5f, 1.0f);
		staminaBarStyle.CornerRadiusTopLeft = 4;
		staminaBarStyle.CornerRadiusTopRight = 4;
		staminaBarStyle.CornerRadiusBottomLeft = 4;
		staminaBarStyle.CornerRadiusBottomRight = 4;
		staminaBar.AddThemeStyleboxOverride("fill", staminaBarStyle);
		
		var bgStyle = new StyleBoxFlat();
		bgStyle.BgColor = new Color(0.15f, 0.15f, 0.15f, 0.8f);
		bgStyle.CornerRadiusTopLeft = 4;
		bgStyle.CornerRadiusTopRight = 4;
		bgStyle.CornerRadiusBottomLeft = 4;
		bgStyle.CornerRadiusBottomRight = 4;
		
		var staminaBg = GetNode<ProgressBar>("PlayerStatsVBox/StaminaBarBackground");
		staminaBg.AddThemeStyleboxOverride("fill", bgStyle);
	}
	
	private void CreateHearts()
	{
		for (int i = 0; i < 5; i++)
		{
			Label heart = new Label();
			heart.Text = "❤";
			heart.AddThemeColorOverride("font_color", new Color(1, 0, 0));
			heart.AddThemeFontSizeOverride("font_size", 24);
			heartsContainer.AddChild(heart);
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
		float staminaPercent = player.GetStaminaPercent();
		staminaBar.Value = staminaPercent * 100;
		
		int fullHearts = Mathf.CeilToInt(player.GetCurrentHealth() / 20.0f);
		fullHearts = Mathf.Clamp(fullHearts, 0, 5);
		
		for (int i = 0; i < heartsContainer.GetChildCount(); i++)
		{
			Label heart = heartsContainer.GetChild<Label>(i);
			if (i < fullHearts)
			{
				heart.Modulate = new Color(1, 0, 0, 1);
			}
			else
			{
				heart.Modulate = new Color(0.3f, 0.3f, 0.3f, 0.5f);
			}
		}
	}
	
	private void SetupCoinSprite()
	{
		var coinSprite = GetNode<AnimatedSprite2D>("PlayerStatsVBox/Coin_ScoreContainer/HeightAdjustment__ScoreToCoinAmount/CoinContainer/CoinSprite");
		var spriteFrames = GD.Load<SpriteFrames>("res://Assets/UI/coin_animation.tres");
		
		if (spriteFrames != null)
		{
			coinSprite.SpriteFrames = spriteFrames;
			coinSprite.Play("default");
		}
		else
		{
			GD.PrintErr("Coin SpriteFrames not found.");
		}
	}
	
	private void OnCoinCountChanged(int newCount)
	{
		coinCount = newCount;
		UpdateCoinCounter();
	}
	
	private void UpdateCoinCounter()
	{
		coinCounterLabel.Text = $"x{coinCount}";
	}
	
	public override void _ExitTree()
	{
		GameEvents.Instance.CoinCountChanged -= OnCoinCountChanged;
	}
}
