using Godot;

public partial class Hud : CanvasLayer
{
    [Export] public Turret player;
    [Export] public StatsManager stats;
        
    private RichTextLabel scoreLabel;
    private RichTextLabel wpmLabel;
    private RichTextLabel accuracyLabel;
    private RichTextLabel timeLabel;

    private TextureRect[] hearts;

    public override void _Ready()
    {
        scoreLabel = GetNode<RichTextLabel>("MarginContainer/Root/TopRight/ScoreLabel");
        wpmLabel = GetNode<RichTextLabel>("MarginContainer/Root/TopRight/WpmLabel");
        accuracyLabel = GetNode<RichTextLabel>("MarginContainer/Root/TopRight/AccuracyLabel");
        timeLabel = GetNode<RichTextLabel>("MarginContainer/Root/TopCenter/TimeLabel");

        hearts = new TextureRect[]
        {
            GetNode<TextureRect>("MarginContainer/Root/TopLeft/LivesContainer/Heart1"),
            GetNode<TextureRect>("MarginContainer/Root/TopLeft/LivesContainer/Heart2"),
            GetNode<TextureRect>("MarginContainer/Root/TopLeft/LivesContainer/Heart3"),
            GetNode<TextureRect>("MarginContainer/Root/TopLeft/LivesContainer/Heart4"),
            GetNode<TextureRect>("MarginContainer/Root/TopLeft/LivesContainer/Heart5"),
        };
    }

    public override void _Process(double delta)
    {
        if (player == null) return;

        if (stats == null) return;

        wpmLabel.Text = ((int)stats.GetWPM()).ToString();
        accuracyLabel.Text = stats.GetAccuracy().ToString("0.0") + "%";
        timeLabel.Text = stats.GetTime();

        scoreLabel.Text = player.GetScore().ToString();

        UpdateHearts();
    }

    private void UpdateHearts()
    {
        int hp = player.GetHealth();

        for (int i = 0; i < hearts.Length; i++)
            hearts[i].Visible = i < hp;
    }
}