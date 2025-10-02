using Godot;

public partial class PlayerCamera : Camera2D
{
	[Export] private CharacterBody2D target;
	[Export] private Vector2 offset = Vector2.Zero;

	private float startY;
	private float maxFallDistance = 10f;

	public override void _Ready()
	{
		if (target != null)
		{
			GlobalPosition = target.GlobalPosition + offset;
		}
		startY = GlobalPosition.Y;
	}

	public override void _PhysicsProcess(double delta)
	{
		if (target == null) return;

		Vector2 newPosition = target.GlobalPosition + offset;

		if (newPosition.Y > startY + maxFallDistance)
		{
			newPosition.Y = startY + maxFallDistance;
		}

		GlobalPosition = newPosition;
	}

	public void SetTarget(CharacterBody2D newTarget)
	{
		target = newTarget;
	}
}
