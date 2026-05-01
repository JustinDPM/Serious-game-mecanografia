using Godot;
using System;
using Npgsql; 

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
		_usernameInput = GetNode<LineEdit>("PanelContainer/VBoxContainer/InputUser");
		_passwordInput = GetNode<LineEdit>("PanelContainer/VBoxContainer/InputPassword");
		_errorMessage = GetNode<Label>("PanelContainer/VBoxContainer/ErrorMessage");
		_enterButton = GetNode<Button>("PanelContainer/VBoxContainer/EnterButton");
		
		_global = GetNode<Global>("/root/Global");

		_enterButton.Pressed += OnEnterButtonPressed;
		_errorMessage.Text = "";
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

	private bool ValidarUsuarioBD(string username, string password)
	{
		try
		{
			using (var connection = new NpgsqlConnection(connectionString))
			{
				connection.Open();

				string sql = "SELECT rol FROM USUARIO WHERE username = @u AND password_hash = @p AND activo = true";
				
				using (var command = new NpgsqlCommand(sql, connection))
				{
					command.Parameters.AddWithValue("u", username);
					command.Parameters.AddWithValue("p", password);

					using (var reader = command.ExecuteReader())
					{
						if (reader.Read())
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
			GD.PrintErr("Error de base de datos: " + ex.Message);
			MostrarError("Error de conexión al servidor.");
		}

		return false;
	}

	private void MostrarError(string mensaje)
	{
		_errorMessage.Modulate = new Color(1, 0, 0);
		_errorMessage.Text = mensaje;
	}
}
