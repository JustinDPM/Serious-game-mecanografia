using Godot;

public partial class AudioManager : Node
{
    private AudioStreamPlayer musicPlayer;
    private AudioStreamPlayer comboMusicPlayer;
    private AudioStreamPlayer sfxPlayer;

    [Export] public AudioStream MenuMusic;
    [Export] public AudioStream GameMusic;
    [Export] public AudioStream ComboMusic;

    [Export] public AudioStream ShootSFX;
    [Export] public AudioStream MeteorDestroySFX;
    [Export] public AudioStream MeteorDestroyedSFX;

    public override void _Ready()
    {
        musicPlayer = new AudioStreamPlayer();
        comboMusicPlayer = new AudioStreamPlayer();
        sfxPlayer = new AudioStreamPlayer();

        AddChild(musicPlayer);
        AddChild(comboMusicPlayer);
        AddChild(sfxPlayer);

        musicPlayer.Bus = "Music";
        comboMusicPlayer.Bus = "Music";
        sfxPlayer.Bus = "SFX";

        comboMusicPlayer.VolumeDb = -80;
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

        if (musicPlayer.Stream == stream && musicPlayer.Playing)
            return;

        musicPlayer.Stream = stream;
        musicPlayer.VolumeDb = 0;
        musicPlayer.Play();

        comboMusicPlayer.Stop();
        comboMusicPlayer.VolumeDb = -80;
    }

    public void PlayShoot()
    {
        PlaySFX(ShootSFX);
    }

    public void PlayMeteorDestroy()
    {
        PlaySFX(MeteorDestroySFX);
    }
    
    public void PlayMeteorDestroyed()
    {
        PlaySFX(MeteorDestroyedSFX);
    }

    private void PlaySFX(AudioStream stream)
    {
        if (stream == null)
            return;

        var player = new AudioStreamPlayer();

        AddChild(player);

        player.Bus = "SFX";
        player.Stream = stream;
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

        comboMusicPlayer.Stream = ComboMusic;
        comboMusicPlayer.VolumeDb = -80;
        comboMusicPlayer.Play();

        var tween = GetTree().CreateTween();
        tween.TweenProperty(musicPlayer, "volume_db", -12f, 0.4f);
        tween.Parallel().TweenProperty(comboMusicPlayer, "volume_db", 0f, 0.4f);
    }

    public void StopComboMusic()
    {
        var tween = GetTree().CreateTween();
        tween.TweenProperty(comboMusicPlayer, "volume_db", -80f, 0.4f);
        tween.Parallel().TweenProperty(musicPlayer, "volume_db", 0f, 0.4f);
        tween.TweenCallback(Callable.From(() =>
        {
            comboMusicPlayer.Stop();
        }));
    }
}