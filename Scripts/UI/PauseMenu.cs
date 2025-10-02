using Godot;

public partial class PauseMenu : CanvasLayer
{
	private Control controlNode;
	private Button resumeButton;
	private Button quitButton;

	public override void _Ready()
	{
		controlNode = GetNode<Control>("Control");
		resumeButton = GetNode<Button>("Control/CenterContainer/MainContent/ButtonsContainer/ResumeButton");
		quitButton = GetNode<Button>("Control/CenterContainer/MainContent/ButtonsContainer/QuitButton");

		resumeButton.Pressed += OnResumePressed;
		quitButton.Pressed += OnQuitPressed;

		HideMenu();
	}

	public new void Show()
	{
		controlNode.Show();
	}

	public new void Hide()
	{
		HideMenu();
	}

	private void HideMenu()
	{
		controlNode.Hide();
	}

	private void OnResumePressed()
	{
		GetParent<GameManager>()?.TogglePause();
	}

	private void OnQuitPressed()
	{
		GetTree().Quit();
	}
}
