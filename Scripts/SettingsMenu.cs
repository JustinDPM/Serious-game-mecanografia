using Godot;
using System;

public partial class SettingsMenu : Control
{
    private OptionButton displayMode;
    private bool openedFromGame = false;
    [Export] private Control mainMenuPanel;

    public override void _Ready()
    {
        Visible = false;

        displayMode = GetNode<OptionButton>("PanelContainer/VBoxContainer/OptionButton");

        // 🔥 Asegurar opciones
        displayMode.Clear();
        displayMode.AddItem("Ventana", 0);
        displayMode.AddItem("Pantalla completa", 1);

        // 🔥 Selección inicial según modo actual
        var mode = DisplayServer.WindowGetMode();
        displayMode.Select(mode == DisplayServer.WindowMode.Fullscreen ? 1 : 0);

        // 🔥 Conectar señal
        displayMode.ItemSelected += OnDisplayModeChanged;

        GD.Print("Settings listo ✅");
    }

    // 🔥 ABRIR MENÚ
    public void Open(bool pauseGame = true)
    {
        openedFromGame = pauseGame;

        if (pauseGame)
            GetTree().Paused = true;

        Visible = true;

        displayMode.GrabFocus(); // 👈 importante
    }

    // 🔥 BOTÓN REANUDAR / VOLVER
    public void OnResumePressed()
    {
        if (openedFromGame)
        {
            // 🎮 volver al juego
            GetTree().Paused = false;
        }
        else
        {
            // 🏠 volver al menú
            if (mainMenuPanel != null)
                mainMenuPanel.Visible = true;
        }

        Visible = false;
    }

    // 🔥 BOTÓN SALIR
    public void OnExitPressed()
    {
        GetTree().Quit();
    }

    // 🔥 CAMBIO DE MODO DE PANTALLA
    private void OnDisplayModeChanged(long index)
    {
        GD.Print("Cambio detectado: " + index);

        if (index == 0)
        {
            DisplayServer.WindowSetMode(DisplayServer.WindowMode.Windowed);
        }
        else
        {
            DisplayServer.WindowSetMode(DisplayServer.WindowMode.Fullscreen);
        }
    }
}