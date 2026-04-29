using Godot;

public partial class Meteor : CharacterBody2D
{
    [Export] public float Speed = 120f;

    public string Word = "";
    private int Health;

    private RichTextLabel label;
    private Node2D target;

    public override void _Ready()
    {
        label = GetNode<RichTextLabel>("RichTextLabel");
        label.Text = Word;

        Health = Word.Length;
    }

    public override void _PhysicsProcess(double delta)
    {
        if (target != null)
        {
            Vector2 dir = (target.GlobalPosition - GlobalPosition).Normalized();
            Velocity = dir * Speed;
        }

        MoveAndSlide();

        for (int i = 0; i < GetSlideCollisionCount(); i++)
        {
            var collision = GetSlideCollision(i);
            var collider = collision.GetCollider();

            if (collider is Turret turret)
            {
                turret.TakeDamage(1);
                QueueFree();
                break;
            }
        }
    }

    public void SetTarget(Node2D t)
    {
        target = t;
    }

    public void TakeDamage()
    {
        Health--;

        if (Health <= 0)
            Die();
    }

    public void Die()
    {
        var turret = GetNode<Turret>("/root/Level1/Turret");
        turret.AddScore(1);
        QueueFree();
    }

    public void UpdateDisplay(string input)
    {
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