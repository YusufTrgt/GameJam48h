using Godot;

public partial class Game : Node2D
{
	[Export] private CanvasLayer pauseMenuLayer;
	
	private Control pauseMenu;
	private Button resumeButton;
	private Button quitButton;
	private bool paused;

	public override void _Ready(){
		if (pauseMenuLayer != null) {
			pauseMenu = pauseMenuLayer.GetNode<Control>("Control");
			if (pauseMenu != null) {
				pauseMenu.Hide();
				
				// Buttons finden
				resumeButton = pauseMenu.GetNode<Button>("CenterContainer/MainContent/ButtonsContainer/ResumeButton");
				quitButton = pauseMenu.GetNode<Button>("CenterContainer/MainContent/ButtonsContainer/QuitButton");
				
				// Signale verbinden
				if (resumeButton != null) {
					resumeButton.Pressed += OnResumePressed;
					GD.Print("Resume Button verbunden!");
				}
				if (quitButton != null) {
					quitButton.Pressed += OnQuitPressed;
					GD.Print("Quit Button verbunden!");
				}
				
				GD.Print("PauseMenu gefunden!");
			} else {
				GD.PrintErr("Control-Node nicht gefunden!");
			}
		} else {
			GD.PrintErr("PauseMenu-Layer ist null!");
		}
	}

	public override void _Process(double delta){
		if (Input.IsActionJustPressed("pause")) TogglePauseMenu();
	}

	private void TogglePauseMenu(){
		if (pauseMenu == null) return;
		
		paused = !paused;
		
		if (paused){
			pauseMenu.Show();
			Engine.TimeScale = 0;
		}
		else{
			pauseMenu.Hide();
			Engine.TimeScale = 1;
		}
	}
	
	private void OnResumePressed(){
		GD.Print("Resume geklickt!");
		TogglePauseMenu();
	}
	
	private void OnQuitPressed(){
		GD.Print("Quit geklickt!");
		GetTree().Quit();
	}
}
