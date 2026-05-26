using Godot;
using System;

public partial class LevelSelection : Control
{
	private Button _level1Btn;
	private Button _level2Btn;
	private Button _level3Btn;
	private Button _level4Btn;
	private Button _bottomBtn; // Tu botón de abajo que ahora se llama PlayButton

	private Global _global;

	public override void _Ready()
	{
		_global = GetNodeOrNull<Global>("/root/Global");

		string rutaCartas = "MarginContainer/VBoxContainer/HBoxContainer/";
		_level1Btn = GetNode<Button>(rutaCartas + "Level1Btn");
		_level2Btn = GetNode<Button>(rutaCartas + "Level2Btn");
		_level3Btn = GetNode<Button>(rutaCartas + "Level3Btn");
		_level4Btn = GetNode<Button>(rutaCartas + "Level4Btn");
		
		_bottomBtn = GetNode<Button>("MarginContainer/VBoxContainer/PlayButton"); 

		// Conectamos las señales de clic solo a los primeros 3
		_level1Btn.Pressed += () => CambiarNivel("res://Escenas/game.tscn");
		_level2Btn.Pressed += () => CambiarNivel("res://Escenas/level2.tscn");
		_level3Btn.Pressed += () => CambiarNivel("res://Escenas/level3.tscn");
		
		_bottomBtn.Pressed += OnBackBtnPressed;

		// 🔥 CAMBIO AQUÍ: Dejamos el Custom habilitado para que funcione la animación,
		// pero como no tiene un evento .Pressed asignado, no te llevará a ningún lado al darle clic.
		_level4Btn.Disabled = false;

		TextureRect planeta1 = _level1Btn.GetNodeOrNull<TextureRect>("MarginContainer/VBoxContainer/TextureRect");
		TextureRect planeta2 = _level2Btn.GetNodeOrNull<TextureRect>("MarginContainer/VBoxContainer/TextureRect");
		TextureRect planeta3 = _level3Btn.GetNodeOrNull<TextureRect>("MarginContainer/VBoxContainer/TextureRect");
		TextureRect planeta4 = _level4Btn.GetNodeOrNull<TextureRect>("MarginContainer/VBoxContainer/TextureRect");

		ConfigurarAnimacionBoton(_level1Btn, planeta1);
		ConfigurarAnimacionBoton(_level2Btn, planeta2);
		ConfigurarAnimacionBoton(_level3Btn, planeta3);
		ConfigurarAnimacionBoton(_level4Btn, planeta4);
		
		ConfigurarAnimacionBoton(_bottomBtn, null);
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

	// Animación actualizada para que CREZCA EL PLANETA además del botón
	private void ConfigurarAnimacionBoton(Button boton, TextureRect planeta)
	{
		boton.MouseEntered += () =>
		{
			// 1. Crece un poquito la carta entera
			boton.PivotOffset = boton.Size / 2;
			Tween tweenBoton = CreateTween();
			tweenBoton.TweenProperty(boton, "scale", new Vector2(1.03f, 1.03f), 0.1f)
				 .SetTrans(Tween.TransitionType.Sine);

			// 2. Crece el planeta un poco más para que resalte (Efecto 3D)
			if (planeta != null)
			{
				planeta.PivotOffset = planeta.Size / 2;
				Tween tweenPlaneta = CreateTween();
				// El planeta crecerá a 1.15 de su tamaño original
				tweenPlaneta.TweenProperty(planeta, "scale", new Vector2(1.15f, 1.15f), 0.2f)
					 .SetTrans(Tween.TransitionType.Sine);
			}
		};

		boton.MouseExited += () =>
		{
			// 1. La carta regresa a su tamaño normal
			boton.PivotOffset = boton.Size / 2;
			Tween tweenBoton = CreateTween();
			tweenBoton.TweenProperty(boton, "scale", new Vector2(1.0f, 1.0f), 0.1f)
				 .SetTrans(Tween.TransitionType.Sine);

			// 2. El planeta regresa a su tamaño normal
			if (planeta != null)
			{
				planeta.PivotOffset = planeta.Size / 2;
				Tween tweenPlaneta = CreateTween();
				tweenPlaneta.TweenProperty(planeta, "scale", new Vector2(1.0f, 1.0f), 0.2f)
					 .SetTrans(Tween.TransitionType.Sine);
			}
		};
	}
}
