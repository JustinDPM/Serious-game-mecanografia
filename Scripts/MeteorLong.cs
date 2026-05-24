using Godot;
using System;

public partial class MeteorLong : Meteor
{
    [Export] public int DamageToPlayer = 2;
    [Export] public float LongMeteorRotationSpeed = 0.3f;

    [Export] public float EntryBoostSpeed = 420f;
    [Export] public float EntryBoostDuration = 1.2f;

    private float originalSpeed;

    public override void _Ready()
    {
        RotationSpeed = LongMeteorRotationSpeed;

        originalSpeed = Speed;
        Speed = EntryBoostSpeed;

        base._Ready();

        CenterTextInsideMeteor();

        GetTree().CreateTimer(EntryBoostDuration).Timeout += () =>
        {
            if (!IsInstanceValid(this))
                return;

            Speed = originalSpeed;
        };
    }

    private void CenterTextInsideMeteor()
    {
        if (label == null || sprite == null)
            return;

        Texture2D texture =
            sprite.SpriteFrames.GetFrameTexture("break", 0);

        Vector2 realSpriteSize =
            texture.GetSize() * sprite.Scale;

        float safeWidth = realSpriteSize.X * 0.30f;
        float safeHeight = realSpriteSize.Y * 0.55f;

        label.Size = new Vector2(safeWidth, safeHeight);
        label.CustomMinimumSize = label.Size;

        label.Position = new Vector2(
            -safeWidth / 2f,
            -safeHeight / 2f
        );

        label.AutowrapMode = TextServer.AutowrapMode.WordSmart;
        label.FitContent = false;

        label.HorizontalAlignment = HorizontalAlignment.Center;
        label.VerticalAlignment = VerticalAlignment.Center;

        label.AddThemeConstantOverride(
            "line_separation",
            6
        );
    }

    protected override int GetDamageToPlayer()
    {
        return DamageToPlayer;
    }

    protected override int GetBaseScore()
    {
        return HasMistake()
            ? 250
            : 500;
    }
}