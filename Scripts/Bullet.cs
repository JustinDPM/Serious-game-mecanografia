using Godot;

public partial class Bullet : Area2D
{
    [Export] public float Speed = 1700f;

    private Node2D target;
    private bool hasHit = false;

    public override void _Ready()
    {
        BodyEntered += OnBodyEntered;
    }

    public override void _PhysicsProcess(double delta)
    {
        if (target == null || !IsInstanceValid(target))
        {
            QueueFree();
            return;
        }

        Vector2 dir = (target.GlobalPosition - GlobalPosition).Normalized();

        Position += dir * Speed * (float)delta;
        Rotation = dir.Angle();
    }

    public void SetTarget(Node2D t)
    {
        target = t;
    }

    private void OnBodyEntered(Node2D body)
    {
        if (hasHit) return;

        if (body is Meteor meteor)
        {
            hasHit = true;

            meteor.TakeDamage(); // 💥 1 bala = 1 daño

            QueueFree();
        }
    }
}