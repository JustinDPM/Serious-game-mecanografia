using Godot;
using System;

public partial class SettingsMenu : Control
{
	private OptionButton displayMode;
	private bool openedFromGame = false;
	[Export] private Control mainMenuPanel;
	
	// Botones del menú
	private Button resumeButton;
	private Button exitButton;
	private Button btnVideo;
	private Button btnAudio;
	private Button btnGameplay;
	private Button btnAccessibility;
	
	// El contenedor de las pestañas
	private TabContainer optionsArea;
	private Global _global;

	public override void _Ready()
	{
		Visible = false;
		_global = GetNodeOrNull<Global>("/root/Global");

		// 1. Rutas de los nodos
		optionsArea = GetNodeOrNull<TabContainer>("CenterContainer/PanelContainer/HBoxPrincipal/OptionsArea");
		displayMode = GetNodeOrNull<OptionButton>("CenterContainer/PanelContainer/HBoxPrincipal/OptionsArea/TabVideo/OptionButton");
		
		resumeButton = GetNodeOrNull<Button>("CenterContainer/PanelContainer/HBoxPrincipal/SideBar/Return");
		exitButton = GetNodeOrNull<Button>("CenterContainer/PanelContainer/HBoxPrincipal/SideBar/Close");
		
		// Rutas de los botones de categorías
		btnVideo = GetNodeOrNull<Button>("CenterContainer/PanelContainer/HBoxPrincipal/SideBar/Video");
		btnAudio = GetNodeOrNull<Button>("CenterContainer/PanelContainer/HBoxPrincipal/SideBar/Audio");
		btnGameplay = GetNodeOrNull<Button>("CenterContainer/PanelContainer/HBoxPrincipal/SideBar/Gameplay");
		btnAccessibility = GetNodeOrNull<Button>("CenterContainer/PanelContainer/HBoxPrincipal/SideBar/Accessibility");

		// 2. Configurar video
		if (displayMode != null)
		{
			displayMode.Clear();
			displayMode.AddItem("Ventana", 0);
			displayMode.AddItem("Pantalla completa", 1);
			var mode = DisplayServer.WindowGetMode();
			displayMode.Select(mode == DisplayServer.WindowMode.Fullscreen ? 1 : 0);
			displayMode.ItemSelected += OnDisplayModeChanged;
		}

		// 3. Conectar señales de los botones
		if (resumeButton != null) 
		{
			resumeButton.Pressed += OnResumePressed;
			resumeButton.Text = "Continuar";
		}
		
		if (exitButton != null) exitButton.Pressed += OnExitPressed;

		// Conectar botones de pestañas (El TabContainer empieza en el índice 0)
		if (btnVideo != null) btnVideo.Pressed += () => ChangeTab(0);
		if (btnAudio != null) btnAudio.Pressed += () => ChangeTab(1);
		if (btnGameplay != null) btnGameplay.Pressed += () => ChangeTab(2);
		if (btnAccessibility != null) btnAccessibility.Pressed += () => ChangeTab(3);

		Button[] botones = { resumeButton, exitButton, btnVideo, btnAudio, btnGameplay, btnAccessibility };
		foreach (Button btn in botones)
		{
			if (btn != null) ConfigurarAnimacionBoton(btn);
		}
		
		GD.Print("Settings conectado y pestañas listas");
	}

	// Cambiar la pestaña visible en el TabContainer
	private void ChangeTab(int tabIndex)
	{
		if (optionsArea != null)
		{
			optionsArea.CurrentTab = tabIndex;
			GD.Print("Cambiando a pestaña: " + tabIndex);
		}
	}

	public void Open(bool pauseGame = true)
	{
		openedFromGame = pauseGame;
		if (pauseGame) GetTree().Paused = true;
		Visible = true;
		
		// Asegurar que siempre abra en la pestaña de Video (0) por defecto
		if (optionsArea != null) optionsArea.CurrentTab = 0; 
		
		if (displayMode != null) displayMode.GrabFocus();
	}

	public void OnResumePressed()
	{
		SceneTree arbol = GetTree(); // Guardamos el árbol primero de forma segura
		
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
			// Si estamos en partida, quitamos la pausa y volvemos al menú
			if (arbol != null) arbol.Paused = false;
			
			if (_global != null)
			{
				_global.CambiarEscena("res://Escenas/main_menu.tscn");
			}
			else
			{
				GD.PrintErr("🚨 Error: Autoload '_global' no encontrado al intentar salir.");
			}
		}
		else
		{
			// Si ya estamos en el menú principal, cerramos el juego
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
