using Godot;
using System;

public partial class Level1 : Node2D
{
	private SettingsMenu settingsMenu;
	private Turret turret;
	private AudioManager audioManager;

	public override void _Ready()
	{
		settingsMenu = GetNode<SettingsMenu>("SettingsMenu");
		turret = GetNode<Turret>("Ship/Turret");
		audioManager = GetNode<AudioManager>("/root/AudioManager");

		// 🔥 GUARDAMOS LA RUTA EXACTA DEL NIVEL ACTUAL EN GLOBAL 🔥
		GetNode<Global>("/root/Global").NivelActual = SceneFilePath;

		audioManager.PlayGameMusic();

		turret.OnComboStarted += audioManager.StartComboMusic;
		turret.OnComboEnded += audioManager.StopComboMusic;
	}

	public override void _ExitTree()
	{
		if (turret != null && audioManager != null)
		{
			turret.OnComboStarted -= audioManager.StartComboMusic;
			turret.OnComboEnded -= audioManager.StopComboMusic;
		}
	}

	public override void _Input(InputEvent @event)
	{
		if (Input.IsActionJustPressed("ui_cancel"))
		{
			if (settingsMenu.Visible)
				settingsMenu.OnResumePressed();
			else
				settingsMenu.Open(true);
		}
	}
}
