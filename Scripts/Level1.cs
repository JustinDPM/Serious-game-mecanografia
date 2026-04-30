using Godot;
using System;

public partial class Level1 : Node2D
{
    private SettingsMenu settingsMenu;

    public override void _Ready()
    {
        settingsMenu = GetNode<SettingsMenu>("SettingsMenu");
    }

    public override void _Input(InputEvent @event)
    {
        if (Input.IsActionJustPressed("ui_cancel")) // ESC
        {
            if (settingsMenu.Visible)
            {
                settingsMenu.OnResumePressed();
            }
            else
            {
                settingsMenu.Open(true); // 🎮 pausa juego
            }
        }
    }
}