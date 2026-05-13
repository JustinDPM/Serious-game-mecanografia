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

    // 🔥 CONTROL DE DISPARO
    private bool isFiring = false;
    private float fireTimer = 0f;
    private const float fireHoldTime = 0.15f;

    public override void _Ready()
    {
        global = GetNode<Global>("/root/Global");

        input = GetNode<InputManager>("/root/Level1/InputManager");

        camera = GetTree().Root
            .GetNodeOrNull<CameraShake>("Level1/CameraShake");

        animatedSprite =
            GetNode<AnimatedSprite2D>("AnimatedSprite2D");

        targetRotation = -Mathf.Pi / 2;
        Rotation = targetRotation;

        if (input != null)
            input.OnShootRequested += EnqueueShot;

        animatedSprite.Play("shoot_end");
    }

    public override void _Process(double delta)
    {
        float rotationSpeed = 8f;

        // 🔥 ROTACIÓN SUAVE
        Rotation = Mathf.LerpAngle(
            Rotation,
            targetRotation,
            rotationSpeed * (float)delta
        );

        // 🔥 COOLDOWN DISPARO
        if (shootCooldown > 0)
            shootCooldown -= (float)delta;

        // 🔥 COLA DE DISPAROS
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

        // 🔥 CONTROLAR SI DEJÓ DE DISPARAR
        if (isFiring)
        {
            fireTimer -= (float)delta;

            if (fireTimer <= 0f)
            {
                isFiring = false;

                animatedSprite.Play("shoot_end");
            }
        }
    }

    private void EnqueueShot(Meteor target)
    {
        if (target == null || !IsInstanceValid(target))
            return;

        shootQueue.Enqueue(target);
    }

    public void Shoot(Node2D target)
    {
        // 🔥 PRIMER DISPARO
        if (!isFiring)
        {
            isFiring = true;

            animatedSprite.Play("shoot_start");
        }
        else
        {
            // 🔥 MANTENER RETRAÍDO
            if (animatedSprite.Animation != "shoot_hold")
                animatedSprite.Play("shoot_hold");
        }

        // 🔥 REINICIAR TIMER
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

        Blink();

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

    public int GetStreak()
    {
        return streak;
    }

    public int GetHealth()
    {
        return Health;
    }

    public int GetScore()
    {
        return Score;
    }

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
        var tween = GetTree().CreateTween();

        for (int i = 0; i < 3; i++)
        {
            tween.TweenProperty(
                animatedSprite,
                "modulate:a",
                0.4f,
                0.2f
            );

            tween.TweenProperty(
                animatedSprite,
                "modulate:a",
                1.0f,
                0.2f
            );
        }

        await ToSignal(tween, "finished");
    }

    private void AimAt(Node2D target)
    {
        Vector2 dir =
            (target.GlobalPosition - GlobalPosition)
            .Normalized();

        targetRotation = dir.Angle();
    }
}