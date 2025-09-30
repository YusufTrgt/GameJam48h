using Godot;
using System;

// folgende klasse ist zustaendig fuer die interaktion mit den 
// Buttons im Hauptmenu : einfachhaltshalber wird direkt hier auch die 
// Logik implementiert

public partial class MainMenuManager : Node
{
	[Export] public Button quitButton;
	[Export] public Button startButton;
	
	 public override void _Ready()
	{
		startButton.Pressed += OnStartButtonPressed;
		quitButton.Pressed += OnQuitButtonPressed;
	}

	// schliesst die applikation
	private void OnQuitButtonPressed()
	{
	   GetTree().Quit();
	}

	// oeffnet das erste level
	private void OnStartButtonPressed()
	{
		GetTree().ChangeSceneToFile("res://Scenes/game.tscn");
	}
}
