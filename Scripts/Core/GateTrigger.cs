using Godot;

public partial class GateTrigger : Area2D
{
	[Export] private PackedScene dialogBoxScene;
	[Export(PropertyHint.MultilineText)] 
	private string dialogText = "Du hast das Tor erreicht! Finde einen Weg, es zu öffnen.";
	
	private bool hasBeenTriggered = false;

	public override void _Ready()
	{
		BodyEntered += OnBodyEntered;
	}

	private void OnBodyEntered(Node2D body)
	{
		if (hasBeenTriggered || !(body is Player))
		{
			return;
		}
		
		hasBeenTriggered = true;
		GetTree().Paused = true;

		if (dialogBoxScene != null)
		{
			DialogueBox dialogInstance = dialogBoxScene.Instantiate<DialogueBox>();
			GetTree().Root.AddChild(dialogInstance);
			dialogInstance.SetText(dialogText);
		}
	}
}
