using Godot;
using System;

public partial class MainMenu : Control
{

	private Button _playButton;
	private Button _profileButton;
	private Button _databaseButton;
	private Button _settingsButton;
	private Button _quitButton;

	private Global _global;
	private SettingsMenu _settingsMenu;
	private Control _mainPanel; 

	public override void _Ready()
	{
		_global = GetNode<Global>("/root/Global");
		
		string rutaBotones = "MarginContainer/VBoxContainer/MainArea/LeftContainer/VBoxContainer/";

		_playButton = GetNode<Button>(rutaBotones + "PlayButton");
		_profileButton = GetNode<Button>(rutaBotones + "ProfileButton");
		_databaseButton = GetNode<Button>(rutaBotones + "DatabaseButton");
		_settingsButton = GetNode<Button>(rutaBotones + "SettingsButton");
		_quitButton = GetNode<Button>(rutaBotones + "QuitButton");

		_mainPanel = GetNode<Control>("MarginContainer");

		_settingsMenu = GetNode<SettingsMenu>("SettingsMenu");

		_playButton.Pressed += OnPlayButtonPressed;
		_settingsButton.Pressed += OnSettingsButtonPressed;
		_quitButton.Pressed += OnQuitButtonPressed;

		ConfigurarAnimacionBoton(_playButton);
		ConfigurarAnimacionBoton(_profileButton);
		ConfigurarAnimacionBoton(_databaseButton);
		ConfigurarAnimacionBoton(_settingsButton);
		ConfigurarAnimacionBoton(_quitButton);
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

	private void OnPlayButtonPressed()
	{
		_global.CambiarEscena("res://Escenas/game.tscn");
	}

	private void OnSettingsButtonPressed()
	{
		_mainPanel.Visible = false; 
		_settingsMenu.Open(false);   
	}

	private void OnQuitButtonPressed()
	{
		_global.LimpiarSesion();
		_global.CambiarEscena("res://Escenas/login.tscn");
	}
}
