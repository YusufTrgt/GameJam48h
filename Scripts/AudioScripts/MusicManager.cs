using Godot;

public partial class MusicManager : Node
{
	public override void _Ready()
	{
		var p = new AudioStreamPlayer { Name = "AudioStreamPlayer", Bus = "Master", VolumeDb = -6f };
		AddChild(p);
		var s = GD.Load<AudioStream>("res://Assets/Sounds/Background/lfh_clean.ogg");
		if (s == null)
		{
			GD.PushError("[BGM] Stream == null (Pfad prüfen)!");
			return;
		}
		
		if (s is AudioStreamOggVorbis ov) ov.Loop = true;
		else if (s is AudioStreamWav w) w.LoopMode = AudioStreamWav.LoopModeEnum.Forward;
		p.Stream = s;
		p.StreamPaused = false;
		p.Play();
		GD.Print($"[BGM] Playing {p.Stream?.ResourcePath}, bus={p.Bus}, vol={p.VolumeDb}");
	}
}
