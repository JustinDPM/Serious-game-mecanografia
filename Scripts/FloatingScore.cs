using Godot;

public partial class FloatingScore : Node2D
{
    private Label label;

    public override void _Ready()
    {
        ZIndex = 100;
        ZAsRelative = false;

        label = GetNode<Label>("Label");

        label.Position = new Vector2(-250, -80);
        label.Size = new Vector2(500, 160);
        label.HorizontalAlignment = HorizontalAlignment.Center;
        label.VerticalAlignment = VerticalAlignment.Center;
        label.Visible = true;
    }

    public void Setup(string text, Color color)
    {
        label.Text = text;
        label.Modulate = color;
        label.SelfModulate = color;

        PlayAnimation();
    }

    private void PlayAnimation()
    {
        var tween = GetTree().CreateTween();

        Vector2 startPos = GlobalPosition;

        tween.TweenProperty(
            this,
            "global_position",
            startPos + new Vector2(0, -80),
            1.0f
        );

        tween.Parallel().TweenProperty(
            this,
            "scale",
            new Vector2(1.2f, 1.2f),
            0.15f
        );

        tween.Parallel().TweenProperty(
            this,
            "modulate:a",
            0f,
            1.0f
        );

        tween.TweenCallback(
            Callable.From(QueueFree)
        );
    }
}