using Godot;

public partial class MainMenuManager : Node
{
	[Export] public Button quitButton;
	[Export] public Button startButton;
	
	 public override void _Ready()
	{
		startButton.Pressed += OnStartButtonPressed;
		quitButton.Pressed += OnQuitButtonPressed;
	}

	private void OnQuitButtonPressed()
	{
	   GetTree().Quit();
	}

	private void OnStartButtonPressed()
	{
		GetTree().ChangeSceneToFile("res://Scenes/Levels/Game.tscn");
	}
}
