using Godot;

public partial class CreditsScreen : Control
{
	public override void _Ready()
	{
		GetNode<Button>("CenterContainer/VBoxContainer/BackButton").Pressed += OnBackButtonPressed;
	}

	private void OnBackButtonPressed()
	{
		GetTree().ChangeSceneToFile("res://Scenes/UI/MainMenu.tscn");
	}
}
