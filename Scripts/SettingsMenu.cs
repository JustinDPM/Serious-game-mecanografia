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

	// Índices de los buses de audio de Godot
	private int masterBus;
	private int musicBus;
	private int sfxBus;

	public override void _Ready()
	{
		Visible = false;
		_global = GetNodeOrNull<Global>("/root/Global");

		// Obtener los canales de audio del sistema
		masterBus = AudioServer.GetBusIndex("Master");
		musicBus = AudioServer.GetBusIndex("Music");
		sfxBus = AudioServer.GetBusIndex("SFX");

		// Ubicación base de tus controles
		string basePath = "CenterContainer/PanelContainer/HBoxPrincipal/VBoxContainer/";

		displayMode = GetNodeOrNull<OptionButton>(basePath + "OptionButton");
		masterSlider = GetNodeOrNull<HSlider>(basePath + "HSlider");
		musicSlider = GetNodeOrNull<HSlider>(basePath + "HSlider2");
		sfxSlider = GetNodeOrNull<HSlider>(basePath + "HSlider3");
		typingSoundToggle = GetNodeOrNull<CheckButton>(basePath + "CheckButton");
		
		resumeButton = GetNodeOrNull<Button>("CenterContainer/PanelContainer/HBoxPrincipal/SideBar/Return");
		exitButton = GetNodeOrNull<Button>("CenterContainer/PanelContainer/HBoxPrincipal/SideBar/Close");
		
		// 1. Configurar Modo de Pantalla
		if (displayMode != null)
		{
			displayMode.Clear();
			displayMode.AddItem("Ventana", 0);
			displayMode.AddItem("Pantalla completa", 1);
			var mode = DisplayServer.WindowGetMode();
			displayMode.Select(mode == DisplayServer.WindowMode.Fullscreen ? 1 : 0);
			displayMode.ItemSelected += OnDisplayModeChanged;
		}

		// 2. Configurar Volúmenes (El 70% se escribe como 0.7)
		ConfigurarSliderVolumen(masterSlider, masterBus);
		ConfigurarSliderVolumen(musicSlider, musicBus);
		ConfigurarSliderVolumen(sfxSlider, sfxBus);

		// 3. Configurar CheckButton del teclado (Activo por defecto)
		if (typingSoundToggle != null)
		{
			typingSoundToggle.ButtonPressed = true;
			typingSoundToggle.Toggled += OnTypingSoundToggled;
		}

		// 4. Configurar Botones Principales
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
		
		GD.Print("Settings listos con volumen al 70%");
	}

	// Esta función prepara los sliders y hace la conversión matemática mágica a decibeles
	private void ConfigurarSliderVolumen(HSlider slider, int busIndex)
	{
		if (slider == null) return;

		slider.MinValue = 0.0001; // Súper bajito para evitar errores matemáticos
		slider.MaxValue = 1.0;    // 100%
		slider.Step = 0.01;

		// Lo ponemos al 70% como pediste
		slider.Value = 0.7;
		AudioServer.SetBusVolumeDb(busIndex, (float)Mathf.LinearToDb(0.7));

		// Cuando el jugador mueva el slider, cambiamos el volumen en tiempo real
		slider.ValueChanged += (value) => 
		{
			AudioServer.SetBusVolumeDb(busIndex, (float)Mathf.LinearToDb(value));
		};
	}

	private void OnTypingSoundToggled(bool activado)
	{
		// Ahorita solo imprime un mensaje, pero después aquí avisaremos a la nave que (no) suene
		GD.Print("Sonido al teclear activado: " + activado);
	}

	public void Open(bool pauseGame = true)
	{
		openedFromGame = pauseGame;
		if (pauseGame) GetTree().Paused = true;
		Visible = true;
		
		if (displayMode != null) displayMode.GrabFocus();
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
		else DisplayServer.WindowSetMode(DisplayServer.WindowMode.Fullscreen);
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
