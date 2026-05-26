using Godot;
using System;
 
public partial class SettingsMenu : Control
{
	private OptionButton displayMode;
	private HSlider masterSlider;
	private HSlider musicSlider;
	private HSlider sfxSlider;
	private CheckButton typingSoundToggle;
 
	private bool openedFromGame = false;
	[Export] private Control mainMenuPanel;
	
	private Button resumeButton;
	private Button exitButton;
 
	private Global _global;
	private AudioManager _audioManager;
 
	private int masterBus;
	private int musicBus;
	private int sfxBus;
 
	public override void _Ready()
	{
		Visible = false;
		_global = GetNodeOrNull<Global>("/root/Global");
		_audioManager = GetNodeOrNull<AudioManager>("/root/AudioManager");
 
		masterBus = AudioServer.GetBusIndex("Master");
		musicBus  = AudioServer.GetBusIndex("Music");
		sfxBus    = AudioServer.GetBusIndex("SFX");
 
		string basePath = "CenterContainer/PanelContainer/HBoxPrincipal/VBoxContainer/";
 
		displayMode       = GetNodeOrNull<OptionButton>(basePath + "OptionButton");
		masterSlider      = GetNodeOrNull<HSlider>(basePath + "HSlider");
		musicSlider       = GetNodeOrNull<HSlider>(basePath + "HSlider2");
		sfxSlider         = GetNodeOrNull<HSlider>(basePath + "HSlider3");
		typingSoundToggle = GetNodeOrNull<CheckButton>(basePath + "CheckButton");
		
		resumeButton = GetNodeOrNull<Button>("CenterContainer/PanelContainer/HBoxPrincipal/SideBar/Return");
		exitButton   = GetNodeOrNull<Button>("CenterContainer/PanelContainer/HBoxPrincipal/SideBar/Close");
		
		if (displayMode != null)
		{
			displayMode.Clear();
			displayMode.AddItem("Ventana", 0);
			displayMode.AddItem("Pantalla completa", 1);
			var mode = DisplayServer.WindowGetMode();
			displayMode.Select(mode == DisplayServer.WindowMode.Fullscreen ? 1 : 0);
			displayMode.ItemSelected += OnDisplayModeChanged;
		}

		ConfigurarSliderVolumen(masterSlider, masterBus, isMaster: true);
		ConfigurarSliderVolumen(musicSlider,  musicBus,  isMaster: false);
		ConfigurarSliderVolumen(sfxSlider,    sfxBus,    isMaster: false);
 
		if (typingSoundToggle != null)
		{
			typingSoundToggle.ButtonPressed = true;
			typingSoundToggle.Toggled += OnTypingSoundToggled;
		}
 
		if (resumeButton != null) 
		{
			resumeButton.Pressed += OnResumePressed;
			resumeButton.Text = "Continuar";
		}
		if (exitButton != null) exitButton.Pressed += OnExitPressed;
 
		Button[] botones = { resumeButton, exitButton };
		foreach (Button btn in botones)
		{
			if (btn != null) ConfigurarAnimacionBoton(btn);
		}
		
		GD.Print("Settings listos.");
	}

	private void ConfigurarSliderVolumen(HSlider slider, int busIndex, bool isMaster)
	{
		if (slider == null) return;
 
		slider.MinValue = 0.0001;
		slider.MaxValue = 1.0;
		slider.Step     = 0.01;

		float currentDb     = AudioServer.GetBusVolumeDb(busIndex);
		double currentLinear = Mathf.DbToLinear(currentDb);
 
		slider.Value = Math.Clamp(currentLinear, slider.MinValue, slider.MaxValue);
 
		slider.ValueChanged += (value) => 
		{
			float db = (float)Mathf.LinearToDb(value);
			AudioServer.SetBusVolumeDb(busIndex, db);
 			if (!isMaster && _audioManager != null)
			{
				float musicDb = AudioServer.GetBusVolumeDb(musicBus);
				float sfxDb   = AudioServer.GetBusVolumeDb(sfxBus);
				_audioManager.SaveAudioSettings(musicDb, sfxDb);
			}
		};
	}
 
	private void OnTypingSoundToggled(bool activado)
	{
		GD.Print("Sonido al teclear activado: " + activado);
	}
 
	public void Open(bool pauseGame = true)
	{
		openedFromGame = pauseGame;
		if (pauseGame) GetTree().Paused = true;
		Visible = true;
		
		RefrescarSlidersDesdeConfig();
		
		if (displayMode != null) displayMode.GrabFocus();
	}
 
	// FIX: lee el archivo y actualiza el valor visual de los sliders sin disparar el evento ValueChanged
	private void RefrescarSlidersDesdeConfig()
	{
		var config = new ConfigFile();
		if (config.Load("user://settings.cfg") != Error.Ok)
			return; // No hay archivo guardado todavía, dejamos los valores actuales
 
		float musicDb = (float)config.GetValue("Audio", "Music", 0f);
		float sfxDb   = (float)config.GetValue("Audio", "SFX",   0f);
 
		// Aplicar al bus (por si acaso)
		AudioServer.SetBusVolumeDb(musicBus, musicDb);
		AudioServer.SetBusVolumeDb(sfxBus,   sfxDb);
 
		// Actualizar sliders silenciosamente desconectando señales temporalmente
		if (musicSlider != null)
			musicSlider.SetValueNoSignal(Math.Clamp(Mathf.DbToLinear(musicDb), musicSlider.MinValue, musicSlider.MaxValue));
		if (sfxSlider != null)
			sfxSlider.SetValueNoSignal(Math.Clamp(Mathf.DbToLinear(sfxDb), sfxSlider.MinValue, sfxSlider.MaxValue));
	}
 
	public void OnResumePressed()
	{
		SceneTree arbol = GetTree();
		
		if (openedFromGame)
		{
			if (arbol != null) arbol.Paused = false;
		}
		else
		{
			if (mainMenuPanel != null) mainMenuPanel.Visible = true;
		}
		Visible = false;
	}
 
	public void OnExitPressed()
	{
		SceneTree arbol = GetTree(); 
 
		if (openedFromGame)
		{
			if (arbol != null) arbol.Paused = false;
			
			if (_global != null)
			{
				_global.CallDeferred("CambiarEscena", "res://Escenas/main_menu.tscn");
			}
			else
			{
				GD.PrintErr("🚨 Error: Autoload '_global' no encontrado al intentar salir.");
			}
		}
		else
		{
			if (arbol != null) 
			{
				arbol.Quit();
			}
		}
	}
 
	private void OnDisplayModeChanged(long index)
	{
		if (index == 0) DisplayServer.WindowSetMode(DisplayServer.WindowMode.Windowed);
		else            DisplayServer.WindowSetMode(DisplayServer.WindowMode.Fullscreen);
	}
	
	private void ConfigurarAnimacionBoton(Button boton)
	{
		boton.MouseEntered += () => 
		{
			boton.PivotOffset = boton.Size / 2; 
			Tween tween = CreateTween();
			tween.TweenProperty(boton, "scale", new Vector2(1.05f, 1.05f), 0.1f).SetTrans(Tween.TransitionType.Sine);
		};
 
		boton.MouseExited += () => 
		{
			boton.PivotOffset = boton.Size / 2;
			Tween tween = CreateTween();
			tween.TweenProperty(boton, "scale", new Vector2(1.0f, 1.0f), 0.1f).SetTrans(Tween.TransitionType.Sine);
		};
	}
}
 
