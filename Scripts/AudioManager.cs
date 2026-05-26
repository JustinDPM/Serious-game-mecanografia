using Godot;
 
public partial class AudioManager : Node
{
	private AudioStreamPlayer musicPlayer;
	private AudioStreamPlayer comboMusicPlayer;
 
	[Export] public AudioStream MenuMusic;
	[Export] public AudioStream GameMusic;
	[Export] public AudioStream ComboMusic;
 
	[Export] public AudioStream ShootSFX;
	[Export] public AudioStream MeteorDestroySFX;
	[Export] public AudioStream MeteorDestroyedSFX;
	[Export] public AudioStream TurretDamageSFX;
	[Export] public AudioStream HealSFX;
 
	private Tween musicTween;
 
	public override void _Ready()
	{
		musicPlayer = new AudioStreamPlayer();
		comboMusicPlayer = new AudioStreamPlayer();
 
		AddChild(musicPlayer);
		AddChild(comboMusicPlayer);
 
		musicPlayer.Bus = "Music";
		comboMusicPlayer.Bus = "Music";
 
		comboMusicPlayer.VolumeDb = -80f;
		
		LoadAudioSettings();
	}
 
	public void PlayMenuMusic()
	{
		PlayMusic(MenuMusic);
	}
 
	public void PlayGameMusic()
	{
		PlayMusic(GameMusic);
	}
 
	private void PlayMusic(AudioStream stream)
	{
		if (stream == null)
			return;
 
		musicTween?.Kill();
 
		if (musicPlayer.Stream == stream && musicPlayer.Playing)
			return;
 
		comboMusicPlayer.Stop();
		comboMusicPlayer.VolumeDb = -80f;
 
		musicPlayer.Stream = stream;
		musicPlayer.VolumeDb = -6f;
		musicPlayer.Play();
	}
 
	public void StopMusic()
	{
		musicTween?.Kill();
		if (musicPlayer != null)
		{
			musicPlayer.Stop();
			musicPlayer.Stream = null;
		}
		if (comboMusicPlayer != null)
		{
			comboMusicPlayer.Stop();
		}
	}
 
	public void PlayShoot()
	{
		PlaySFX(ShootSFX, 2f);
	}
 
	public void PlayMeteorDestroy()
	{
		PlaySFX(MeteorDestroySFX, 1f);
	}
 
	public void PlayMeteorDestroyed()
	{
		PlaySFX(MeteorDestroyedSFX, 3f);
	}
 
	public void PlayTurretDamage()
	{
		PlaySFX(TurretDamageSFX, 10f);
	}
 
	public void PlayHeal()
	{
		PlaySFX(HealSFX, 3f);
	}
 
	private void PlaySFX(AudioStream stream, float volumeDb = 0f)
	{
		if (stream == null)
			return;
 
		var player = new AudioStreamPlayer();
		AddChild(player);
 
		player.Bus = "SFX";
		player.Stream = stream;
		player.VolumeDb = volumeDb;
		player.Play();
 
		player.Finished += () =>
		{
			player.QueueFree();
		};
	}
 
	public void StartComboMusic()
	{
		if (ComboMusic == null)
			return;
 
		if (comboMusicPlayer.Playing)
			return;
 
		musicTween?.Kill();
 
		comboMusicPlayer.Stream = ComboMusic;
		comboMusicPlayer.VolumeDb = -80f;
		comboMusicPlayer.Play();
 
		musicTween = GetTree().CreateTween();
 
		musicTween.TweenProperty(
			musicPlayer,
			"volume_db",
			-14f,
			0.4f
		);
 
		musicTween.Parallel().TweenProperty(
			comboMusicPlayer,
			"volume_db",
			-6f, 
			0.4f
		);
	}
 
	public void StopComboMusic()
	{
		if (!comboMusicPlayer.Playing)
			return;
 
		musicTween?.Kill();
 
		musicTween = GetTree().CreateTween();
 
		musicTween.TweenProperty(
			comboMusicPlayer,
			"volume_db",
			-80f,
			0.4f
		);
 
		musicTween.Parallel().TweenProperty(
			musicPlayer,
			"volume_db",
			-6f, 
			0.4f
		);
 
		musicTween.TweenCallback(
			Callable.From(() =>
			{
				comboMusicPlayer.Stop();
			})
		);
	}
	
	// FIX: corregido typo "ublic" -> "public"
	public void SaveAudioSettings(float musicDb, float sfxDb)
	{
		int musicBus = AudioServer.GetBusIndex("Music");
		int sfxBus = AudioServer.GetBusIndex("SFX");
		AudioServer.SetBusVolumeDb(musicBus, musicDb);
		AudioServer.SetBusVolumeDb(sfxBus, sfxDb);
 
		var config = new ConfigFile();
		config.SetValue("Audio", "Music", musicDb);
		config.SetValue("Audio", "SFX", sfxDb);
		config.Save("user://settings.cfg");
	}
 
	public void LoadAudioSettings()
	{
		int musicBus = AudioServer.GetBusIndex("Music");
		int sfxBus = AudioServer.GetBusIndex("SFX");
		var config = new ConfigFile();
 
		if (config.Load("user://settings.cfg") == Error.Ok)
		{
			float musicDb = (float)config.GetValue("Audio", "Music", 0f);
			float sfxDb = (float)config.GetValue("Audio", "SFX", 0f);
 
			AudioServer.SetBusVolumeDb(musicBus, musicDb);
			AudioServer.SetBusVolumeDb(sfxBus, sfxDb);
		}
	}
}
