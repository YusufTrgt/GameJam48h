using Godot;

public partial class PlayerCamera : Camera2D
{
	[Export] private CharacterBody2D target;
	[Export] private Vector2 offset = Vector2.Zero;

	public override void _Ready()
	{
		if (target != null)
		{
			GlobalPosition = target.GlobalPosition + offset;
		}
	}

	public override void _PhysicsProcess(double delta)
	{
		if (target == null) return;
		GlobalPosition = target.GlobalPosition + offset;
	}

	public void SetTarget(CharacterBody2D newTarget)
	{
		target = newTarget;
	}
}
