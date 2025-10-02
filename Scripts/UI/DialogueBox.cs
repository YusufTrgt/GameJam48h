using Godot;

public partial class DialogueBox : CanvasLayer
{
	private Label messageLabel;
	private Button closeButton;

	public override void _Ready()
	{
		messageLabel = GetNode<Label>("Panel/VBoxContainer/Label");
		closeButton = GetNode<Button>("Panel/VBoxContainer/Button");

		closeButton.Pressed += OnCloseButtonPressed;
	}

	public void SetText(string text)
	{
		messageLabel.Text = text;
	}

	private void OnCloseButtonPressed()
	{
		GetTree().Paused = false;
		QueueFree();
	}
}
