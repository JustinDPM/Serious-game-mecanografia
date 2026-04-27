using Godot;
using System;

public partial class Bullet : Area2D
{
	[Export] public float Speed = 1500f;
	private Node2D target;
	
	public override void _Ready()
	{
		BodyEntered += OnBodyEntered;
	}
	
	public void SetTarget(Node2D meteor)
	{
		target = meteor;
	}

	public override void _PhysicsProcess(double delta)
	{
	   if (target != null)
		{
			Vector2 direction = (target.GlobalPosition - GlobalPosition).Normalized();
			Position += direction * Speed * (float)delta;
		}
		else
		{
			Position += new Vector2(0, -Speed * (float)delta);
		}

		if (target == null){
			QueueFree();
		}

	}
	private void OnBodyEntered(Node2D body){
		
		if (body is Meteor meteor){
			
			meteor.TakeDamage();
			QueueFree();
			
		}
	}	
}
