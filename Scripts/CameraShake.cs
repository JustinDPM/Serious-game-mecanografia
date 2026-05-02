using Godot;

public partial class CameraShake : Camera2D
{
    private float shakeTime = 0f;
    private float shakeStrength = 0f;

    public override void _Process(double delta)
    {
      
        if (shakeTime > 0)
        {
            shakeTime -= (float)delta;

            Offset = new Vector2(
                (float)GD.RandRange(-1, 1),
                (float)GD.RandRange(-1, 1)
            ) * shakeStrength;
        }
        else
        {
            Offset = Vector2.Zero;
        }
    }

    public void Shake(float strength, float duration)
    {
        shakeStrength = strength;
        shakeTime = duration;
    }
}