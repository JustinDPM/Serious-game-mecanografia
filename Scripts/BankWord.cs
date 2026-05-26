using Godot;
using System.Collections.Generic;

public partial class BankWord : Control
{
	private RichTextLabel _historyList;
	private Button _fileBtn;
	private Button _wordBtn;
	private Button _gameBtn; 
	private Button _backBtn; // Tu nuevo botón de Salir

	private FileDialog _fileDialog;
	private AcceptDialog _addWordDialog;
	private LineEdit _wordInput;
	
	private Global _global;

	public List<string> ListaPalabras = new List<string>();

	public override void _Ready()
	{
		_global = GetNodeOrNull<Global>("/root/Global");
		
		// Para que la música fluya si vienes probando la escena directo con F6
		GetNodeOrNull<Node>("/root/AudioManager")?.Call("PlayMenuMusic");
		
		_historyList = GetNode<RichTextLabel>("MainPanel/MarginContainer/RootVBox/ScrollContainer/HistoryList");

		_fileBtn = GetNode<Button>("MainPanel/MarginContainer/RootVBox/ButtonsHBox/FileBtn");
		_wordBtn = GetNode<Button>("MainPanel/MarginContainer/RootVBox/ButtonsHBox/WordBtn");
		_gameBtn = GetNode<Button>("MainPanel/MarginContainer/RootVBox/ButtonsHBox/GameBtn"); 
		_backBtn = GetNode<Button>("MainPanel/MarginContainer/RootVBox/ButtonsHBox/BackBtn"); // Enlazamos el botón de salir

		_fileDialog = GetNode<FileDialog>("FileDialog");
		_addWordDialog = GetNode<AcceptDialog>("AddWordDialog");
		_wordInput = GetNode<LineEdit>("AddWordDialog/WordInput");

		_historyList.BbcodeEnabled = true;
		_fileDialog.CurrentDir = "res://Diccionarios";

		_fileBtn.Pressed += OnFileBtnPressed;
		_wordBtn.Pressed += OnWordBtnPressed;
		_gameBtn.Pressed += OnGameBtnPressed;
		_backBtn.Pressed += OnBackBtnPressed; // Conectamos el clic de salir

		_fileDialog.FileSelected += OnFileSelected;
		_addWordDialog.Confirmed += OnDialogConfirmed;

		if (_global != null && !string.IsNullOrEmpty(_global.RutaTxtCustom))
		{
			CargarArchivoTxt(_global.RutaTxtCustom);
		}
		else
		{
			CargarArchivoTxt("res://Diccionarios/nivel1.txt"); 
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
		string nuevaPalabra = _wordInput.Text.Trim();

		if (string.IsNullOrEmpty(nuevaPalabra)) return;

		string ruta = _global != null ? _global.RutaTxtCustom : "res://Diccionarios/nivel1.txt";
		
		// Abrimos el archivo, escribimos y lo cerramos automáticamente
		using (var file = FileAccess.Open(ruta, FileAccess.ModeFlags.ReadWrite))
		{
			if (file != null)
			{
				file.SeekEnd();
				file.StoreLine(nuevaPalabra);
				
				// 🔥 EL TRUCO: Actualizamos la lista en memoria al instante
				ListaPalabras.Add(nuevaPalabra);
				ActualizarPantalla();
			}
			else
			{
				GD.PrintErr("No se pudo escribir en el archivo: " + ruta);
			}
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

	private void OnGameBtnPressed()
	{
		if (_global != null && !string.IsNullOrEmpty(_global.RutaNivelCustom))
		{
			_global.CambiarEscena(_global.RutaNivelCustom);
		}
		else
		{
			GetTree().ChangeSceneToFile("res://Escenas/game.tscn");
		}
	}

	// 🔥 NUEVA FUNCIÓN PARA EL BOTÓN SALIR 🔥
	private void OnBackBtnPressed()
	{
		if (_global != null)
		{
			// Te regresa a la selección de nivel (ya que de ahí venías)
			_global.CambiarEscena("res://Escenas/level_selection.tscn");
		}
		else
		{
			GetTree().ChangeSceneToFile("res://Escenas/level_selection.tscn");
		}
	}
}
