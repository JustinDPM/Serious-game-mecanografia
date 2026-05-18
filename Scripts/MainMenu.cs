using Godot;
using System;
using Npgsql; 

public partial class MainMenu : Control
{
	private Button _playButton;
	private Button _profileButton;
	private Button _databaseButton;
	private Button _settingsButton;
	private Button _quitButton;

	private Label _nameMain;
	private Label _usernameMain;
	private TextureRect _avatarImage;
	private Label _pilotName;
	private Label _recordLabel;
	private Label _activeWordsLabel;
	private Label _gradeLabel;

	private Global _global;
	private SettingsMenu _settingsMenu;
	private Control _mainPanel; 

	private string connectionString = "Host=localhost;Username=postgres;Password=contrasena;Database=astrotype_db";

	public override void _Ready()
	{
		_global = GetNode<Global>("/root/Global"); 
		
		string rutaBotones = "MarginContainer/VBoxContainer/MainArea/LeftContainer/VBoxContainer/";

		_playButton = GetNode<Button>(rutaBotones + "PlayButton"); 
		_profileButton = GetNode<Button>(rutaBotones + "ProfileButton"); 
		_databaseButton = GetNode<Button>(rutaBotones + "DatabaseButton"); 
		_settingsButton = GetNode<Button>(rutaBotones + "SettingsButton"); 
		_quitButton = GetNode<Button>(rutaBotones + "QuitButton"); 

		string rutaTopBar = "MarginContainer/VBoxContainer/TopBar/UserProfile/";
		_nameMain = GetNode<Label>(rutaTopBar + "ProfileTexts/NameMain");
		_usernameMain = GetNode<Label>(rutaTopBar + "ProfileTexts/UsernameMain");
		_avatarImage = GetNode<TextureRect>(rutaTopBar + "AvatarImage");

		string rutaRight = "MarginContainer/VBoxContainer/MainArea/RightContainer/VBoxContainer/";
		_pilotName = GetNode<Label>(rutaRight + "StatsTop/PilotName");
		_recordLabel = GetNode<Label>(rutaRight + "StatsTop/Record");
		_activeWordsLabel = GetNode<Label>(rutaRight + "DictBottom/ActiveWords");
		_gradeLabel = GetNode<Label>(rutaRight + "DictBottom/Grade");

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

		ActualizarInterfazJugador();
	}

	private void ActualizarInterfazJugador()
	{
		_nameMain.Text = _global.Rol;
		_usernameMain.Text = _global.UsuarioActivo; 

		string primerNombre = _global.NombreCompleto.Split(' ')[0];
		_pilotName.Text = $"PILOTO: {primerNombre.ToUpper()}";

		string nombreGrado = _global.IdGrado switch
		{
			1 => "PRIMARIA BAJA",
			2 => "PRIMARIA ALTA",
			3 => "SECUNDARIA",
			4 => "PREPARATORIA",
			_ => "SIN ASIGNAR" 
		};
		_gradeLabel.Text = $"GRADO: {nombreGrado}";

		if (ResourceLoader.Exists(_global.RutaFotoPerfil))
		{
			_avatarImage.Texture = GD.Load<Texture2D>(_global.RutaFotoPerfil);
		}
		else
		{
			GD.Print("Foto no encontrada, cargando avatar por defecto.");
			_avatarImage.Texture = GD.Load<Texture2D>("res://assets/Perfiles/default.png"); 
		}

		CargarEstadisticasDB();
	}

	private void CargarEstadisticasDB()
	{
		int maxWpm = 0;
		int palabrasActivas = 0;

		try
		{
			using (var conn = new NpgsqlConnection(connectionString))
			{
				conn.Open();

				string queryRecord = "SELECT COALESCE(MAX(ppm_promedio), 0) FROM PARTIDA WHERE id_alumno = @id";
				using (var cmd1 = new NpgsqlCommand(queryRecord, conn))
				{
					cmd1.Parameters.AddWithValue("id", _global.IdUsuario);
					maxWpm = Convert.ToInt32(cmd1.ExecuteScalar());
				}

				string queryPalabras = "SELECT COUNT(*) FROM PALABRA";
				if (_global.Rol == "Alumno") 
				{
					queryPalabras += " WHERE id_grado <= @grado";
				}
				
				using (var cmd2 = new NpgsqlCommand(queryPalabras, conn))
				{
					if (_global.Rol == "Alumno")
					{
						cmd2.Parameters.AddWithValue("grado", _global.IdGrado);
					}
					palabrasActivas = Convert.ToInt32(cmd2.ExecuteScalar());
				}
			}

			_recordLabel.Text = $"RECORD: {maxWpm}WPM";
			_activeWordsLabel.Text = $"PALABRAS ACTIVAS: {palabrasActivas:N0}"; 
		}
		catch (Exception ex)
		{
			GD.PrintErr("Error al cargar estadísticas DB: " + ex.Message);
			_recordLabel.Text = "RECORD: -- WPM";
			_activeWordsLabel.Text = "PALABRAS ACTIVAS: --";
		}
	}

	private void ConfigurarAnimacionBoton(Button boton) //[cite: 2]
	{
		boton.MouseEntered += () =>  //[cite: 2]
		{
			boton.PivotOffset = boton.Size / 2;  //[cite: 2]
			Tween tween = CreateTween(); //[cite: 2]
			tween.TweenProperty(boton, "scale", new Vector2(1.05f, 1.05f), 0.1f).SetTrans(Tween.TransitionType.Sine); 
		};

		boton.MouseExited += () =>  //[cite: 2]
		{
			boton.PivotOffset = boton.Size / 2; //[cite: 2]
			Tween tween = CreateTween(); //[cite: 2]
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
