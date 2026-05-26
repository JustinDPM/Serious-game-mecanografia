using Godot;
using System;

public partial class LevelSelection : Control
{
	// Botones de Niveles
	private Button _level1Btn;
	private Button _level2Btn;
	private Button _level3Btn;
	
	// Botones Custom
	private Button _custom1Btn;
	private Button _custom2Btn;
	private Button _custom3Btn;
	
	private Button _bottomBtn; // Tu botón de Volver al menú

	private Global _global;

	public override void _Ready()
	{
		_global = GetNodeOrNull<Global>("/root/Global");

		// Enlazamos los 3 botones principales
		string rutaNiveles = "MarginContainer/VBoxContainer/HBoxContainer/";
		_level1Btn = GetNode<Button>(rutaNiveles + "Level1Btn");
		_level2Btn = GetNode<Button>(rutaNiveles + "Level2Btn");
		_level3Btn = GetNode<Button>(rutaNiveles + "Level3Btn");
		
		// Enlazamos los 3 botones Custom de abajo
		string rutaCustoms = "MarginContainer/VBoxContainer/HBoxContainer2/";
		_custom1Btn = GetNode<Button>(rutaCustoms + "CustomSecu");
		_custom2Btn = GetNode<Button>(rutaCustoms + "CustomSecu2");
		_custom3Btn = GetNode<Button>(rutaCustoms + "CustomSecu3");
		
		_bottomBtn = GetNode<Button>("MarginContainer/VBoxContainer/PlayButton"); 

		// 🔥 INTERCAMBIO DE NIVELES 🔥
		// Primaria (Level1Btn) ahora te lleva a level3
		_level1Btn.Pressed += () => CambiarNivel("res://Escenas/level3.tscn");
		// Secundaria se queda igual
		_level2Btn.Pressed += () => CambiarNivel("res://Escenas/level2.tscn");
		// Preparatoria (Level3Btn) ahora te lleva a game.tscn (el antiguo primaria)
		_level3Btn.Pressed += () => CambiarNivel("res://Escenas/game.tscn");
		
		// 🔥 BOTONES CUSTOM 🔥
		// Mandan la escena correspondiente y el .txt predeterminado
		_custom1Btn.Pressed += () => AbrirBancoPalabras("res://Escenas/level3.tscn", "res://Diccionarios/nivel1.txt");
		_custom2Btn.Pressed += () => AbrirBancoPalabras("res://Escenas/level2.tscn", "res://Diccionarios/nivel2.txt"); // Asegúrate de que nivel2.txt esté en minúscula si así está en tus carpetas
		_custom3Btn.Pressed += () => AbrirBancoPalabras("res://Escenas/game.tscn", "res://Diccionarios/nivel3.txt"); 
		
		_bottomBtn.Pressed += OnBackBtnPressed;

		// Extraemos el TextureRect (El planeta) de cada botón principal para animarlo
		TextureRect planeta1 = _level1Btn.GetNodeOrNull<TextureRect>("MarginContainer/VBoxContainer/TextureRect");
		TextureRect planeta2 = _level2Btn.GetNodeOrNull<TextureRect>("MarginContainer/VBoxContainer/TextureRect");
		TextureRect planeta3 = _level3Btn.GetNodeOrNull<TextureRect>("MarginContainer/VBoxContainer/TextureRect");

		// Animamos los botones
		ConfigurarAnimacionBoton(_level1Btn, planeta1);
		ConfigurarAnimacionBoton(_level2Btn, planeta2);
		ConfigurarAnimacionBoton(_level3Btn, planeta3);
		
		ConfigurarAnimacionBoton(_custom1Btn, null);
		ConfigurarAnimacionBoton(_custom2Btn, null);
		ConfigurarAnimacionBoton(_custom3Btn, null);
		ConfigurarAnimacionBoton(_bottomBtn, null);
	}

	private void AbrirBancoPalabras(string rutaNivelDestino, string rutaTxtDestino)
	{
		if (_global != null)
		{
			// Le decimos al Global a dónde ir y qué archivo cargar
			_global.RutaNivelCustom = rutaNivelDestino;
			_global.RutaTxtCustom = rutaTxtDestino;
			_global.CambiarEscena("res://Escenas/bank_word.tscn");
		}
	}

	private void CambiarNivel(string rutaEscena)
	{
		if (_global != null) _global.CambiarEscena(rutaEscena);
		else GetTree().ChangeSceneToFile(rutaEscena);
	}

	private void OnBackBtnPressed()
	{
		if (_global != null) _global.CambiarEscena("res://Escenas/main_menu.tscn");
		else GetTree().ChangeSceneToFile("res://Escenas/main_menu.tscn");
	}

	private void ConfigurarAnimacionBoton(Button boton, TextureRect planeta)
	{
		boton.MouseEntered += () =>
		{
			boton.PivotOffset = boton.Size / 2;
			Tween tweenBoton = CreateTween();
			tweenBoton.TweenProperty(boton, "scale", new Vector2(1.03f, 1.03f), 0.1f).SetTrans(Tween.TransitionType.Sine);

			if (planeta != null)
			{
				planeta.PivotOffset = planeta.Size / 2;
				Tween tweenPlaneta = CreateTween();
				tweenPlaneta.TweenProperty(planeta, "scale", new Vector2(1.15f, 1.15f), 0.2f).SetTrans(Tween.TransitionType.Sine);
			}
		};

		boton.MouseExited += () =>
		{
			boton.PivotOffset = boton.Size / 2;
			Tween tweenBoton = CreateTween();
			tweenBoton.TweenProperty(boton, "scale", new Vector2(1.0f, 1.0f), 0.1f).SetTrans(Tween.TransitionType.Sine);

			if (planeta != null)
			{
				planeta.PivotOffset = planeta.Size / 2;
				Tween tweenPlaneta = CreateTween();
				tweenPlaneta.TweenProperty(planeta, "scale", new Vector2(1.0f, 1.0f), 0.2f).SetTrans(Tween.TransitionType.Sine);
			}
		};
	}
}
