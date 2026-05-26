using Godot;
using System.Collections.Generic;

public partial class BankWord : Control
{
	private RichTextLabel _historyList;
	private Button _fileBtn;
	private Button _wordBtn;
	private Button _gameBtn; // Tu nuevo botón de jugar
	private FileDialog _fileDialog;
	private AcceptDialog _addWordDialog;
	private LineEdit _wordInput;
	
	private Global _global;

	// Ahora usamos una sola lista
	public List<string> ListaPalabras = new List<string>();

	public override void _Ready()
	{
		_global = GetNodeOrNull<Global>("/root/Global");
		
		_historyList = GetNode<RichTextLabel>("MainPanel/MarginContainer/RootVBox/ScrollContainer/HistoryList");

		_fileBtn = GetNode<Button>("MainPanel/MarginContainer/RootVBox/ButtonsHBox/FileBtn");
		_wordBtn = GetNode<Button>("MainPanel/MarginContainer/RootVBox/ButtonsHBox/WordBtn");
		_gameBtn = GetNode<Button>("MainPanel/MarginContainer/RootVBox/ButtonsHBox/GameBtn"); // Conectamos el botón Iniciar Juego

		_fileDialog = GetNode<FileDialog>("FileDialog");
		_addWordDialog = GetNode<AcceptDialog>("AddWordDialog");
		_wordInput = GetNode<LineEdit>("AddWordDialog/WordInput");

		_historyList.BbcodeEnabled = true;
		_fileDialog.CurrentDir = "res://Diccionarios";

		_fileBtn.Pressed += OnFileBtnPressed;
		_wordBtn.Pressed += OnWordBtnPressed;
		
		// Conectamos el botón jugar
		_gameBtn.Pressed += OnGameBtnPressed;

		_fileDialog.FileSelected += OnFileSelected;
		_addWordDialog.Confirmed += OnDialogConfirmed;

		// Cargamos el Txt que seleccionó el usuario en el menú anterior
		if (_global != null && !string.IsNullOrEmpty(_global.RutaTxtCustom))
		{
			CargarArchivoTxt(_global.RutaTxtCustom);
		}
		else
		{
			CargarArchivoTxt("res://Diccionarios/nivel1.txt"); // Por defecto por si acaso
		}
	}

	private void OnFileBtnPressed() => _fileDialog.PopupCentered(new Vector2I(600, 400));
	private void OnWordBtnPressed()
	{
		_wordInput.Text = "";
		_addWordDialog.PopupCentered(new Vector2I(350, 100));
	}

	private void OnDialogConfirmed()
	{
		// .Trim() elimina espacios al inicio y al final. Si hay espacios en medio, los respeta (frase).
		string nuevaPalabra = _wordInput.Text.Trim();

		if (string.IsNullOrEmpty(nuevaPalabra)) return;

		// Guardamos la palabra en el txt actual
		string ruta = _global != null ? _global.RutaTxtCustom : "res://Diccionarios/nivel1.txt";
		
		using var file = FileAccess.Open(ruta, FileAccess.ModeFlags.ReadWrite);
		if (file != null)
		{
			file.SeekEnd();
			file.StoreLine(nuevaPalabra);
			CargarArchivoTxt(ruta); // Recargamos para verla
		}
		else
		{
			GD.PrintErr("No se pudo escribir en el archivo: " + ruta);
		}
	}

	private void OnFileSelected(string rutaArchivo)
	{
		if (_global != null) _global.RutaTxtCustom = rutaArchivo;
		CargarArchivoTxt(rutaArchivo);
	}

	private void CargarArchivoTxt(string ruta)
	{
		ListaPalabras.Clear();

		if (FileAccess.FileExists(ruta))
		{
			using var file = FileAccess.Open(ruta, FileAccess.ModeFlags.Read);
			
			while (!file.EofReached())
			{
				// .Trim() limpia espacios muertos, pero deja frases intactas
				string linea = file.GetLine().Trim();
				if (!string.IsNullOrEmpty(linea))
				{
					ListaPalabras.Add(linea);
				}
			}
			ActualizarPantalla();
		}
		else
		{
			_historyList.Text = $"[center][color=red]Archivo no encontrado:\n{ruta}[/color][/center]";
		}
	}

	private void ActualizarPantalla()
	{
		string textoPantalla = "\n";
		
		// Ahora solo mostramos una gran categoría de "Palabras Activas"
		textoPantalla += $"[font_size=28][color=#2dd4bf][b]❖ PALABRAS EN EL DICCIONARIO ({ListaPalabras.Count})[/b][/color][/font_size]\n";

		if (ListaPalabras.Count == 0)
		{
			textoPantalla += "    [font_size=22][color=#64748b][i]El archivo está vacío...[/i][/color][/font_size]\n\n";
		}
		else
		{
			foreach (string palabra in ListaPalabras)
			{
				textoPantalla += $"    [font_size=26][color=#a855f7]•[/color] {palabra}[/font_size]\n";
			}
		}

		_historyList.Text = textoPantalla;
	}

	// 🔥 ESTE ES EL BOTÓN MÁGICO 🔥
	private void OnGameBtnPressed()
	{
		// Si sabemos de qué nivel venimos, vamos hacia allá
		if (_global != null && !string.IsNullOrEmpty(_global.RutaNivelCustom))
		{
			_global.CambiarEscena(_global.RutaNivelCustom);
		}
		else
		{
			// Respaldo por si falló algo
			GetTree().ChangeSceneToFile("res://Escenas/game.tscn");
		}
	}
}
