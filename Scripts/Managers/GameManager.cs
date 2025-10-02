using Godot;

public partial class GameManager : Node2D
{
	[Export] private PauseMenu pauseMenu;

	private bool isPaused = false;

	public override void _Ready()
	{
		if (pauseMenu == null)
		{
			pauseMenu = GetNode<PauseMenu>("PauseMenu");
		}
	}

	public override void _Process(double delta)
	{
		if (Input.IsActionJustPressed("pause"))
		{
			TogglePause();
		}
	}

	public void TogglePause()
	{
		isPaused = !isPaused;

		if (isPaused)
		{
			pauseMenu?.Show();
			GetTree().Paused = true;
		}
		else
		{
			pauseMenu?.Hide();
			GetTree().Paused = false;
		}
	}
}
