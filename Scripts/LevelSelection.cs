using Godot;
using System;

public partial class LevelSelection : Control
{
	private Button _level1Btn;
	private Button _level2Btn;
	private Button _level3Btn;
	private Button _level4Btn;
	private Button _backBtn;

	private Global _global;

	public override void _Ready()
	{
		_global = GetNodeOrNull<Global>("/root/Global");

		// Rutas exactas a tus nodos según tu árbol
		string rutaCartas = "MarginContainer/VBoxContainer/HBoxContainer/";
		_level1Btn = GetNode<Button>(rutaCartas + "Level1Btn");
		_level2Btn = GetNode<Button>(rutaCartas + "Level2Btn");
		_level3Btn = GetNode<Button>(rutaCartas + "Level3Btn");
		_level4Btn = GetNode<Button>(rutaCartas + "Level4Btn");
		
		// Tu botón de regresar
		_backBtn = GetNode<Button>("MarginContainer/VBoxContainer/Button"); 

		// Conectamos las señales de clic a tus escenas
		_level1Btn.Pressed += () => CambiarNivel("res://Escenas/game.tscn");
		_level2Btn.Pressed += () => CambiarNivel("res://Escenas/level2.tscn");
		_level3Btn.Pressed += () => CambiarNivel("res://Escenas/level3.tscn");
		_backBtn.Pressed += OnBackBtnPressed;

		// Desactivamos el nivel 4 para que no se pueda clickear por ahora
		_level4Btn.Disabled = true;
		_level4Btn.Text = "Nivel 4\n(Bloqueado)";

		// Les damos la animación de hover
		Button[] botones = { _level1Btn, _level2Btn, _level3Btn, _backBtn };
		foreach (Button btn in botones)
		{
			ConfigurarAnimacionBoton(btn);
		}
	}

	private void CambiarNivel(string rutaEscena)
	{
		if (_global != null)
		{
			_global.CambiarEscena(rutaEscena);
		}
		else
		{
			GetTree().ChangeSceneToFile(rutaEscena);
		}
	}

	private void OnBackBtnPressed()
	{
		if (_global != null)
		{
			_global.CambiarEscena("res://Escenas/main_menu.tscn");
		}
		else
		{
			GetTree().ChangeSceneToFile("res://Escenas/main_menu.tscn");
		}
	}

	// Animación para que los botones crezcan al pasar el mouse
	private void ConfigurarAnimacionBoton(Button boton)
	{
		boton.MouseEntered += () =>
		{
			boton.PivotOffset = boton.Size / 2;
			Tween tween = CreateTween();
			tween.TweenProperty(boton, "scale", new Vector2(1.05f, 1.05f), 0.1f)
				 .SetTrans(Tween.TransitionType.Sine);
		};
		boton.MouseExited += () =>
		{
			boton.PivotOffset = boton.Size / 2;
			Tween tween = CreateTween();
			tween.TweenProperty(boton, "scale", new Vector2(1.0f, 1.0f), 0.1f)
				 .SetTrans(Tween.TransitionType.Sine);
		};
	}
}
