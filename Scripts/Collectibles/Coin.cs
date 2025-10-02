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
		if (body is Player player)
		{
			CollectCoin(player);
		}
	}

	private void CollectCoin(Player player)
	{
		GD.Print($"+{CoinValue} coin(s)");
		GameEvents.Instance.RaiseCoinCollected(CoinValue, GlobalPosition);
		QueueFree();
	}
}
