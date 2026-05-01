using Godot;

public partial class Turret : CharacterBody2D
{
    [Export] public PackedScene BulletScene;
    [Export] public Marker2D shootPoint;

    [Export] public int Health = 10;
    [Export] public int Score = 0;

    [Export] public float FloatAmplitude = 10f;
    [Export] public float FloatSpeed = 2f;

    private RichTextLabel healthLabel;
    private RichTextLabel scoreLabel;
    private Global global;

    private int streak = 0;

    private Vector2 startPosition;
    private float time;

    public override void _Ready()
    {
        healthLabel = GetNode<RichTextLabel>("/root/Level1/HealthLabel");
        scoreLabel = GetNode<RichTextLabel>("/root/Level1/ScoreLabel");
        global = GetNode<Global>("/root/Global");

        startPosition = Position;

        UpdateUI();
    }

    public override void _Process(double delta)
    {
        time += (float)delta;

        float offsetY = Mathf.Sin(time * FloatSpeed) * FloatAmplitude;
        Position = new Vector2(startPosition.X, startPosition.Y + offsetY);
    }

    // 🔥 AHORA SOLO DISPARA 1 BALA
    public void Shoot(Node2D target)
    {
        if (target == null) return;

        var bullet = (Bullet)BulletScene.Instantiate();
        bullet.Position = shootPoint.GlobalPosition;
        bullet.SetTarget(target);

        GetTree().CurrentScene.AddChild(bullet);
    }

    public void TakeDamage(int dmg)
    {
        Health -= dmg;
        streak = 0;

        UpdateUI();
        Blink();

        if (Health <= 0)
            Die();
    }

    public void AddScore(int value)
    {
        if (streak >= 10)
            Score += value * 2;
        else
            Score += value;

        UpdateUI();
    }

    private void UpdateUI()
    {
        if (healthLabel != null)
            healthLabel.Text = "❤️ " + Health;

        if (scoreLabel != null)
        {
            if (streak >= 10)
                scoreLabel.Text = Score + " (X2)";
            else
                scoreLabel.Text = "" + Score;
        }
    }

    private void Die()
    {
        GD.Print("💀 GAME OVER");

        SetProcess(false);
        SetPhysicsProcess(false);

        global.CambiarEscena("res://Escenas/main_menu.tscn");
    }

    private async void Blink()
    {
        var sprite = GetNode<Sprite2D>("Sprite2D");

        var tween = GetTree().CreateTween();

        for (int i = 0; i < 3; i++)
        {
            tween.TweenProperty(sprite, "modulate:a", 0.4f, 0.2f);
            tween.TweenProperty(sprite, "modulate:a", 2.0f, 0.2f);
        }

        await ToSignal(tween, "finished");
    }

    public void addStreak()
    {
        streak++;
    }
}