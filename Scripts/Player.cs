using Godot;
using System;

public partial class Player : CharacterBody2D
{
	[Export] public float Speed = 300f;
	[Export] public PackedScene BulletScene;
	private Marker2D shootPoint;

	public override void _Ready()
	{
		shootPoint = GetNode<Marker2D>("ShootPoint");
	}
	public override void _PhysicsProcess(double delta)
	{
		float direction = 0;

		if (Input.IsActionPressed("ui_left"))
			direction -= 1;

		if (Input.IsActionPressed("ui_right"))
			direction += 1;

		Velocity = new Vector2(direction * Speed, 0);
		MoveAndSlide();
	}
	public void Shoot(Node2D target)
	{
		var bullet = (Bullet)BulletScene.Instantiate();
		bullet.Position = shootPoint.GlobalPosition;
		
		bullet.SetTarget(target);
		
		GetTree().CurrentScene.AddChild(bullet);
	}
}
