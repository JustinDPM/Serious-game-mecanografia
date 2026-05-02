using System;
using Godot;

public partial class Meteor : CharacterBody2D, IDamageable
{

    public event Action<Meteor> OnMeteorDestroyed;

    [Export] public float Speed = 250f;
    [Export] public float RotationSpeed = 1f;

    public string Word = "";
    private int hitsReceived = 0;

    private RichTextLabel label;
    private Node2D target;
    private Sprite2D sprite;

    private bool hasHit = false;
    private bool isDead = false;

    private Turret turret;

    public override void _Ready()
    {
        label = GetNodeOrNull<RichTextLabel>("RichTextLabel");
        sprite = GetNode<Sprite2D>("Sprite2D");

        if (label != null)
            label.Text = Word;

    }

    public override void _PhysicsProcess(double delta)
    {
        if (!IsInsideTree())
            return;

        if (target != null)
        {
            Vector2 dir = (target.GlobalPosition - GlobalPosition).Normalized();

            if (sprite != null)
                sprite.Rotation += RotationSpeed * (float)delta;

            float dynamicSpeed = Speed;

            if (turret != null)
            {
                int streak = turret.GetStreak();
                dynamicSpeed += streak * 6f;

                dynamicSpeed = Mathf.Min(dynamicSpeed, 450f);
            }

            Velocity = dir * dynamicSpeed;
        }

        MoveAndSlide();

        if (hasHit) return;

        for (int i = 0; i < GetSlideCollisionCount(); i++)
        {
            var collision = GetSlideCollision(i);
            var collider = collision.GetCollider();

            if (collider is Turret turret)
            {
                hasHit = true;

                turret.TakeDamage(1);

                if (IsInsideTree())
                {
                    QueueFree();
                }
                return;
            }
        }
    }

    public void SetTarget(Node2D t)
    {
        target = t;
    }

    public void TakeDamage()
    {
        if (isDead) return;

        hitsReceived++;

        if (hitsReceived >= Word.Length)
            Die();
    }

    public void Die()
    {
        if (isDead) return;

        isDead = true;

        OnMeteorDestroyed?.Invoke(this);

        QueueFree();
    }

    public void UpdateDisplay(string input)
    {
        if (label == null) return;

        string result = "";

        for (int i = 0; i < Word.Length; i++)
        {
            if (i < input.Length && input[i] == Word[i])
                result += "[color=green]" + Word[i] + "[/color]";
            else if (i < input.Length)
                result += "[color=red]" + Word[i] + "[/color]";
            else
                result += Word[i];
        }

        label.Text = result;
    }

    public void SetTurret(Turret t)
    {
        turret = t;
    }

}