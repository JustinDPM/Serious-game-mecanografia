using Godot;
using System;

public partial class MeteorLong : Meteor
{
    [Export] public int DamageToPlayer = 2;

    [Export] public float LongMeteorRotationSpeed = 0f;

    [Export] public float EntryBoostSpeed = 420f;
    [Export] public float EntryBoostDuration = 1.2f;

    [Export] public int LongSmokeAmount = 6;
    [Export] public int LongComboSmokeAmount = 12;

    [Export] public float LongSmokeSpeedScale = 0.35f;
    [Export] public float LongComboSmokeSpeedScale = 0.55f;

    private float originalSpeed;

    public override void _Ready()
    {
        RotationSpeed = LongMeteorRotationSpeed;

        originalSpeed = Speed;
        Speed = EntryBoostSpeed;

        base._Ready();

        if (smokeTrail != null)
        {
            smokeTrail.Scale = Vector2.One;
            smokeTrail.Rotation = 0f;
        }

        CenterTextInsideMeteor();

        GetTree().CreateTimer(
            EntryBoostDuration
        ).Timeout += () =>
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
            sprite.SpriteFrames.GetFrameTexture(
                "break",
                0
            );

        Vector2 realSpriteSize =
            texture.GetSize() * sprite.Scale;

        float safeWidth =
            realSpriteSize.X * 0.30f;

        float safeHeight =
            realSpriteSize.Y * 0.55f;

        label.Size =
            new Vector2(
                safeWidth,
                safeHeight
            );

        label.CustomMinimumSize =
            label.Size;

        label.Position =
            new Vector2(
                -safeWidth / 2f,
                -safeHeight / 2f
            );

        label.AutowrapMode =
            TextServer.AutowrapMode.WordSmart;

        label.FitContent = false;

        label.HorizontalAlignment =
            HorizontalAlignment.Center;

        label.VerticalAlignment =
            VerticalAlignment.Center;

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

    protected override int GetSmokeAmount()
    {
        return LongSmokeAmount;
    }

    protected override int GetComboSmokeAmount()
    {
        return LongComboSmokeAmount;
    }

    protected override float GetSmokeSpeedScale()
    {
        return LongSmokeSpeedScale;
    }

    protected override float GetComboSmokeSpeedScale()
    {
        return LongComboSmokeSpeedScale;
    }

    protected override void UpdateSmokeTrail()
    {
        if (smokeTrail == null)
            return;

        if (
            turret != null &&
            turret.IsComboActive()
        )
        {
            smokeTrail.Amount =
                GetComboSmokeAmount();

            smokeTrail.SpeedScale =
                GetComboSmokeSpeedScale();
        }
        else
        {
            smokeTrail.Amount =
                GetSmokeAmount();

            smokeTrail.SpeedScale =
                GetSmokeSpeedScale();
        }

        if (
            smokeTrail.ProcessMaterial
                is ParticleProcessMaterial material
        )
        {
            material.Gravity = Vector3.Zero;
            material.Direction = new Vector3(0f, -1f, 0f);
        }
    }
}