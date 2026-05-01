using Godot;

public partial class Meteor : CharacterBody2D
{
    [Export] public float Speed = 250f;

    public string Word = "";
    private int hitsReceived = 0;

    private RichTextLabel label;
    private Node2D target;

    private bool hasHit = false;
    private bool isDead = false;

    public override void _Ready()
    {
        label = GetNodeOrNull<RichTextLabel>("RichTextLabel");

        if (label != null)
            label.Text = Word;
    }

    public override void _PhysicsProcess(double delta)
    {
        if (!IsInsideTree())
            return;

        if (target != null)
        {
            Vector2 dir = (target.GlobalPosition - GlobalPosition).Normalized();
            Velocity = dir * Speed;
        }

        MoveAndSlide();

        if (hasHit) return;

        for (int i = 0; i < GetSlideCollisionCount(); i++)
        {
            var collision = GetSlideCollision(i);
            var collider = collision.GetCollider();

            if (collider is Turret turret)
            {
                hasHit = true;

                turret.TakeDamage(1);

                var input = GetTree().Root.GetNodeOrNull<InputManager>("Level1/InputManager");
                input?.ResetInput();

                QueueFree();
                return;
            }
        }
    }

    public void SetTarget(Node2D t)
    {
        target = t;
    }

    public void TakeDamage()
    {
        if (isDead) return;

        hitsReceived++;

        if (hitsReceived >= Word.Length)
            Die();
    }

    public void Die()
    {
        if (isDead) return;

        isDead = true;

        var turret = GetTree().Root.GetNodeOrNull<Turret>("Level1/Turret");

        // 🔥 SOLO AQUÍ se suma el score
        turret?.AddScore(1);
        turret?.addStreak();

        var input = GetTree().Root.GetNodeOrNull<InputManager>("Level1/InputManager");
        input?.ResetInput();

        QueueFree();
    }

    public void UpdateDisplay(string input)
    {
        if (label == null) return;

        string result = "";

        for (int i = 0; i < Word.Length; i++)
        {
            if (i < input.Length && input[i] == Word[i])
                result += "[color=green]" + Word[i] + "[/color]";
            else if (i < input.Length)
                result += "[color=red]" + Word[i] + "[/color]";
            else
                result += Word[i];
        }

        label.Text = result;
    }
}