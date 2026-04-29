using Godot;

public partial class Bullet : Area2D
{
    [Export] public float Speed = 1500f;

    private Node2D target;

    public override void _PhysicsProcess(double delta)
    {
        if (target == null || !IsInstanceValid(target))
        {
            QueueFree();
            return;
        }

        Vector2 dir = (target.GlobalPosition - GlobalPosition).Normalized();
        Position += dir * Speed * (float)delta;
    }

    public void SetTarget(Node2D t)
    {
        target = t;
    }

    private void OnBodyEntered(Node2D body)
    {
        if (body is Meteor meteor)
        {
            meteor.TakeDamage();
            QueueFree();
        }
    }
}