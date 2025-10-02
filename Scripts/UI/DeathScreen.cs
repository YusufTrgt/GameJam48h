using Godot;

public partial class DeathScreen : CanvasLayer
{
	private Button restartButton;
	private Button quitButton;

	public override void _Ready()
	{
		restartButton = GetNode<Button>("Control/CenterContainer/VBoxContainer/RestartButton");
		quitButton = GetNode<Button>("Control/CenterContainer/VBoxContainer/QuitButton");

		restartButton.Pressed += OnRestartPressed;
		quitButton.Pressed += OnQuitPressed;

		Hide();
	}

	private void OnRestartPressed()
	{
		GetTree().Paused = false;
		GetTree().ReloadCurrentScene();
	}

	private void OnQuitPressed()
	{
		GetTree().Paused = false;
		GetTree().ChangeSceneToFile("res://Scenes/UI/MainMenu.tscn");
	}
}
