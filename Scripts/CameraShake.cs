using Godot;

public partial class CameraShake : Camera2D
{
    [Export] public Turret turret;

    private float shakeTime = 0f;
    private float shakeStrength = 0f;

    private float time = 0f;

    public override void _Process(double delta)
    {
        float dt = (float)delta;
        time += dt;

        int streak = (turret != null) ? turret.GetStreak() : 0;

        float intensity = Mathf.Clamp(streak / 10f, 0f, 1f);

        // 💥 SHAKE
        if (shakeTime > 0)
        {
            shakeTime -= dt;

            float currentStrength = shakeStrength + (1.5f + intensity * 2.5f);

            Offset = new Vector2(
                (float)GD.RandRange(-1, 1),
                (float)GD.RandRange(-1, 1)
            ) * currentStrength;
        }
        else
        {
            Offset = Vector2.Zero;
        }

        // 🎯 ZOOM SOLO EN STREAK >= 10
        float targetZoom = 1f;

        if (streak >= 10)
        {
            targetZoom = 1f + intensity * 0.08f;
        }

        Zoom = Zoom.Lerp(
            new Vector2(targetZoom, targetZoom),
            4f * dt
        );
    }

    // 💥 shake normal
    public void Shake(float strength, float duration)
    {
        shakeStrength = strength;
        shakeTime = duration;
    }
}