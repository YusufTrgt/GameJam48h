using Godot;

public partial class GameEvents : Node
{
	private static GameEvents _instance;
	public static GameEvents Instance
	{
		get
		{
			if (_instance == null)
			{
				_instance = new GameEvents();
				_instance.Name = "GameEvents";
			}
			return _instance;
		}
	}

	[Signal]
	public delegate void CoinCollectedEventHandler(int amount, Vector2 position);

	[Signal]
	public delegate void CoinCountChangedEventHandler(int newCount);

	[Signal]
	public delegate void PlayerDamagedEventHandler(float damage);

	[Signal]
	public delegate void PlayerHealedEventHandler(float amount);

	[Signal]
	public delegate void PlayerDiedEventHandler();

	[Signal]
	public delegate void PlayerRespawnedEventHandler(Vector2 position);

	[Signal]
	public delegate void StaminaChangedEventHandler(float current, float max);

	[Signal]
	public delegate void StaminaDepletedEventHandler();

	[Signal]
	public delegate void HealthChangedEventHandler(float current, float max);

	[Signal]
	public delegate void LevelCompletedEventHandler();

	[Signal]
	public delegate void CheckpointReachedEventHandler(Vector2 position);

	[Signal]
	public delegate void GamePausedEventHandler();

	[Signal]
	public delegate void GameResumedEventHandler();

	public void RaiseCoinCollected(int amount, Vector2 position)
	{
		EmitSignal(SignalName.CoinCollected, amount, position);
	}

	public void RaisePlayerDamaged(float damage)
	{
		EmitSignal(SignalName.PlayerDamaged, damage);
	}

	public void Initialize()
	{
		GD.Print("[GameEvents] Initialized");
	}

	public override void _ExitTree()
	{
		_instance = null;
	}
}
