using Godot;

public partial class MatchResultCard : PanelContainer
{
    private Label titleLabel;
    private Label statsLabel;

    public override void _Ready()
    {
        titleLabel = GetNode<Label>(
            "MarginContainer/VBoxContainer/TitleLabel"
        );

        statsLabel = GetNode<Label>(
            "MarginContainer/VBoxContainer/StatsLabel"
        );
    }

    public void Setup(int index, MatchResult match)
    {
        titleLabel.Text = $"Partida {index} - {match.LevelName}";

        statsLabel.Text =
            $"Score: {match.Score}   |   " +
            $"Precisión: {match.Accuracy:0.0}%   |   " +
            $"WPM: {match.WPM:0.0}   |   " +
            $"Tiempo: {match.Duration}";
    }
}