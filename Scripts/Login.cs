using Godot;
using System;
using Npgsql; 

public partial class Login : Control
{
	private LineEdit _usernameInput;
	private LineEdit _passwordInput;
	private Label _errorMessage;
	private Button _enterButton;
	private Button _exitButton; 
	private Button _registerLinkButton; // NUEVO: El botón para ir a registro
	private Global _global;

	// Tu cadena de conexión original
	private string connectionString = "Host=localhost;Username=postgres;Password=040306;Database=astrotype_db";

	public override void _Ready()
	{
		_usernameInput = GetNode<LineEdit>("PanelContainer/VBoxContainer/InputUser");
		_passwordInput = GetNode<LineEdit>("PanelContainer/VBoxContainer/InputPassword");
		_errorMessage = GetNode<Label>("PanelContainer/VBoxContainer/ErrorMessage");
		_enterButton = GetNode<Button>("PanelContainer/VBoxContainer/EnterButton");
		
		// Asegúrate de que el nombre del nodo aquí sea el mismo que le pusiste en Godot
		_registerLinkButton = GetNodeOrNull<Button>("PanelContainer/VBoxContainer/RegisterButton"); 
		
		_exitButton = GetNodeOrNull<Button>("BtnExit"); 
		
		_global = GetNode<Global>("/root/Global");

		_enterButton.Pressed += OnEnterButtonPressed;
		_errorMessage.Text = "";

		if (_exitButton != null)
		{
			_exitButton.Pressed += OnExitPressed;
		}

		// NUEVO: Conectamos el botón de registro
		if (_registerLinkButton != null)
		{
			_registerLinkButton.Pressed += OnRegisterLinkPressed;
		}
	}

	private void OnEnterButtonPressed()
	{
		string user = _usernameInput.Text.Trim();
		string pass = _passwordInput.Text.Trim();

		_errorMessage.Text = ""; 

		if (string.IsNullOrEmpty(user) || string.IsNullOrEmpty(pass))
		{
			MostrarError("Por favor, llena todos los campos.");
			return;
		}

		// Conexión real a la Base de Datos
		try
		{
			using (var conn = new NpgsqlConnection(connectionString))
			{
				conn.Open();
				// Hacemos el SELECT para traer todos los datos útiles del usuario
				string query = @"SELECT id_usuario, nombre_completo, ruta_foto_perfil, rol, id_grado 
								 FROM USUARIO 
								 WHERE username = @user AND password_hash = @pass AND activo = TRUE";
				
				using (var cmd = new NpgsqlCommand(query, conn))
				{
					cmd.Parameters.AddWithValue("user", user);
					cmd.Parameters.AddWithValue("pass", pass);

					using (var reader = cmd.ExecuteReader())
					{
						if (reader.Read())
						{
							// ¡Credenciales correctas! Guardamos los datos en Global
							_global.UsuarioActivo = user;
							_global.IdUsuario = reader.GetInt32(0);
							
							_global.NombreCompleto = reader.IsDBNull(1) ? user : reader.GetString(1);
							_global.RutaFotoPerfil = reader.IsDBNull(2) ? "res://assets/Perfiles/default.png" : reader.GetString(2);
							_global.Rol = reader.GetString(3);
							_global.IdGrado = reader.IsDBNull(4) ? 0 : reader.GetInt32(4);

							GD.Print($"¡Login Exitoso! Bienvenido {_global.NombreCompleto} (Rol: {_global.Rol})");
							
							// Cambiamos a la escena del menú
							_global.CambiarEscena("res://Escenas/main_menu.tscn");
						}
						else
						{
							MostrarError("Usuario o contraseña incorrectos.");
						}
					}
				}
			}
		}
		catch (Exception ex)
		{
			MostrarError("Error al conectar con el servidor.");
			GD.PrintErr("Detalle del error de BD: " + ex.Message);
		}
	}

	// NUEVO: Función que cambia a la pantalla de registro
	private void OnRegisterLinkPressed()
	{
		_global.CambiarEscena("res://Escenas/registro.tscn");
	}

	private void MostrarError(string mensaje)
	{
		_errorMessage.Modulate = new Color(1, 0, 0);
		_errorMessage.Text = mensaje;
	}
	
	private void OnExitPressed()
	{
		GetTree().Quit();
	}
}
