using Godot;

public partial class CameraShake : Camera2D
{
	[Export] public Turret turret;

	private float shakeTime = 0f;
	private float shakeStrength = 0f;

	private float time = 0f;

	// 🔥 estado combo
	private bool comboActive = false;

	// 🔥 zoom objetivo
	private float targetZoom = 1f;

	public override void _Ready()
	{
		if (turret != null)
		{
			turret.OnComboStarted += EnableComboMode;
			turret.OnComboEnded += DisableComboMode;
		}
	}

	public override void _ExitTree()
	{
		if (turret != null)
		{
			turret.OnComboStarted -= EnableComboMode;
			turret.OnComboEnded -= DisableComboMode;
		}
	}

	public override void _Process(double delta)
	{
		float dt = (float)delta;

		time += dt;

		int streak =
			(turret != null)
			? turret.GetStreak()
			: 0;

		float intensity =
			Mathf.Clamp(
				streak / 10f,
				0f,
				1f
			);

		// 🔥 shake normal
		if (shakeTime > 0)
		{
			shakeTime -= dt;

			float currentStrength =
				shakeStrength
				+ (
					1.5f
					+ intensity * 2.5f
				);

			Offset = new Vector2(
				(float)GD.RandRange(-1, 1),
				(float)GD.RandRange(-1, 1)
			) * currentStrength;
		}
		else
		{
			Offset = Vector2.Zero;
		}

		// 🔥 pulso combo
		if (comboActive)
		{
			float pulse =
				Mathf.Sin(time * 5f) * 0.015f;

			targetZoom =
				1.08f + pulse;
		}

		Zoom = Zoom.Lerp(
			new Vector2(targetZoom, targetZoom),
			4f * dt
		);
	}

	public void Shake(
		float strength,
		float duration
	)
	{
		shakeStrength = strength;
		shakeTime = duration;
	}

	private void EnableComboMode()
	{
		comboActive = true;

		targetZoom = 1.08f;
	}

	private void DisableComboMode()
	{
		comboActive = false;

		targetZoom = 1f;
	}
}
