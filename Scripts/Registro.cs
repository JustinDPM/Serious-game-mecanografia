using Godot;
using System;
using Npgsql; 

public partial class Registro : Control
{
	private LineEdit _nameInput;      
	private LineEdit _usernameInput;  
	private LineEdit _passwordInput;
	private OptionButton _gradeInput; // NUEVO: El selector de grado
	private Label _messageLabel;      
	private Button _registerButton;
	private Button _backButton; 
	
	private Global _global;
	private string connectionString = "Host=localhost;Username=postgres;Password=040306;Database=astrotype_db";

	public override void _Ready()
	{
		_nameInput = GetNode<LineEdit>("PanelContainer/VBoxContainer/InputName"); 
		_usernameInput = GetNode<LineEdit>("PanelContainer/VBoxContainer/InputUser");
		_passwordInput = GetNode<LineEdit>("PanelContainer/VBoxContainer/InputPassword");
		
		// Conectamos el nuevo selector
		_gradeInput = GetNode<OptionButton>("PanelContainer/VBoxContainer/InputGrade"); 
		
		_messageLabel = GetNode<Label>("PanelContainer/VBoxContainer/ErrorMessage");
		_registerButton = GetNode<Button>("PanelContainer/VBoxContainer/RegisterButton");
		_backButton = GetNodeOrNull<Button>("BtnExit"); 
		
		_global = GetNode<Global>("/root/Global");

		_registerButton.Pressed += OnRegisterButtonPressed;
		if (_backButton != null) _backButton.Pressed += OnBackButtonPressed;
		
		_messageLabel.Text = "";

		// NUEVO: Llenamos el OptionButton con los Grados y sus IDs de la BD
		ConfigurarSelectorGrados();
	}

	private void ConfigurarSelectorGrados()
	{
		_gradeInput.Clear(); // Limpiamos por si había algo
		// El primer valor es el texto que ve el usuario, el segundo es el ID que va a PostgreSQL
		_gradeInput.AddItem("Primaria Baja", 1);
		_gradeInput.AddItem("Primaria Alta", 2);
		_gradeInput.AddItem("Secundaria", 3);
		_gradeInput.AddItem("Preparatoria", 4);
	}

	private void OnRegisterButtonPressed()
	{
		string name = _nameInput.Text.Trim();
		string user = _usernameInput.Text.Trim();
		string pass = _passwordInput.Text.Trim();
		
		// Obtenemos el ID exacto (1, 2, 3 o 4) que seleccionó el jugador
		int idGradoSeleccionado = _gradeInput.GetSelectedId();

		if (string.IsNullOrEmpty(name) || string.IsNullOrEmpty(user) || string.IsNullOrEmpty(pass))
		{
			MostrarMensaje("Por favor, llena todos los campos.", true);
			return;
		}

		try
		{
			using (var conn = new NpgsqlConnection(connectionString))
			{
				conn.Open();

				string checkQuery = "SELECT COUNT(*) FROM USUARIO WHERE username = @user";
				using (var checkCmd = new NpgsqlCommand(checkQuery, conn))
				{
					checkCmd.Parameters.AddWithValue("user", user);
					long count = (long)checkCmd.ExecuteScalar();

					if (count > 0)
					{
						MostrarMensaje("Esa matrícula ya está registrada.", true);
						return; 
					}
				}

				// NUEVO: Ahora el INSERT usa la variable @grado en lugar de un 1 fijo
				string insertQuery = @"INSERT INTO USUARIO (username, nombre_completo, password_hash, ruta_foto_perfil, rol, activo, id_grado) 
									   VALUES (@user, @name, @pass, 'res://assets/Perfiles/default.png', 'Alumno', TRUE, @grado)";
				
				using (var insertCmd = new NpgsqlCommand(insertQuery, conn))
				{
					insertCmd.Parameters.AddWithValue("user", user);
					insertCmd.Parameters.AddWithValue("name", name);
					insertCmd.Parameters.AddWithValue("pass", pass);
					insertCmd.Parameters.AddWithValue("grado", idGradoSeleccionado); // Pasamos el grado seleccionado
					
					insertCmd.ExecuteNonQuery();
				}

				MostrarMensaje("¡Cuenta creada con éxito! Volviendo...", false);
				
				GetTree().CreateTimer(1.5f).Timeout += OnBackButtonPressed;
			}
		}
		catch (Exception ex)
		{
			MostrarMensaje("Error al conectar con el servidor.", true);
			GD.PrintErr("Error DB: " + ex.Message);
		}
	}

	private void MostrarMensaje(string mensaje, bool esError)
	{
		_messageLabel.Modulate = esError ? new Color(1, 0, 0) : new Color(0, 1, 0); 
		_messageLabel.Text = mensaje;
	}
	
	private void OnBackButtonPressed()
	{
		_global.CambiarEscena("res://Escenas/login.tscn");
	}
}
