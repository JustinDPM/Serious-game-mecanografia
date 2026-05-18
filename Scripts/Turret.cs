using Godot;
using System;
using System.Collections.Generic;

public partial class Turret : CharacterBody2D
{
	public event Action OnGameOver;

	[Export] public PackedScene BulletScene;
	[Export] public Marker2D shootPoint;

	private InputManager input;

	[Export] public int Health = 5;
	[Export] public int Score = 0;

	private Global global;
	private CameraShake camera;

	private int streak = 0;

	private Queue<Meteor> shootQueue = new Queue<Meteor>();

	private float shootCooldown = 0f;
	private const float cooldownTime = 0.10f;

	private float targetRotation;

	private AnimatedSprite2D animatedSprite;

	private bool isFiring = false;
	private float fireTimer = 0.2f;
	private const float fireHoldTime = 0.15f;

	private float time = 0f;

	public override void _Ready()
	{
		global = GetNode<Global>("/root/Global");
		input = GetNode<InputManager>("/root/Level1/InputManager");

		camera = GetTree().Root
			.GetNodeOrNull<CameraShake>("Level1/CameraShake");

		animatedSprite = GetNode<AnimatedSprite2D>("AnimatedSprite2D");

		targetRotation = -Mathf.Pi / 2;
		Rotation = targetRotation;

		if (input != null)
			input.OnShootRequested += EnqueueShot;

		animatedSprite.Play("shoot_end");
	}

	public override void _Process(double delta)
	{
		float dt = (float)delta;

		Rotation = Mathf.LerpAngle(Rotation, targetRotation, 8f * dt);

		if (shootCooldown > 0)
			shootCooldown -= dt;

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

		if (isFiring)
		{
			fireTimer -= dt;

			if (fireTimer <= 0f)
			{
				isFiring = false;
				animatedSprite.Play("shoot_end");
			}
		}

		time += dt;

		int s = GetStreak();

		float intensity = Mathf.Clamp(s / 10f, 0f, 1f);

		float flickerSpeed = (s >= 10) ? 25f : 8f;
		float flickerAmount = (s >= 10) ? 0.25f : 0.08f;

		float flicker = Mathf.Sin(time * flickerSpeed) * flickerAmount;

		float glow = intensity + flicker;

		float brightness = 1f + glow * 0.35f;

		animatedSprite.Modulate = new Color(
			brightness,
			brightness,
			brightness
		);
	}

	private void EnqueueShot(Meteor target)
	{
		if (target == null || !IsInstanceValid(target))
			return;

		shootQueue.Enqueue(target);
	}

	public void Shoot(Node2D target)
	{
		if (!isFiring)
		{
			isFiring = true;
			animatedSprite.Play("shoot_start");
		}
		else
		{
			if (animatedSprite.Animation != "shoot_hold")
				animatedSprite.Play("shoot_hold");
		}

		fireTimer = fireHoldTime;

		var bullet = (Bullet)BulletScene.Instantiate();
		bullet.GlobalPosition = shootPoint.GlobalPosition;
		bullet.SetTarget(target);

		GetTree().CurrentScene.AddChild(bullet);
	}

	public void TakeDamage(int dmg)
	{
		Health -= dmg;

		streak = 0;

		camera?.Shake(8f, 0.2f);

		if (Health <= 0)
			Die();
	}

	public void AddScore(int value)
	{
		Score += (streak >= 10)
			? value * 2
			: value;
	}

	public void addStreak()
	{
		streak++;
	}

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

	private void AimAt(Node2D target)
	{
		Vector2 dir = (target.GlobalPosition - GlobalPosition).Normalized();
		targetRotation = dir.Angle();
	}
}
