using Godot;
using System;

public partial class Player : CharacterBody2D
{
	[Export] public float Speed = 300f;

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
}
