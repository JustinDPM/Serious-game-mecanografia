using Godot;
using System;
using System.Collections.Generic;

public partial class Login : Control
{
	private LineEdit _usernameInput;
	private LineEdit _passwordInput;
	private Label _errorMessage;
	private Button _enterButton;
	private Global _global;

	private readonly Dictionary<string, string> _mockUsers = new Dictionary<string, string>
	{
		{ "alumno", "1234" },
		{ "profe", "admin" },
		{ "cheque", "uv2026" } 
	};

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

		if (_mockUsers.ContainsKey(user) && _mockUsers[user] == pass)
		{
			_global.UsuarioActivo = user;
			
			// Redirige a la nueva escena del menú en inglés
			_global.CambiarEscena("res://Escenas/main_menu.tscn");
		}
		else
		{
			_errorMessage.Modulate = new Color(1, 0, 0);
			_errorMessage.Text = "Invalid credentials. Try again."; // Mensaje en inglés
		}
	}
}
