using Godot;
using System.Collections.Generic;

public partial class BankWord : Control
{
    private RichTextLabel _historyList;
    private Button _fileBtn;
    private Button _wordBtn;
    private Button _gameBtn;
    private Button _backBtn;

    private FileDialog _fileDialog;
    private AcceptDialog _addWordDialog;
    private LineEdit _wordInput;

    private Global _global;
    private WordManager _wordManager;

    public override void _Ready()
    {
        _global =
            GetNodeOrNull<Global>("/root/Global");

        _wordManager =
            GetNode<WordManager>("/root/WordManager");

        GetNodeOrNull<Node>("/root/AudioManager")
            ?.Call("PlayMenuMusic");

        _historyList =
            GetNode<RichTextLabel>(
                "MainPanel/MarginContainer/RootVBox/ScrollContainer/HistoryList"
            );

        _fileBtn =
            GetNode<Button>(
                "MainPanel/MarginContainer/RootVBox/ButtonsHBox/FileBtn"
            );

        _wordBtn =
            GetNode<Button>(
                "MainPanel/MarginContainer/RootVBox/ButtonsHBox/WordBtn"
            );

        _gameBtn =
            GetNode<Button>(
                "MainPanel/MarginContainer/RootVBox/ButtonsHBox/GameBtn"
            );

        _backBtn =
            GetNode<Button>(
                "MainPanel/MarginContainer/RootVBox/ButtonsHBox/BackBtn"
            );

        _fileDialog = GetNode<FileDialog>("FileDialog");
        _addWordDialog = GetNode<AcceptDialog>("AddWordDialog");
        _wordInput = GetNode<LineEdit>("AddWordDialog/WordInput");

        _historyList.BbcodeEnabled = true;
        _fileDialog.CurrentDir = "res://Diccionarios";

        _fileBtn.Pressed += OnFileBtnPressed;
        _wordBtn.Pressed += OnWordBtnPressed;
        _gameBtn.Pressed += OnGameBtnPressed;
        _backBtn.Pressed += OnBackBtnPressed;

        _fileDialog.FileSelected += OnFileSelected;
        _addWordDialog.Confirmed += OnDialogConfirmed;

        string ruta =
            _global != null &&
            !string.IsNullOrEmpty(_global.RutaTxtCustom)
                ? _global.RutaTxtCustom
                : "res://Diccionarios/nivel1.txt";

        _wordManager.CargarArchivoTxt(ruta);
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
            _wordManager.AgregarPalabra(nuevaPalabra);

        if (!agregada)
        {
            GD.PrintErr(
                "No se pudo agregar: " + nuevaPalabra
            );

            return;
        }

        ActualizarPantalla();
    }

    private void OnFileSelected(string rutaArchivo)
    {
        if (_global != null)
            _global.RutaTxtCustom = rutaArchivo;

        _wordManager.CargarArchivoTxt(rutaArchivo);

        ActualizarPantalla();
    }

    private void ActualizarPantalla()
    {
        string textoPantalla = "\n";

        textoPantalla += FormatearCategoriaUI(
            "Palabras para Meteoritos Normales",
            _wordManager.ListaPalabras
        );

        textoPantalla += FormatearCategoriaUI(
            "Frases para Meteoritos Grandes",
            _wordManager.ListaFrases
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
                "    [font_size=22][color=#64748b][i]Sin elementos...[/i][/color][/font_size]\n\n";
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

    private void OnGameBtnPressed()
    {
        if (
            _global != null &&
            !string.IsNullOrEmpty(_global.RutaNivelCustom)
        )
        {
            _global.CambiarEscena(_global.RutaNivelCustom);
        }
        else
        {
            GetTree().ChangeSceneToFile(
                "res://Escenas/game.tscn"
            );
        }
    }

    private void OnBackBtnPressed()
    {
        if (_global != null)
        {
            _global.CambiarEscena(
                "res://Escenas/level_selection.tscn"
            );
        }
        else
        {
            GetTree().ChangeSceneToFile(
                "res://Escenas/level_selection.tscn"
            );
        }
    }
}