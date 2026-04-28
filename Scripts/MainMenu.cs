using Godot;
using System;

public partial class MainMenu : Control
{
	private Button _playButton;
	private Button _quitButton;
	private Global _global;

	public override void _Ready()
	{
		_global = GetNode<Global>("/root/Global");
		
		_playButton = GetNode<Button>("VBoxContainer/PlayButton");
		_quitButton = GetNode<Button>("VBoxContainer/QuitButton");

		_playButton.Pressed += OnPlayButtonPressed;
		_quitButton.Pressed += OnQuitButtonPressed;
	}

	private void OnPlayButtonPressed()
	{
		// Conectamos con la escena de tu compañero sin tocar su código
		_global.CambiarEscena("res://Escenas/game.tscn");
	}

	private void OnQuitButtonPressed()
	{
		_global.LimpiarSesion();
		
		// Regresa al login
		_global.CambiarEscena("res://Escenas/login.tscn");
	}
}
