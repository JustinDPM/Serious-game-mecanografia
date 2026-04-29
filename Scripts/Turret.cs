	using Godot;
	using System;

	public partial class Turret : CharacterBody2D
	{
		[Export] public float Speed = 300f;
		[Export] public PackedScene BulletScene;
		private Marker2D shootPoint;
		private int health = 10;
		private RichTextLabel healthLabel;

		public override void _Ready()
		{
			shootPoint = GetNode<Marker2D>("ShootPoint");
			healthLabel = GetNode<RichTextLabel>("/root/Level1/HealthLabel");
			UpdateUI();
		
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
			if (target != null)
			{
				// 1. El Player gira para mirar al objetivo
				LookAt(target.GlobalPosition);
			}
			bullet.SetTarget(target);
		
			GetTree().CurrentScene.AddChild(bullet);
		}

		public void TakeDamage(int amount)
		{
			health -= amount;
			UpdateUI();

	        GD.Print("Vida restante: " + health);

			if (health <= 0)
			{
				Die();
			}
		}

		private void Die()
		{
			GD.Print("Turret destruida 💀");
			QueueFree();
		}

		private void UpdateUI()
		{
			healthLabel.Text = "❤️ " + health;
		}
	}


