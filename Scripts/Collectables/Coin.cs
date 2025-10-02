using Godot;

public partial class Coin : Area2D
{
	[Export] public int CoinValue = 1;

	public override void _Ready()
	{
		BodyEntered += OnBodyEntered;
	}

	private void OnBodyEntered(Node2D body)
	{
		if (body.GetType().Name == "Player")
		{
			CollectCoin();
		}
	}

	private void CollectCoin()
	{
		var ui = GetTree().CurrentScene.GetNodeOrNull<PlayerUI>("PlayerUI");
		if (ui != null)
		{
			ui.AddCoin(CoinValue);
		}

		QueueFree();
	}
}
