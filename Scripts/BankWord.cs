using Godot;
using System.Collections.Generic;

public partial class BankWord : Control
{
    private RichTextLabel _historyList;

    private Button _fileBtn;
    private Button _wordBtn;
    private Button _backBtn;

    private FileDialog _fileDialog;

    private AcceptDialog _addWordDialog;
    private LineEdit _wordInput;

    private WordManager wordManager;

    private string _rutaActual = "res://Diccionarios/nivel1.txt";

    public override void _Ready()
    {
        wordManager =
            GetNode<WordManager>("/root/WordManager");

        _historyList = GetNode<RichTextLabel>("MainPanel/MarginContainer/RootVBox/ScrollContainer/HistoryList");

        _fileBtn = GetNode<Button>("MainPanel/MarginContainer/RootVBox/ButtonsHBox/FileBtn");
        _wordBtn = GetNode<Button>("MainPanel/MarginContainer/RootVBox/ButtonsHBox/WordBtn");
        _backBtn = GetNode<Button>("MainPanel/MarginContainer/RootVBox/ButtonsHBox/BackBtn");

        _fileDialog = GetNode<FileDialog>("FileDialog");
        _addWordDialog = GetNode<AcceptDialog>("AddWordDialog");
        _wordInput = GetNode<LineEdit>("AddWordDialog/WordInput");

        _historyList.BbcodeEnabled = true;
        _fileDialog.CurrentDir = "res://Diccionarios";

        _fileBtn.Pressed += OnFileBtnPressed;
        _wordBtn.Pressed += OnWordBtnPressed;
        _backBtn.Pressed += OnBackBtnPressed;

        _fileDialog.FileSelected += OnFileSelected;

        _addWordDialog.Confirmed += OnDialogConfirmed;

        wordManager.CargarArchivoTxt(_rutaActual);
        ActualizarPantalla();
    }

    private void OnFileBtnPressed()
    {
        _fileDialog.PopupCentered(new Vector2I(600, 400));
    }

    private void OnWordBtnPressed()
    {
        _wordInput.Text = "";
        _addWordDialog.PopupCentered(new Vector2I(350, 100));
    }

    private void OnDialogConfirmed()
    {
        string nuevaPalabra = _wordInput.Text.Trim();

        if (string.IsNullOrEmpty(nuevaPalabra))
            return;

        bool agregada =
            wordManager.AgregarPalabra(nuevaPalabra);

        if (!agregada)
        {
            GD.PrintErr(
                "No se pudo agregar la palabra: " +
                nuevaPalabra
            );

            return;
        }

        ActualizarPantalla();
    }

    private void OnFileSelected(string rutaArchivo)
    {
        _rutaActual = rutaArchivo;

        wordManager.CargarArchivoTxt(_rutaActual);

        ActualizarPantalla();
    }

    private void ActualizarPantalla()
    {
        if (wordManager == null)
            return;

        string textoPantalla = "\n";

        textoPantalla += FormatearCategoriaUI(
            "Nivel 1: Primaria Baja",
            wordManager.ListaPrimariaBaja
        );

        textoPantalla += FormatearCategoriaUI(
            "Nivel 2: Primaria Alta",
            wordManager.ListaPrimariaAlta
        );

        textoPantalla += FormatearCategoriaUI(
            "Nivel 3: Secundaria",
            wordManager.ListaSecundaria
        );

        textoPantalla += FormatearCategoriaUI(
            "Nivel 4: Preparatoria",
            wordManager.ListaPreparatoria
        );

        _historyList.Text = textoPantalla;
    }

    private string FormatearCategoriaUI(
        string titulo,
        List<string> lista
    )
    {
        string bloque =
            $"[font_size=28][color=#2dd4bf][b]❖ {titulo} ({lista.Count})[/b][/color][/font_size]\n";

        if (lista.Count == 0)
        {
            bloque +=
                "    [font_size=22][color=#64748b][i]Sin palabras en esta categoría...[/i][/color][/font_size]\n\n";
        }
        else
        {
            foreach (string palabra in lista)
            {
                bloque +=
                    $"    [font_size=26][color=#a855f7]•[/color] {palabra}[/font_size]\n";
            }

            bloque += "\n";
        }

        return bloque;
    }

    public List<string> ObtenerPalabrasParaJuego(
        int nivelSeleccionado
    )
    {
        return wordManager.ObtenerPalabrasParaJuego(
            nivelSeleccionado
        );
    }

    private void OnBackBtnPressed()
    {
        GetTree().ChangeSceneToFile(
            "res://Escenas/main_menu.tscn"
        );
    }
}