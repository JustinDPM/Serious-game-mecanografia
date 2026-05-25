using Godot;
using System.Collections.Generic;

public partial class BankWord : Control
{
	private RichTextLabel _historyList;
	
	// Tus tres botones de la foto
	private Button _fileBtn;
	private Button _wordBtn;
	private Button _backBtn;
	
	private FileDialog _fileDialog;

	// Los nodos nuevos para la ventana emergente
	private AcceptDialog _addWordDialog;
	private LineEdit _wordInput;

	// Aquí siempre sabremos qué archivo estamos modificando
	private string _rutaActual = "res://Diccionarios/nivel1.txt";

	public List<string> ListaPrimariaBaja = new List<string>();
	public List<string> ListaPrimariaAlta = new List<string>();
	public List<string> ListaSecundaria = new List<string>();
	public List<string> ListaPreparatoria = new List<string>();

	public override void _Ready()
	{
		_historyList = GetNode<RichTextLabel>("MainPanel/MarginContainer/RootVBox/ScrollContainer/HistoryList");
		
		// Enlazamos tus botones
		_fileBtn = GetNode<Button>("MainPanel/MarginContainer/RootVBox/ButtonsHBox/FileBtn");
		_wordBtn = GetNode<Button>("MainPanel/MarginContainer/RootVBox/ButtonsHBox/WordBtn");
		_backBtn = GetNode<Button>("MainPanel/MarginContainer/RootVBox/ButtonsHBox/BackBtn");
		
		_fileDialog = GetNode<FileDialog>("FileDialog");

		// Enlazamos la ventana emergente (Asegúrate de haberla creado colgando de la raíz)
		_addWordDialog = GetNode<AcceptDialog>("AddWordDialog");
		_wordInput = GetNode<LineEdit>("AddWordDialog/WordInput");

		_historyList.BbcodeEnabled = true;
		_fileDialog.CurrentDir = "res://Diccionarios";

		// Conectamos las acciones
		_fileBtn.Pressed += OnFileBtnPressed;
		_wordBtn.Pressed += OnWordBtnPressed;
		_backBtn.Pressed += OnBackBtnPressed;
		
		_fileDialog.FileSelected += OnFileSelected;
		
		// Conectamos el botón de "OK" de la ventana emergente
		_addWordDialog.Confirmed += OnDialogConfirmed;

		CargarArchivoTxt(_rutaActual);
	}

	private void OnFileBtnPressed()
	{
		// Abre el explorador de archivos nativo de Windows para buscar un txt
		_fileDialog.PopupCentered(new Vector2I(600, 400));
	}

	private void OnWordBtnPressed()
	{
		// Limpiamos la caja de texto por si quedó algo escrito antes
		_wordInput.Text = ""; 
		// Hacemos aparecer la ventanita emergente en el centro
		_addWordDialog.PopupCentered(new Vector2I(350, 100)); 
	}

	// 🔥 ESTO OCURRE CUANDO LE DAS A "OK" EN LA VENTANITA EMERGENTE
	private void OnDialogConfirmed()
	{
		string nuevaPalabra = _wordInput.Text.Trim();

		if (!string.IsNullOrEmpty(nuevaPalabra))
		{
			// Abrimos el archivo actual en modo escritura
			using var file = FileAccess.Open(_rutaActual, FileAccess.ModeFlags.ReadWrite);
			if (file != null)
			{
				file.SeekEnd(); // Nos vamos hasta el final del archivo
				file.StoreLine(nuevaPalabra); // Guardamos la palabra nueva
				
				// Recargamos el texto en pantalla para que la palabra aparezca mágicamente
				CargarArchivoTxt(_rutaActual); 
			}
			else
			{
				GD.PrintErr("No se pudo abrir el archivo para escribir: " + _rutaActual);
			}
		}
	}

	private void OnFileSelected(string rutaArchivo)
	{
		// Cargamos el archivo que el usuario eligió y lo guardamos como el actual
		CargarArchivoTxt(rutaArchivo);
	}

	private void CargarArchivoTxt(string ruta)
	{
		_rutaActual = ruta;

		ListaPrimariaBaja.Clear();
		ListaPrimariaAlta.Clear();
		ListaSecundaria.Clear();
		ListaPreparatoria.Clear();

		if (FileAccess.FileExists(ruta))
		{
			using var file = FileAccess.Open(ruta, FileAccess.ModeFlags.Read);
			
			while (!file.EofReached())
			{
				string linea = file.GetLine().Trim();
				if (!string.IsNullOrEmpty(linea))
				{
					int dificultad = EvaluarDificultadLinea(linea);
					
					if (dificultad == 1) ListaPrimariaBaja.Add(linea);
					else if (dificultad == 2) ListaPrimariaAlta.Add(linea);
					else if (dificultad == 3) ListaSecundaria.Add(linea);
					else if (dificultad == 4) ListaPreparatoria.Add(linea);
				}
			}
			
			string textoPantalla = "\n"; 
			
			textoPantalla += FormatearCategoriaUI("Nivel 1: Primaria Baja", ListaPrimariaBaja);
			textoPantalla += FormatearCategoriaUI("Nivel 2: Primaria Alta", ListaPrimariaAlta);
			textoPantalla += FormatearCategoriaUI("Nivel 3: Secundaria", ListaSecundaria);
			textoPantalla += FormatearCategoriaUI("Nivel 4: Preparatoria", ListaPreparatoria);
			
			_historyList.Text = textoPantalla;
		}
		else
		{
			_historyList.Text = $"[center][color=red]Error: No se encontró el archivo:\n{ruta}[/color][/center]";
		}
	}

	private string FormatearCategoriaUI(string titulo, List<string> lista)
	{
		string bloque = $"[font_size=28][color=#2dd4bf][b]❖ {titulo} ({lista.Count})[/b][/color][/font_size]\n";
		
		if (lista.Count == 0)
		{
			bloque += "    [font_size=22][color=#64748b][i]Sin palabras en esta categoría...[/i][/color][/font_size]\n\n";
		}
		else
		{
			foreach(string palabra in lista)
			{
				bloque += $"    [font_size=26][color=#a855f7]•[/color] {palabra}[/font_size]\n";
			}
			bloque += "\n"; 
		}
		
		return bloque;
	}

	private int EvaluarDificultadLinea(string linea)
	{
		string[] partes = linea.Split(' ', System.StringSplitOptions.RemoveEmptyEntries);
		int cantidadPalabras = partes.Length;

		if (cantidadPalabras == 1)
		{
			int letras = linea.Length;
			if (letras < 5) return 1;
			if (letras < 7) return 2;
			if (letras < 10) return 3;
			return 4;
		}
		else
		{
			if (cantidadPalabras <= 3) return 1;
			if (cantidadPalabras <= 5) return 2;
			if (cantidadPalabras <= 7) return 3;
			return 4;
		}
	}

	public List<string> ObtenerPalabrasParaJuego(int nivelSeleccionado)
	{
		List<string> palabrasParaJugar = new List<string>();

		if (nivelSeleccionado >= 1) palabrasParaJugar.AddRange(ListaPrimariaBaja);
		if (nivelSeleccionado >= 2) palabrasParaJugar.AddRange(ListaPrimariaAlta);
		if (nivelSeleccionado >= 3) palabrasParaJugar.AddRange(ListaSecundaria);
		if (nivelSeleccionado >= 4) palabrasParaJugar.AddRange(ListaPreparatoria);

		return palabrasParaJugar;
	}

	private void OnBackBtnPressed()
	{
		GetTree().ChangeSceneToFile("res://Escenas/main_menu.tscn");
	}
}
