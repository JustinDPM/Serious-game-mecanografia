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
		musicPlayer.VolumeDb = 0f;
		musicPlayer.Play();
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
			0f,
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
			0f,
			0.4f
		);

		musicTween.TweenCallback(
			Callable.From(() =>
			{
				comboMusicPlayer.Stop();
			})
		);
	}
}
