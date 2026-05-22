using Godot;
using System;

public partial class Level1 : Node2D
{
	private SettingsMenu settingsMenu;

	public override void _Ready()
	{
		settingsMenu = GetNode<SettingsMenu>("SettingsMenu");

		var turret = GetNode<Turret>("Ship/Turret");
		var global = GetNode<Global>("/root/Global");

		GetNode<AudioManager>("/root/AudioManager")
			.PlayGameMusic();

	}

	public override void _Input(InputEvent @event)
	{
		if (Input.IsActionJustPressed("ui_cancel")) 
		{
			if (settingsMenu.Visible)
			{
				settingsMenu.OnResumePressed();
			}
			else
			{
				settingsMenu.Open(true); 
			}
		}
	}
}
