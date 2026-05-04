using Godot;
using System;
using System.Collections.Generic;

public partial class Turret : CharacterBody2D
{
	public event Action OnGameOver;

	[Export] public PackedScene BulletScene;
	[Export] public Marker2D shootPoint;
	[Export] public InputManager input;

	[Export] public int Health = 5;
	[Export] public int Score = 0;

	[Export] public float FloatAmplitude = 10f;
	[Export] public float FloatSpeed = 2f;

	private Global global;
	private CameraShake camera;

	private int streak = 0;

	private Vector2 startPosition;
	private float time;

	private Queue<Meteor> shootQueue = new Queue<Meteor>();

	private float shootCooldown = 0f;
	private const float cooldownTime = 0.05f;

	private float targetRotation;

	public override void _Ready()
	{
		global = GetNode<Global>("/root/Global");
		camera = GetTree().Root.GetNodeOrNull<CameraShake>("Level1/CameraShake");

		startPosition = Position;
		targetRotation = -Mathf.Pi / 2;
		Rotation = targetRotation;

		if (input != null)
			input.OnShootRequested += EnqueueShot;


	}

	public override void _Process(double delta)
	{
		time += (float)delta;
		float rotationSpeed = 8f; 

		Rotation = Mathf.LerpAngle(Rotation, targetRotation, rotationSpeed * (float)delta);


		float offsetY = Mathf.Sin(time * FloatSpeed) * FloatAmplitude;
		Position = new Vector2(startPosition.X, startPosition.Y + offsetY);

		if (shootCooldown > 0)
			shootCooldown -= (float)delta;

		if (shootQueue.Count > 0 && shootCooldown <= 0f)
		{
			var target = shootQueue.Dequeue();

			if (target != null && IsInstanceValid(target))
			{
				AimAt(target);
				Shoot(target);
				shootCooldown = cooldownTime;
			}
		}
	}

	private void EnqueueShot(Meteor target)
	{
		if (target == null || !IsInstanceValid(target)) return;

		shootQueue.Enqueue(target);
	}

	public void Shoot(Node2D target)
	{
		var bullet = (Bullet)BulletScene.Instantiate();
		bullet.Position = shootPoint.GlobalPosition;
		bullet.SetTarget(target);

		GetTree().CurrentScene.AddChild(bullet);
	}

	public void TakeDamage(int dmg)
	{
		Health -= dmg;
		streak = 0;

		Blink();
		camera?.Shake(8f, 0.2f);

		if (Health <= 0)
			Die();
	}

	public void AddScore(int value)
	{
		Score += (streak >= 10) ? value * 2 : value;
	}

	public void addStreak() => streak++;
	public int GetStreak() => streak;

	public int GetHealth() => Health;
	public int GetScore() => Score;

	private void Die()
	{
		GD.Print("GAME OVER");

		camera?.Shake(12f, 0.3f);

		SetProcess(false);
		SetPhysicsProcess(false);

		OnGameOver?.Invoke();
	}

	private async void Blink()
	{
		var sprite = GetNode<Sprite2D>("Sprite2D");
		var tween = GetTree().CreateTween();

		for (int i = 0; i < 3; i++)
		{
			tween.TweenProperty(sprite, "modulate:a", 0.4f, 0.2f);
			tween.TweenProperty(sprite, "modulate:a", 1.0f, 0.2f);
		}

		await ToSignal(tween, "finished");
	}

	private void AimAt(Node2D target)
	{
		Vector2 dir = (target.GlobalPosition - GlobalPosition).Normalized();
		targetRotation = dir.Angle();
	}
}
