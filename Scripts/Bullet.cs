using Godot;

public partial class Bullet : Area2D
{
	[Export] public float Speed = 1900f;

	private Node2D target;

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

		// mover
		Position += dir * Speed * (float)delta;

		// 🔥 rotar hacia el meteorito
		Rotation = dir.Angle();
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
