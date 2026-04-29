using Godot;
using System;

public partial class MainMenu : Control
{
	private Button _playButton;
	private Button _quitButton;
    private Button _settingsButton;
    private Global _global;
    private SettingsMenu settingsMenu;
    private Control mainPanel; // referencia al PanelContainer

    public override void _Ready()
	{
		_global = GetNode<Global>("/root/Global");
		
		_playButton = GetNode<Button>("PanelContainer/VBoxContainer/PlayButton");
		_quitButton = GetNode<Button>("PanelContainer/VBoxContainer/QuitButton");
		_settingsButton = GetNode<Button>("PanelContainer/VBoxContainer/SettingsButton");
        settingsMenu = GetNode<SettingsMenu>("SettingsMenu");
        mainPanel = GetNode<Control>("PanelContainer");


        _playButton.Pressed += OnPlayButtonPressed;
		_quitButton.Pressed += OnQuitButtonPressed;
		_settingsButton.Pressed += OnSettingsButtonPressed;
	}

	private void OnPlayButtonPressed()
	{
		_global.CambiarEscena("res://Escenas/game.tscn");
	}

	private void OnQuitButtonPressed()
	{
		_global.LimpiarSesion();
		
		_global.CambiarEscena("res://Escenas/login.tscn");
	}

	private void OnSettingsButtonPressed()
	{
        mainPanel.Visible = false; // 👈 oculta menú
        settingsMenu.Open(false);	
	}
}
