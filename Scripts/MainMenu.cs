using Godot;

public partial class MainMenu : Control
{
	// Botones de la izquierda
	private Button _playButton;
	private Button _profileButton;   // Tu nuevo botón (Estadísticas)
	private Button _databaseButton;  // (Cómo jugar)
	private Button _settingsButton;
	private Button _quitButton;

	// Textos de la derecha
	private Label _pilotName;
	private Label _recordLabel;

	// La nave
	private TextureRect _shipImage;

	private Global _global;
	private SettingsMenu _settingsMenu;
	private Control _mainPanel;

	public override void _Ready()
	{
		_global = GetNodeOrNull<Global>("/root/Global");

		// 1. Buscamos los botones de la izquierda
		string rutaBotones = "MarginContainer/VBoxContainer/MainArea/LeftContainer/VBoxContainer/";
		_playButton = GetNodeOrNull<Button>(rutaBotones + "PlayButton");
		_profileButton = GetNodeOrNull<Button>(rutaBotones + "ProfileButton");
		_databaseButton = GetNodeOrNull<Button>(rutaBotones + "DatabaseButton");
		_settingsButton = GetNodeOrNull<Button>(rutaBotones + "SettingsButton");
		_quitButton = GetNodeOrNull<Button>(rutaBotones + "QuitButton");

		// 2. Buscamos los elementos de la derecha
		string rutaRight = "MarginContainer/VBoxContainer/MainArea/RightContainer/VBoxContainer/";
		_pilotName = GetNodeOrNull<Label>(rutaRight + "StatsTop/PilotName");
		_recordLabel = GetNodeOrNull<Label>(rutaRight + "StatsTop/Record");
		_shipImage = GetNodeOrNull<TextureRect>(rutaRight + "ShipContainer/TextureRect");

		_mainPanel = GetNodeOrNull<Control>("MarginContainer");
		_settingsMenu = GetNodeOrNull<SettingsMenu>("SettingsMenu");

		// 3. Conectamos los botones
		if (_playButton != null) _playButton.Pressed += OnPlayButtonPressed;
		if (_profileButton != null) _profileButton.Pressed += OnStatsPressed;
		if (_databaseButton != null) _databaseButton.Pressed += OnDatabaseButtonPressed;
		if (_settingsButton != null) _settingsButton.Pressed += OnSettingsButtonPressed;
		if (_quitButton != null) _quitButton.Pressed += OnQuitButtonPressed;

		Button[] botones = { _playButton, _profileButton, _databaseButton, _settingsButton, _quitButton };
		foreach (Button btn in botones)
		{
			if (btn != null) ConfigurarAnimacionBoton(btn);
		}

		// 4. Arrancamos música y actualizamos textos
		GetNodeOrNull<AudioManager>("/root/AudioManager")?.Call("PlayMenuMusic");
		ActualizarInterfaz();

		// 🔥 5. INICIAMOS LA ANIMACIÓN DE LA NAVE 🔥
		if (_shipImage != null)
		{
			AnimacionNaveLevitando();
		}
	}

	private void ActualizarInterfaz()
	{
		if (_global == null) return;

		string primerNombre = _global.NombreCompleto.Split(' ')[0];

		if (_pilotName != null) 
			_pilotName.Text = $"PILOTO: {primerNombre.ToUpper()}";

		if (_recordLabel != null) 
			_recordLabel.Text = $"RECORD: {_global.LastWPM:0} WPM";
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

	// 🔥 LA MAGIA DE LA LEVITACIÓN 🔥
	private async void AnimacionNaveLevitando()
	{
		// 1. Truco Pro: Esperamos un frame a que el motor calcule el layout real
		// de tus contenedores. Así evitamos que tome el centro como 0.
		await ToSignal(GetTree(), "process_frame");

		// Verificamos que la nave siga existiendo después de la espera por seguridad
		if (!IsInstanceValid(_shipImage) || !IsInsideTree()) return;

		// Ahora sí, guardamos la posición real y milimétrica del centro del editor
		float centroY = _shipImage.Position.Y;
		
		// SetLoops() hace que la animación se repita infinitamente
		Tween tween = CreateTween().SetLoops(); 
		
		// 2. Sube 12 pixeles suavemente desde el centro (tarda 1.5 segundos)
		tween.TweenProperty(_shipImage, "position:y", centroY - 20f, 1.5f)
			 .SetTrans(Tween.TransitionType.Sine)
			 .SetEase(Tween.EaseType.InOut);
			 
		// 3. Baja cruzando el centro hasta 12 pixeles abajo (recorrido largo, tarda 3.0 segundos)
		tween.TweenProperty(_shipImage, "position:y", centroY + 20f, 3.0f)
			 .SetTrans(Tween.TransitionType.Sine)
			 .SetEase(Tween.EaseType.InOut);

		// 4. Sube de regreso desde abajo para cerrar el ciclo en el centro exacto (tarda 1.5 segundos)
		tween.TweenProperty(_shipImage, "position:y", centroY, 1.5f)
			 .SetTrans(Tween.TransitionType.Sine)
			 .SetEase(Tween.EaseType.InOut);
	}

	private void OnPlayButtonPressed()
	{
		_global?.CambiarEscena("res://Escenas/level_selection.tscn");
	}

	private void OnStatsPressed()
	{
		_global?.CambiarEscena("res://Escenas/stats_screen.tscn");
	}

	private void OnDatabaseButtonPressed() 
	{
		_global?.CambiarEscena("res://Escenas/tutorial.tscn");
	}

	private void OnSettingsButtonPressed()
	{
		if (_mainPanel != null) _mainPanel.Visible = false;
		if (_settingsMenu != null) _settingsMenu.Open(false);
	}

	private void OnQuitButtonPressed()
	{
		GetTree().Quit();
	}
}
