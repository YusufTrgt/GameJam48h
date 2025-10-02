using Godot;

public partial class PlayerUI : CanvasLayer
{
	[Export] public NodePath PlayerPath;

	private ProgressBar staminaBar;
	private HBoxContainer heartsContainer;
	private Label coinCounterLabel;
	private CharacterBody2D player;
	private int coinCount = 0;

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
	}

	private void FindPlayer()
	{
		if (PlayerPath != null && !PlayerPath.IsEmpty)
		{
			player = GetNode<CharacterBody2D>(PlayerPath);
		}
		else
		{
			player = GetTree().CurrentScene.GetNodeOrNull<CharacterBody2D>("CharacterBody2D");
		}

		if (player == null)
		{
			GD.PrintErr("Player not found!");
		}
	}

	private void CreateStaminaBarStyle()
	{
		var staminaStyle = new StyleBoxFlat();
		staminaStyle.BgColor = new Color(0.2f, 0.5f, 1.0f);
		staminaStyle.CornerRadiusTopLeft = 4;
		staminaStyle.CornerRadiusTopRight = 4;
		staminaStyle.CornerRadiusBottomLeft = 4;
		staminaStyle.CornerRadiusBottomRight = 4;
		staminaBar.AddThemeStyleboxOverride("fill", staminaStyle);

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
		for (int i = 0; i < GameConstants.MAX_HEARTS; i++)
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
		float currentHealth = GetPlayerHealth();
		float maxHealth = GetPlayerMaxHealth();
		float currentStamina = GetPlayerStamina();
		float maxStamina = GetPlayerMaxStamina();

		float staminaPercent = currentStamina / maxStamina;
		staminaBar.Value = staminaPercent * 100;

		int fullHearts = Mathf.CeilToInt(currentHealth / GameConstants.HEALTH_PER_HEART);
		fullHearts = Mathf.Clamp(fullHearts, 0, GameConstants.MAX_HEARTS);

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

	private float GetPlayerHealth()
	{
		if (player == null) return 0;
		return (float)player.Get("currentHealth");
	}

	private float GetPlayerMaxHealth()
	{
		if (player == null) return 100;
		return (float)player.Get("MaxHealth");
	}

	private float GetPlayerStamina()
	{
		if (player == null) return 0;
		return (float)player.Get("currentStamina");
	}

	private float GetPlayerMaxStamina()
	{
		if (player == null) return 100;
		return (float)player.Get("MaxStamina");
	}

	private void SetupCoinSprite()
	{
		var coinSprite = GetNode<AnimatedSprite2D>("PlayerStatsVBox/Coin_ScoreContainer/HeightAdjustment__ScoreToCoinAmount/CoinContainer/CoinSprite");
		var spriteFrames = GD.Load<SpriteFrames>("res://Scenes/Collectables/Coin.tscn::SpriteFrames_sjilf");

		if (spriteFrames != null)
		{
			coinSprite.SpriteFrames = spriteFrames;
			coinSprite.Play("default");
		}
	}

	public void AddCoin(int amount = 1)
	{
		if (amount < 0) return;
		coinCount += amount;
		UpdateCoinCounter();
	}

	private void UpdateCoinCounter()
	{
		coinCounterLabel.Text = $"x{coinCount}";
	}

	public int GetCoinCount() => coinCount;
}
