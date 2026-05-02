using Godot;

public partial class GameOver : Control
{
    private Button retryButton;
    private Button exitButton;
    private Global global;

    private Label scoreLabel;
    private Label accuracyLabel;
    private Label wpmLabel;

    public override void _Ready()
    {
        retryButton = GetNode<Button>("TextureRect/CenterContainer/PrincipalPanel/MarginContainer/PrincipalVBox/ButtonsHBox/BtnRetry");
        exitButton = GetNode<Button>("TextureRect/CenterContainer/PrincipalPanel/MarginContainer/PrincipalVBox/ButtonsHBox/BtnMainMenu");
        global = GetNode<Global>("/root/Global");

        scoreLabel = GetNode<Label>("TextureRect/CenterContainer/PrincipalPanel/MarginContainer/PrincipalVBox/HBoxContainer/StatsGrid/ValWords");
        accuracyLabel = GetNode<Label>("TextureRect/CenterContainer/PrincipalPanel/MarginContainer/PrincipalVBox/HBoxContainer/StatsGrid/ValAccuracy");
        wpmLabel = GetNode<Label>("TextureRect/CenterContainer/PrincipalPanel/MarginContainer/PrincipalVBox/HBoxContainer/StatsGrid/ValWPM");

        scoreLabel.Text = global.LastScore.ToString();
        accuracyLabel.Text = global.LastAccuracy.ToString("0.0") + "%";
        wpmLabel.Text = ((int)global.LastWPM).ToString();

        retryButton.Pressed += OnRetryPressed;
        exitButton.Pressed += OnExitPressed;
    }

    private void OnRetryPressed()
    {
        global.CallDeferred("CambiarEscena", "res://Escenas/game.tscn");
    }

    private void OnExitPressed()
    {

        global.CallDeferred("CambiarEscena", "res://Escenas/main_menu.tscn");
    }
}