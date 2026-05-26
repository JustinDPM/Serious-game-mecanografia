using Godot;
using System;

public partial class LevelSelection : Control
{
	private Button _level1Btn, _level2Btn, _level3Btn;
	private Button _custom1Btn, _custom2Btn, _custom3Btn;
	private Button _bottomBtn;

	private Global _global;

	public override void _Ready()
	{
		_global = GetNodeOrNull<Global>("/root/Global");

		string rutaCartas = "MarginContainer/VBoxContainer/VBoxContainer/HBoxContainer/";
		_level1Btn = GetNodeOrNull<Button>(rutaCartas + "Level1Btn");
		_level2Btn = GetNodeOrNull<Button>(rutaCartas + "Level2Btn");
		_level3Btn = GetNodeOrNull<Button>(rutaCartas + "Level3Btn");

		string rutaCustoms = "MarginContainer/VBoxContainer/VBoxContainer/HBoxContainer2/";
		_custom1Btn = GetNodeOrNull<Button>(rutaCustoms + "CustomSecu");
		_custom2Btn = GetNodeOrNull<Button>(rutaCustoms + "CustomSecu2");
		_custom3Btn = GetNodeOrNull<Button>(rutaCustoms + "CustomSecu3");

		_bottomBtn = GetNodeOrNull<Button>("MarginContainer/VBoxContainer/PlayButton");

		// =========================
		// BOTONES NORMALES
		// =========================

		if (_level1Btn != null)
			_level1Btn.Pressed += () =>
				CambiarNivel(
					"res://Escenas/level3.tscn",
                    "res://Diccionarios/primaria.txt"
				);
		else
			GD.PrintErr("🚨 ERROR CÓDIGO: No encuentro Level1Btn");

		if (_level2Btn != null)
			_level2Btn.Pressed += () =>
				CambiarNivel(
					"res://Escenas/level2.tscn",
                    "res://Diccionarios/secundaria.txt"
				);
		else
			GD.PrintErr("🚨 ERROR CÓDIGO: No encuentro Level2Btn");

		if (_level3Btn != null)
			_level3Btn.Pressed += () =>
				CambiarNivel(
					"res://Escenas/game.tscn",
                    "res://Diccionarios/preparatoria.txt"
				);
		else
			GD.PrintErr("🚨 ERROR CÓDIGO: No encuentro Level3Btn");

		// =========================
		// BOTONES CUSTOM
		// =========================

		if (_custom1Btn != null)
			_custom1Btn.Pressed += () =>
				AbrirBancoPalabras(
					"res://Escenas/level3.tscn",
                    "res://Diccionarios/primaria.txt"
				);
		else
			GD.PrintErr("🚨 ERROR CÓDIGO: No encuentro CustomSecu");

		if (_custom2Btn != null)
			_custom2Btn.Pressed += () =>
				AbrirBancoPalabras(
					"res://Escenas/level2.tscn",
                    "res://Diccionarios/secundaria.txt"
				);
		else
			GD.PrintErr("🚨 ERROR CÓDIGO: No encuentro CustomSecu2");

		if (_custom3Btn != null)
			_custom3Btn.Pressed += () =>
				AbrirBancoPalabras(
					"res://Escenas/game.tscn",
                    "res://Diccionarios/preparatoria.txt"
				);
		else
			GD.PrintErr("🚨 ERROR CÓDIGO: No encuentro CustomSecu3");

		if (_bottomBtn != null)
			_bottomBtn.Pressed += OnBackBtnPressed;
		else
			GD.PrintErr("🚨 ERROR CÓDIGO: No encuentro PlayButton");

		TextureRect planeta1 =
			_level1Btn?.GetNodeOrNull<TextureRect>(
                "MarginContainer/VBoxContainer/TextureRect"
			);

		TextureRect planeta2 =
			_level2Btn?.GetNodeOrNull<TextureRect>(
                "MarginContainer/VBoxContainer/TextureRect"
			);

		TextureRect planeta3 =
			_level3Btn?.GetNodeOrNull<TextureRect>(
                "MarginContainer/VBoxContainer/TextureRect"
			);

		if (_level1Btn != null)
			ConfigurarAnimacionBoton(_level1Btn, planeta1);

		if (_level2Btn != null)
			ConfigurarAnimacionBoton(_level2Btn, planeta2);

		if (_level3Btn != null)
			ConfigurarAnimacionBoton(_level3Btn, planeta3);

		if (_custom1Btn != null)
			ConfigurarAnimacionBoton(_custom1Btn, null);

		if (_custom2Btn != null)
			ConfigurarAnimacionBoton(_custom2Btn, null);

		if (_custom3Btn != null)
			ConfigurarAnimacionBoton(_custom3Btn, null);

		if (_bottomBtn != null)
			ConfigurarAnimacionBoton(_bottomBtn, null);
	}

	private void AbrirBancoPalabras(
		string rutaNivelDestino,
		string rutaTxtDestino
	)
	{
		if (_global != null)
		{
			_global.RutaNivelCustom =
				rutaNivelDestino;

			_global.RutaTxtCustom =
				rutaTxtDestino;

			_global.CambiarEscena(
                "res://Escenas/bank_word.tscn"
			);
		}
	}

	private void CambiarNivel(
		string rutaEscena,
		string rutaTxt
	)
	{
		if (_global != null)
		{
			_global.RutaTxtCustom =
				rutaTxt;

			_global.CambiarEscena(
				rutaEscena
			);
		}
		else
		{
			GetTree().ChangeSceneToFile(
				rutaEscena
			);
		}
	}

	private void OnBackBtnPressed()
	{
		if (_global != null)
		{
			_global.CambiarEscena(
                "res://Escenas/main_menu.tscn"
			);
		}
		else
		{
			GetTree().ChangeSceneToFile(
                "res://Escenas/main_menu.tscn"
			);
		}
	}

	private void ConfigurarAnimacionBoton(
		Button boton,
		TextureRect planeta
	)
	{
		boton.MouseEntered += () =>
		{
			if (
				!IsInstanceValid(this) ||
				!IsInstanceValid(boton) ||
				!IsInsideTree()
			)
			{
				return;
			}

			boton.PivotOffset =
				boton.Size / 2;

			Tween tweenBoton =
				CreateTween();

			tweenBoton.TweenProperty(
				boton,
				"scale",
				new Vector2(1.03f, 1.03f),
				0.1f
			).SetTrans(
				Tween.TransitionType.Sine
			);

			if (
				planeta != null &&
				IsInstanceValid(planeta)
			)
			{
				planeta.PivotOffset =
					planeta.Size / 2;

				Tween tweenPlaneta =
					CreateTween();

				tweenPlaneta.TweenProperty(
					planeta,
					"scale",
					new Vector2(1.15f, 1.15f),
					0.2f
				).SetTrans(
					Tween.TransitionType.Sine
				);
			}
		};

		boton.MouseExited += () =>
		{
			if (
				!IsInstanceValid(this) ||
				!IsInstanceValid(boton) ||
				!IsInsideTree()
			)
			{
				return;
			}

			boton.PivotOffset =
				boton.Size / 2;

			Tween tweenBoton =
				CreateTween();

			tweenBoton.TweenProperty(
				boton,
				"scale",
				new Vector2(1.0f, 1.0f),
				0.1f
			).SetTrans(
				Tween.TransitionType.Sine
			);

			if (
				planeta != null &&
				IsInstanceValid(planeta)
			)
			{
				planeta.PivotOffset =
					planeta.Size / 2;

				Tween tweenPlaneta =
					CreateTween();

				tweenPlaneta.TweenProperty(
					planeta,
					"scale",
					new Vector2(1.0f, 1.0f),
					0.2f
				).SetTrans(
					Tween.TransitionType.Sine
				);
			}
		};
	}
}
