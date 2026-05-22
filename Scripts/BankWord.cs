using Godot;
using System.Collections.Generic;

public partial class BankWord : Control
{
	private RichTextLabel _historyList;
	private Button _wordBtn;
	private Button _backBtn;
	private FileDialog _fileDialog;

	public List<string> ListaPrimariaBaja = new List<string>();
	public List<string> ListaPrimariaAlta = new List<string>();
	public List<string> ListaSecundaria = new List<string>();
	public List<string> ListaPreparatoria = new List<string>();

	public override void _Ready()
	{
		_historyList = GetNode<RichTextLabel>("MainPanel/MarginContainer/RootVBox/ScrollContainer/HistoryList");
		_wordBtn = GetNode<Button>("MainPanel/MarginContainer/RootVBox/ButtonsHBox/WordBtn");
		_backBtn = GetNode<Button>("MainPanel/MarginContainer/RootVBox/ButtonsHBox/BackBtn");
		_fileDialog = GetNode<FileDialog>("FileDialog");

		_historyList.BbcodeEnabled = true;
		_fileDialog.CurrentDir = "res://Diccionarios";

		_wordBtn.Pressed += OnWordBtnPressed;
		_backBtn.Pressed += OnBackBtnPressed;
		_fileDialog.FileSelected += OnFileSelected;

		CargarArchivoTxt("res://Diccionarios/nivel1.txt");
	}

	private void OnWordBtnPressed()
	{
		_fileDialog.PopupCentered(new Vector2I(600, 400));
	}

	private void OnFileSelected(string rutaArchivo)
	{
		CargarArchivoTxt(rutaArchivo);
	}

	private void CargarArchivoTxt(string ruta)
	{
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
			
			// Quitamos las etiquetas [center] para que por defecto se alinee a la izquierda
			string textoPantalla = "\n"; 
			
			textoPantalla += FormatearCategoriaUI("Nivel 1: Primaria Baja", ListaPrimariaBaja);
			textoPantalla += FormatearCategoriaUI("Nivel 2: Primaria Alta", ListaPrimariaAlta);
			textoPantalla += FormatearCategoriaUI("Nivel 3: Secundaria", ListaSecundaria);
			textoPantalla += FormatearCategoriaUI("Nivel 4: Preparatoria", ListaPreparatoria);
			
			_historyList.Text = textoPantalla;
		}
		else
		{
			// Este sí lo dejamos centrado porque es un mensaje de error
			_historyList.Text = $"[center][color=red]Error: No se encontró el archivo:\n{ruta}[/color][/center]";
		}
	}

	private string FormatearCategoriaUI(string titulo, List<string> lista)
	{
		// Título de la categoría en tamaño 28
		string bloque = $"[font_size=28][color=#2dd4bf][b]❖ {titulo} ({lista.Count})[/b][/color][/font_size]\n";
		
		if (lista.Count == 0)
		{
			// Si no hay palabras, texto en cursiva tamaño 22
			bloque += "    [font_size=22][color=#64748b][i]Sin palabras en esta categoría...[/i][/color][/font_size]\n\n";
		}
		else
		{
			foreach(string palabra in lista)
			{
				// Palabras de la lista en tamaño 26 para que se lean perfecto
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
