using Godot;

public partial class MainMenu : Control
{
    private Button _playButton;
    private Button _databaseButton;
    private Button _settingsButton;
    private Button _quitButton;

    private Label _nameMain;
    private Label _usernameMain;
    private TextureRect _avatarImage;
    private Label _pilotName;
    private Label _recordLabel;
    private Label _activeWordsLabel;
    private Label _gradeLabel;

    private Global _global;
    private WordManager _wordManager;

    private SettingsMenu _settingsMenu;
    private Control _mainPanel;

    public override void _Ready()
    {
        _global =
            GetNode<Global>("/root/Global");

        _wordManager =
            GetNode<WordManager>("/root/WordManager");

        string rutaBotones =
            "MarginContainer/VBoxContainer/MainArea/LeftContainer/VBoxContainer/";

        _playButton =
            GetNode<Button>(
                rutaBotones + "PlayButton"
            );

        _databaseButton =
            GetNode<Button>(
                rutaBotones + "DatabaseButton"
            );

        _settingsButton =
            GetNode<Button>(
                rutaBotones + "SettingsButton"
            );

        _quitButton =
            GetNode<Button>(
                rutaBotones + "QuitButton"
            );

        string rutaTopBar =
            "MarginContainer/VBoxContainer/TopBar/UserProfile/";

        _nameMain =
            GetNode<Label>(
                rutaTopBar + "ProfileTexts/NameMain"
            );

        _usernameMain =
            GetNode<Label>(
                rutaTopBar + "ProfileTexts/UsernameMain"
            );

        _avatarImage =
            GetNode<TextureRect>(
                rutaTopBar + "AvatarImage"
            );

        string rutaRight =
            "MarginContainer/VBoxContainer/MainArea/RightContainer/VBoxContainer/";

        _pilotName =
            GetNode<Label>(
                rutaRight + "StatsTop/PilotName"
            );

        _recordLabel =
            GetNode<Label>(
                rutaRight + "StatsTop/Record"
            );

        _activeWordsLabel =
            GetNode<Label>(
                rutaRight + "DictBottom/ActiveWords"
            );

        _gradeLabel =
            GetNode<Label>(
                rutaRight + "DictBottom/Grade"
            );

        _mainPanel =
            GetNode<Control>("MarginContainer");

        _settingsMenu =
            GetNode<SettingsMenu>("SettingsMenu");

        _playButton.Pressed += OnPlayButtonPressed;

        _databaseButton.Pressed +=
            OnDatabaseButtonPressed;

        _settingsButton.Pressed +=
            OnSettingsButtonPressed;

        _quitButton.Pressed +=
            OnQuitButtonPressed;

        Button[] botones =
        {
            _playButton,
            _databaseButton,
            _settingsButton,
            _quitButton
        };

        foreach (Button btn in botones)
            ConfigurarAnimacionBoton(btn);

        GetNode<AudioManager>("/root/AudioManager")
            .PlayMenuMusic();

        ActualizarInterfaz();
    }

    private void ActualizarInterfaz()
    {
        _nameMain.Text =
            _global.Rol;

        _usernameMain.Text =
            _global.UsuarioActivo;

        string primerNombre =
            _global.NombreCompleto.Split(' ')[0];

        _pilotName.Text =
            $"PILOTO: {primerNombre.ToUpper()}";

        string nombreGrado =
            _global.IdGrado switch
            {
                1 => "PRIMARIA BAJA",
                2 => "PRIMARIA ALTA",
                3 => "SECUNDARIA",
                4 => "PREPARATORIA",
                _ => "SIN ASIGNAR"
            };

        _gradeLabel.Text =
            $"GRADO: {nombreGrado}";

        if (
            ResourceLoader.Exists(
                _global.RutaFotoPerfil
            )
        )
        {
            _avatarImage.Texture =
                GD.Load<Texture2D>(
                    _global.RutaFotoPerfil
                );
        }
        else
        {
            _avatarImage.Texture =
                GD.Load<Texture2D>(
                    "res://assets/Perfiles/default.jpg"
                );
        }

        _recordLabel.Text =
            $"RECORD: {_global.LastWPM:0} WPM";

        int palabrasActivas =
            _wordManager
                .ObtenerPalabrasParaJuego(
                    _global.IdGrado
                )
                .Count;

        _activeWordsLabel.Text =
            $"PALABRAS ACTIVAS: {palabrasActivas}";
    }

    private void ConfigurarAnimacionBoton(
        Button boton
    )
    {
        boton.MouseEntered += () =>
        {
            boton.PivotOffset =
                boton.Size / 2;

            Tween tween =
                CreateTween();

            tween.TweenProperty(
                boton,
                "scale",
                new Vector2(1.05f, 1.05f),
                0.1f
            ).SetTrans(
                Tween.TransitionType.Sine
            );
        };

        boton.MouseExited += () =>
        {
            boton.PivotOffset =
                boton.Size / 2;

            Tween tween =
                CreateTween();

            tween.TweenProperty(
                boton,
                "scale",
                new Vector2(1.0f, 1.0f),
                0.1f
            ).SetTrans(
                Tween.TransitionType.Sine
            );
        };
    }

    private void OnPlayButtonPressed()
    {
        _global.CambiarEscena(
            "res://Escenas/level_selection.tscn"
        );
    }

    private void OnDatabaseButtonPressed()
    {
        _global.CambiarEscena(
            "res://Escenas/bank_word.tscn"
        );
    }

    private void OnSettingsButtonPressed()
    {
        _mainPanel.Visible = false;

        _settingsMenu.Open(false);
    }

    private void OnQuitButtonPressed()
    {
        GetTree().Quit();
    }

    private void OnStatsPressed()
    {
        _global.CambiarEscena(
            "res://Escenas/stats_screen.tscn"
        );
    }
}