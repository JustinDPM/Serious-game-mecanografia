using Godot;
using System;
using Npgsql; // No olvides importar Npgsql para sacar el Récord y Palabras de la BD

public partial class MainMenu : Control
{
	private Button _playButton;
	private Button _profileButton;
	private Button _databaseButton;
	private Button _settingsButton;
	private Button _quitButton;

	// --- NUEVOS NODOS DE LA INTERFAZ ---
	private Label _nameMain;
	private Label _usernameMain;
	private TextureRect _avatarImage;
	private Label _pilotName;
	private Label _recordLabel;
	private Label _activeWordsLabel;
	private Label _gradeLabel;
	// -----------------------------------

	private Global _global;
	private SettingsMenu _settingsMenu;
	private Control _mainPanel; 

	// Tu cadena de conexión para las estadísticas
	private string connectionString = "Host=localhost;Username=postgres;Password=contrasena;Database=astrotype_db";

	public override void _Ready()
	{
		_global = GetNode<Global>("/root/Global"); //
		
		string rutaBotones = "MarginContainer/VBoxContainer/MainArea/LeftContainer/VBoxContainer/";

		_playButton = GetNode<Button>(rutaBotones + "PlayButton"); //[cite: 2]
		_profileButton = GetNode<Button>(rutaBotones + "ProfileButton"); //[cite: 2]
		_databaseButton = GetNode<Button>(rutaBotones + "DatabaseButton"); //[cite: 2]
		_settingsButton = GetNode<Button>(rutaBotones + "SettingsButton"); //[cite: 2]
		_quitButton = GetNode<Button>(rutaBotones + "QuitButton"); //[cite: 2]

		// 1. CAPTURAR LOS TEXTOS DEL PERFIL BASADO EN TU ÁRBOL DE NODOS
		string rutaTopBar = "MarginContainer/VBoxContainer/TopBar/UserProfile/";
		_nameMain = GetNode<Label>(rutaTopBar + "ProfileTexts/NameMain");
		_usernameMain = GetNode<Label>(rutaTopBar + "ProfileTexts/UsernameMain");
		_avatarImage = GetNode<TextureRect>(rutaTopBar + "AvatarImage");

		string rutaRight = "MarginContainer/VBoxContainer/MainArea/RightContainer/VBoxContainer/";
		_pilotName = GetNode<Label>(rutaRight + "StatsTop/PilotName");
		_recordLabel = GetNode<Label>(rutaRight + "StatsTop/Record");
		_activeWordsLabel = GetNode<Label>(rutaRight + "DictBottom/ActiveWords");
		_gradeLabel = GetNode<Label>(rutaRight + "DictBottom/Grade");

		_mainPanel = GetNode<Control>("MarginContainer"); //[cite: 2]
		_settingsMenu = GetNode<SettingsMenu>("SettingsMenu"); //[cite: 2]

		_playButton.Pressed += OnPlayButtonPressed; //[cite: 2]
		_settingsButton.Pressed += OnSettingsButtonPressed; //[cite: 2]
		_quitButton.Pressed += OnQuitButtonPressed; //[cite: 2]

		ConfigurarAnimacionBoton(_playButton); //[cite: 2]
		ConfigurarAnimacionBoton(_profileButton); //[cite: 2]
		ConfigurarAnimacionBoton(_databaseButton); //[cite: 2]
		ConfigurarAnimacionBoton(_settingsButton); //[cite: 2]
		ConfigurarAnimacionBoton(_quitButton); //[cite: 2]

		// 2. ACTUALIZAR TODA LA PANTALLA CON LOS DATOS DEL JUGADOR
		ActualizarInterfazJugador();
	}

	private void ActualizarInterfazJugador()
	{
		// Textos del perfil superior
		_nameMain.Text = _global.Rol; // Puedes poner Rol o Nombre
		_usernameMain.Text = _global.UsuarioActivo; // La matrícula S24016724

		// Nombre del Piloto (Cortamos el nombre completo para que solo salga el primer nombre)
		string primerNombre = _global.NombreCompleto.Split(' ')[0];
		_pilotName.Text = $"PILOTO: {primerNombre.ToUpper()}";

		// Mapear el ID del grado a un texto visible
		string nombreGrado = _global.IdGrado switch
		{
			1 => "PRIMARIA BAJA",
			2 => "PRIMARIA ALTA",
			3 => "SECUNDARIA",
			4 => "PREPARATORIA",
			_ => "SIN ASIGNAR" // Para los profes/admin
		};
		_gradeLabel.Text = $"GRADO: {nombreGrado}";

		// --- AQUÍ ESTÁ EL CÓDIGO NUEVO DE LA FOTO ---
		// Cargar la foto de perfil dinámicamente con un salvavidas
		if (ResourceLoader.Exists(_global.RutaFotoPerfil))
		{
			// Si la foto existe en los archivos de Godot, la carga
			_avatarImage.Texture = GD.Load<Texture2D>(_global.RutaFotoPerfil);
		}
		else
		{
			// Si la ruta está mal o el archivo se borró, carga una genérica
			GD.Print("Foto no encontrada, cargando avatar por defecto.");
			_avatarImage.Texture = GD.Load<Texture2D>("res://assets/Perfiles/default.png"); 
		}
		// --------------------------------------------

		// 3. CARGAR ESTADÍSTICAS REALES DESDE POSTGRESQL
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

				// Consulta A: Obtener el Récord Histórico de WPM del alumno actual[cite: 1]
				string queryRecord = "SELECT COALESCE(MAX(ppm_promedio), 0) FROM PARTIDA WHERE id_alumno = @id";
				using (var cmd1 = new NpgsqlCommand(queryRecord, conn))
				{
					cmd1.Parameters.AddWithValue("id", _global.IdUsuario);
					maxWpm = Convert.ToInt32(cmd1.ExecuteScalar());
				}

				// Consulta B: Obtener la cantidad de palabras que el alumno va a enfrentar (<= a su grado)[cite: 1]
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

			// Actualizamos las etiquetas con el formato exacto de tu diseño
			_recordLabel.Text = $"RECORD: {maxWpm}WPM";
			// El ":N0" le pone la comita de los miles (ej. 1,200)
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
			tween.TweenProperty(boton, "scale", new Vector2(1.05f, 1.05f), 0.1f).SetTrans(Tween.TransitionType.Sine); //[cite: 2]
		};

		boton.MouseExited += () =>  //[cite: 2]
		{
			boton.PivotOffset = boton.Size / 2; //[cite: 2]
			Tween tween = CreateTween(); //[cite: 2]
			tween.TweenProperty(boton, "scale", new Vector2(1.0f, 1.0f), 0.1f).SetTrans(Tween.TransitionType.Sine); //[cite: 2]
		};
	}

	private void OnPlayButtonPressed() //[cite: 2]
	{
		_global.CambiarEscena("res://Escenas/game.tscn"); //[cite: 1, 2]
	}

	private void OnSettingsButtonPressed() //[cite: 2]
	{
		_mainPanel.Visible = false;  //[cite: 2]
		_settingsMenu.Open(false);   //[cite: 2]
	}

	private void OnQuitButtonPressed() //[cite: 2]
	{
		_global.LimpiarSesion(); //[cite: 1, 2]
		_global.CambiarEscena("res://Escenas/login.tscn"); //[cite: 1, 2]
	}
}
