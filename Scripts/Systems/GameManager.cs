using Godot;

public partial class GameManager : Node
{
	private static GameManager _instance;
	public static GameManager Instance => _instance;

	private bool _isPaused = false;
	public bool IsPaused => _isPaused;

	private int _totalCoins = 0;
	public int TotalCoins => _totalCoins;

	private int _currentLevelCoins = 0;
	public int CurrentLevelCoins => _currentLevelCoins;

	private Vector2 _lastCheckpoint = Vector2.Zero;
	public Vector2 LastCheckpoint => _lastCheckpoint;

	public override void _Ready()
	{
		_instance = this;
		GameEvents.Instance.Initialize();
		ConnectEvents();
		GD.Print("[GameManager] Initialized");
	}

	private void ConnectEvents()
	{
		var events = GameEvents.Instance;
		events.CoinCollected += OnCoinCollected;
		events.CheckpointReached += OnCheckpointReached;
		events.PlayerDied += OnPlayerDied;
		events.GamePaused += OnGamePaused;
		events.GameResumed += OnGameResumed;
	}

	private void OnCoinCollected(int amount, Vector2 position)
	{
		_currentLevelCoins += amount;
		_totalCoins += amount;
		GameEvents.Instance.EmitSignal(GameEvents.SignalName.CoinCountChanged, _currentLevelCoins);
		GD.Print($"[GameManager] Coin collected! Level: {_currentLevelCoins}, Total: {_totalCoins}");
	}

	private void OnCheckpointReached(Vector2 position)
	{
		_lastCheckpoint = position;
		GD.Print($"[GameManager] Checkpoint saved at {position}");
	}

	private void OnPlayerDied()
	{
		GD.Print("[GameManager] Player died - respawning...");
		GetTree().CreateTimer(1.0f).Timeout += () => { RespawnPlayer(); };
	}

	private void OnGamePaused()
	{
		_isPaused = true;
		GetTree().Paused = true;
	}

	private void OnGameResumed()
	{
		_isPaused = false;
		GetTree().Paused = false;
	}

	public void RespawnPlayer()
	{
		var player = GetTree().GetFirstNodeInGroup("player") as Player;
		if (player != null)
		{
			player.GlobalPosition = _lastCheckpoint;
			GameEvents.Instance.EmitSignal(GameEvents.SignalName.PlayerRespawned, _lastCheckpoint);
		}
		else
		{
			GD.PrintErr("[GameManager] Player not found in group 'player'!");
		}
	}

	public void ResetLevel()
	{
		_currentLevelCoins = 0;
		GetTree().ReloadCurrentScene();
	}

	public void LoadLevel(string scenePath)
	{
		_currentLevelCoins = 0;
		GetTree().ChangeSceneToFile(scenePath);
	}

	public void TogglePause()
	{
		if (_isPaused)
		{
			GameEvents.Instance.EmitSignal(GameEvents.SignalName.GameResumed);
		}
		else
		{
			GameEvents.Instance.EmitSignal(GameEvents.SignalName.GamePaused);
		}
	}

	public override void _ExitTree()
	{
		_instance = null;
	}
}
