using Godot;
using System;

public partial class Ship : Node2D
{
    [Export] public float FloatAmplitude = 5f;
    [Export] public float FloatSpeed = 2f;

    [Export] public Turret turret;

    private Vector2 startPosition;
    private float time;

    private AnimatedSprite2D flame;
    private Sprite2D sprite;

    public override void _Ready()
    {
        startPosition = Position;

        sprite = GetNode<Sprite2D>("Sprite2D");
        flame = GetNode<AnimatedSprite2D>("Flame/AnimatedSprite2D");

        flame.Play("idle");
    }

    public override void _Process(double delta)
    {
        time += (float)delta;

        if (turret == null || sprite == null)
            return;

        int streak = turret.GetStreak();

        // 🌊 flotación base
        float floatOffset = Mathf.Sin(time * FloatSpeed) * FloatAmplitude;

        Vector2 basePos = new Vector2(
            startPosition.X,
            startPosition.Y + floatOffset
        );

        // ⚡ intensidad (0 - 1)
        float intensity = Mathf.Clamp(streak / 10f, 0f, 1f);

        // 💥 movimiento inestable en overdrive
        if (streak >= 10)
        {
            float shakeX = Mathf.Sin(time * 30f) * (1.5f + intensity * 2f);
            float shakeY = Mathf.Cos(time * 34f) * (1.5f + intensity * 2f);

            basePos += new Vector2(shakeX, shakeY);
        }

        // ✨ PARPADEO (Flicker)
        float flickerSpeed = (streak >= 10) ? 25f : 8f;
        float flicker = Mathf.Sin(time * flickerSpeed);

        float flickerAmount = (streak >= 10) ? 0.25f : 0.08f;

        float flickerOffset = flicker * flickerAmount;

        // 💡 glow base
        float pulse = Mathf.Sin(time * 12f) * 0.1f;
        float glow = intensity + pulse + flickerOffset;

        float brightness = 1f + glow * 0.5f;

        sprite.Modulate = new Color(
            brightness,
            brightness,
            brightness
        );

        // 🔥 FLAMA (SIN ESCALA)
        if (flame != null)
        {
            // velocidad de animación
            flame.SpeedScale = 1f + intensity * 2.5f;

            // 💜 color morado energético
            flame.Modulate = new Color(
                1f + intensity * 0.25f,
                0.5f + intensity * 0.2f,
                1f + intensity * 0.7f
            );
        }

        Position = basePos;
    }
}