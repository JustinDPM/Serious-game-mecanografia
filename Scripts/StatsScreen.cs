using System.Text.RegularExpressions;
using Godot;

public partial class StatsScreen : Control
{
    private Global global;
    private VBoxContainer historyList;
    private Button backButton;
    [Export] private PackedScene matchCardScene;

    public override void _Ready()
    {
        global = GetNode<Global>("/root/Global");

        historyList = GetNode<VBoxContainer>(
            "MainPanel/MarginContainer/RootVBox/ScrollContainer/HistoryList"
        );

        backButton = GetNode<Button>(
            "MainPanel/MarginContainer/RootVBox/ButtonsHBox/BackBtn"
        );

        backButton.Pressed += OnBackPressed;

        GD.Print("Partidas guardadas: " + global.MatchHistory.Count);
        LoadHistory();
    }

    private void LoadHistory()
    {
        foreach (Node child in historyList.GetChildren())
            child.QueueFree();

        if (global.MatchHistory.Count == 0)
        {
            return;
        }

        for (int i = 0; i < global.MatchHistory.Count; i++)
        {
            MatchResult match = global.MatchHistory[i];

            AddCard(i + 1, match);
        }
    }

    private void AddCard(int index, MatchResult match)
    {
        var card = matchCardScene.Instantiate<MatchResultCard>();

        historyList.AddChild(card);

        card.Setup(index, match);
    }

    private void OnBackPressed()
    {
        global.CambiarEscena("res://Escenas/main_menu.tscn");
    }
}