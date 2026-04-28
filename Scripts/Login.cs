using Godot;
using System;
using Npgsql; // ¡NUEVA LIBRERÍA! El traductor para que C# hable con PostgreSQL

public partial class Login : Control
{
	private LineEdit _usernameInput;
	private LineEdit _passwordInput;
	private Label _errorMessage;
	private Button _enterButton;
	private Global _global;

	// LA LLAVE DEL REINO: Tu cadena de conexión a la BD
	// OJO: Cambia 'tu_contraseña_aqui' por la contraseña de tu usuario postgres
	private string connectionString = "Host=localhost;Username=postgres;Password=040306;Database=astrotype_db";

	public override void _Ready()
	{
		_usernameInput = GetNode<LineEdit>("VBoxContainer/InputUser");
		_passwordInput = GetNode<LineEdit>("VBoxContainer/InputPassword");
		_errorMessage = GetNode<Label>("VBoxContainer/ErrorMessage");
		_enterButton = GetNode<Button>("VBoxContainer/EnterButton");
		
		_global = GetNode<Global>("/root/Global");

		_enterButton.Pressed += OnEnterButtonPressed;
		_errorMessage.Text = "";
	}

	private void OnEnterButtonPressed()
	{
		string user = _usernameInput.Text.Trim();
		string pass = _passwordInput.Text.Trim();

		_errorMessage.Text = ""; // Limpiamos errores anteriores

		// Validación básica antes de ir a la BD
		if (string.IsNullOrEmpty(user) || string.IsNullOrEmpty(pass))
		{
			MostrarError("Por favor, llena todos los campos.");
			return;
		}

		// ¡EL MOMENTO DE LA VERDAD! Vamos a la base de datos
		if (ValidarUsuarioBD(user, pass))
		{
			_global.UsuarioActivo = user;
			_global.CambiarEscena("res://Escenas/main_menu.tscn"); // Ojo con minúsculas/mayúsculas de tu escena
		}
		else
		{
			MostrarError("Invalid credentials. Try again.");
		}
	}

	// Función que se conecta a PostgreSQL y hace la consulta
	private bool ValidarUsuarioBD(string username, string password)
	{
		try
		{
			// Creamos y abrimos la conexión
			using (var connection = new NpgsqlConnection(connectionString))
			{
				connection.Open();

				// Tu consulta SQL. 
				// Usamos @u y @p (parámetros) en lugar de concatenar texto para EVITAR INYECCIONES SQL. ¡Buenas prácticas!
				string sql = "SELECT rol FROM USUARIO WHERE username = @u AND password_hash = @p AND activo = true";
				
				using (var command = new NpgsqlCommand(sql, connection))
				{
					// Le asignamos los valores a los parámetros
					command.Parameters.AddWithValue("u", username);
					command.Parameters.AddWithValue("p", password);

					// Ejecutamos la lectura
					using (var reader = command.ExecuteReader())
					{
						if (reader.Read()) // Si lee al menos un renglón, el usuario y contraseña son correctos
						{
							string rol = reader.GetString(0);
							GD.Print($"¡Ingreso exitoso en BD! Bienvenido {username}, Rol: {rol}");
							return true;
						}
					}
				}
			}
		}
		catch (Exception ex)
		{
			// Si la base de datos está apagada o la contraseña de Postgres está mal
			GD.PrintErr("Error de base de datos: " + ex.Message);
			MostrarError("Error de conexión al servidor.");
		}

		return false; // Si llega aquí, o falló la conexión o las credenciales no existen
	}

	private void MostrarError(string mensaje)
	{
		_errorMessage.Modulate = new Color(1, 0, 0); // Lo pintamos de rojo
		_errorMessage.Text = mensaje;
	}
}
