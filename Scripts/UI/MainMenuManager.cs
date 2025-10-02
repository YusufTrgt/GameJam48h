using Godot;

public partial class MainMenuManager : Control
{
	private Button quitButton;
	private Button startButton;

	public override void _Ready()
	{
		startButton = GetNode<Button>("CenterContainer/MainContent/ButtonsContainer/StartGameButton");
		quitButton = GetNode<Button>("CenterContainer/MainContent/ButtonsContainer/QuitGameButton");

		if (startButton != null)
		{
			startButton.Pressed += OnStartButtonPressed;
			GD.Print("StartButton connected!");
		}
		else
		{
			GD.PrintErr("StartButton not found!");
		}

		if (quitButton != null)
		{
			quitButton.Pressed += OnQuitButtonPressed;
			GD.Print("QuitButton connected!");
		}
		else
		{
			GD.PrintErr("QuitButton not found!");
		}
	}

	private void OnQuitButtonPressed()
	{
		GD.Print("Quit button pressed!");
		GetTree().Quit();
	}

	private void OnStartButtonPressed()
	{
		GD.Print("Start button pressed!");
		string scenePath = "res://Scenes/Levels/Level01.tscn";
		
		if (ResourceLoader.Exists(scenePath))
		{
			GD.Print($"Loading scene: {scenePath}");
			var error = GetTree().ChangeSceneToFile(scenePath);
			if (error != Error.Ok)
			{
				GD.PrintErr($"Failed to change scene! Error: {error}");
			}
		}
		else
		{
			GD.PrintErr($"Scene not found at: {scenePath}");
			GD.Print("Trying fallback path...");
			scenePath = "res://Scenes/game.tscn";
			if (ResourceLoader.Exists(scenePath))
			{
				GetTree().ChangeSceneToFile(scenePath);
			}
			else
			{
				GD.PrintErr("Fallback scene also not found!");
			}
		}
	}
}
