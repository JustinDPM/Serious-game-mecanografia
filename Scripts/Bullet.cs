using Godot;

public partial class Bullet : Area2D
{
    [Export] public float Speed = 2500f;

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

        Vector2 dir =
            (target.GlobalPosition - GlobalPosition)
            .Normalized();

        Rotation = dir.Angle();

        Position += dir * Speed * (float)delta;
    }

    public void SetTarget(Node2D t)
    {
        target = t;

        UpdateRotationToTarget();
    }

    private void UpdateRotationToTarget()
    {
        if (target == null || !IsInstanceValid(target))
            return;

        Vector2 dir =
            (target.GlobalPosition - GlobalPosition)
            .Normalized();

        Rotation = dir.Angle();
    }

    private void OnBodyEntered(Node2D body)
    {
        if (hasHit)
            return;

        if (body == target && body is IDamageable damageable)
        {
            hasHit = true;

            damageable.TakeDamage();

            QueueFree();
        }
    }
}