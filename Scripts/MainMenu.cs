using Godot;
using System;

public partial class MainMenu : Control
{
	// 1. Declarar todos los botones
	private Button _playButton;
	private Button _profileButton;
	private Button _databaseButton;
	private Button _settingsButton;
	private Button _quitButton;

	// Referencias a otros scripts y paneles
	private Global _global;
	private SettingsMenu _settingsMenu;
	private Control _mainPanel; // Ahora este será tu MarginContainer

	public override void _Ready()
	{
		_global = GetNode<Global>("/root/Global");
		
		// La ruta base donde viven tus botones ahora:
		string rutaBotones = "MarginContainer/VBoxContainer/MainArea/LeftContainer/VBoxContainer/";

		// Reconectando los cables (Nodos)
		_playButton = GetNode<Button>(rutaBotones + "PlayButton");
		_profileButton = GetNode<Button>(rutaBotones + "ProfileButton");
		_databaseButton = GetNode<Button>(rutaBotones + "DatabaseButton");
		_settingsButton = GetNode<Button>(rutaBotones + "SettingsButton");
		_quitButton = GetNode<Button>(rutaBotones + "QuitButton");

		// El panel principal que ocultamos cuando se abren los Ajustes
		_mainPanel = GetNode<Control>("MarginContainer");

		// (NOTA: Asegúrate de que tu nodo SettingsMenu siga en el árbol de escenas, 
		// hasta abajo, fuera del MarginContainer)
		_settingsMenu = GetNode<SettingsMenu>("SettingsMenu");

		// Conectar los clics
		_playButton.Pressed += OnPlayButtonPressed;
		_settingsButton.Pressed += OnSettingsButtonPressed;
		_quitButton.Pressed += OnQuitButtonPressed;
		// (Pronto conectaremos Perfil y BD dependiendo del Rol del usuario)

		// ¡Activar la magia de las animaciones Hover!
		ConfigurarAnimacionBoton(_playButton);
		ConfigurarAnimacionBoton(_profileButton);
		ConfigurarAnimacionBoton(_databaseButton);
		ConfigurarAnimacionBoton(_settingsButton);
		ConfigurarAnimacionBoton(_quitButton);
	}

	// --- ANIMACIONES FLUIDAS CON TWEENS ---
	private void ConfigurarAnimacionBoton(Button boton)
{
	boton.MouseEntered += () => 
	{
		// Truco de ingeniero: Centramos el pivote dinámicamente según el tamaño actual del botón
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

	// --- ACCIONES DE LOS BOTONES ---
	private void OnPlayButtonPressed()
	{
		_global.CambiarEscena("res://Escenas/game.tscn");
	}

	private void OnSettingsButtonPressed()
	{
		_mainPanel.Visible = false; // Oculta la interfaz principal
		_settingsMenu.Open(false);   // Abre el menú flotante
	}

	private void OnQuitButtonPressed()
	{
		_global.LimpiarSesion();
		_global.CambiarEscena("res://Escenas/login.tscn");
	}
}
