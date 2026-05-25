using Godot;

public partial class Background : Node2D
{
	[Export] public float BaseSpeed = 200f;
	[Export] public float MaxSpeedBoost = 300f;
	[Export] public float Acceleration = 0.08f;

	[Export] public Texture2D BackgroundTexture;

	[Export]
	public Vector2 BackgroundScale =
		new Vector2(1.9f, 1f);

	[Export] private Turret turret;

	private Sprite2D bg1;
	private Sprite2D bg2;
	private Sprite2D bg3;

	private GpuParticles2D stars1;
	private GpuParticles2D stars2;
	private GpuParticles2D stars3;

	private float height;

	private int baseAmount1;
	private int baseAmount2;

	private bool comboActive = false;

	public override void _Ready()
	{
		bg1 = GetNode<Sprite2D>("BG1");
		bg2 = GetNode<Sprite2D>("BG2");
		bg3 = GetNode<Sprite2D>("BG3");



		stars1 =
			GetNode<GpuParticles2D>("Stars");

		stars2 =
			GetNode<GpuParticles2D>("BlueStars");

		stars3 =
			GetNode<GpuParticles2D>("Stars3");

		stars1.Preprocess = 12f;
		stars2.Preprocess = 12f;
		stars3.Preprocess = 12f;

		if (BackgroundTexture != null)
		{
			bg1.Texture = BackgroundTexture;
			bg2.Texture = BackgroundTexture;
			bg3.Texture = BackgroundTexture;
		}

		bg1.Scale = BackgroundScale;
		bg2.Scale = BackgroundScale;
		bg3.Scale = BackgroundScale;

		height =
			bg1.Texture.GetHeight()
			* bg1.Scale.Y;

		bg1.Position = new Vector2(0, 0);

		bg2.Position =
			new Vector2(0, -height);

		bg3.Position =
			new Vector2(0, -height * 2);

		baseAmount1 = stars1.Amount;
		baseAmount2 = stars2.Amount;

		// 🔥 conectar eventos
		if (turret != null)
		{
			turret.OnComboStarted += EnableHyperMode;
			turret.OnComboEnded += DisableHyperMode;
		}
	}

	public override void _ExitTree()
	{
		// 🔥 desconectar eventos
		if (turret != null)
		{
			turret.OnComboStarted -= EnableHyperMode;
			turret.OnComboEnded -= DisableHyperMode;
		}
	}

	public override void _Process(double delta)
	{
		int streak =
			turret != null
			? turret.GetStreak()
			: 0;

		float speedBoost =
			(1 - Mathf.Exp(-streak * Acceleration))
			* MaxSpeedBoost;

		float dynamicSpeed =
			BaseSpeed + speedBoost;

		float move =
			dynamicSpeed * (float)delta;

		bg1.Position += new Vector2(0, move);
		bg2.Position += new Vector2(0, move);
		bg3.Position += new Vector2(0, move);

		Loop(bg1);
		Loop(bg2);
		Loop(bg3);

		float speedFactor =
			dynamicSpeed / BaseSpeed;

		if (stars1 != null)
			stars1.SpeedScale =
				1.2f * speedFactor;

		if (stars2 != null)
			stars2.SpeedScale =
				0.7f * speedFactor;

		if (comboActive && stars3 != null)
		{
			stars3.SpeedScale =
				1.5f * speedFactor;
		}
	}

	private void EnableHyperMode()
	{
		comboActive = true;

		if (stars1 != null)
			stars1.Amount = baseAmount1 / 2;

		if (stars2 != null)
			stars2.Amount = baseAmount2 / 2;

		if (stars3 != null)
			stars3.Emitting = true;
	}

	private void DisableHyperMode()
	{
		comboActive = false;

		if (stars1 != null)
			stars1.Amount = baseAmount1;

		if (stars2 != null)
			stars2.Amount = baseAmount2;

		if (stars3 != null)
			stars3.Emitting = false;
	}

	private void Loop(Sprite2D bg)
	{
		if (bg.Position.Y >= height)
		{
			float highest = Mathf.Min(
				bg1.Position.Y,
				Mathf.Min(
					bg2.Position.Y,
					bg3.Position.Y
				)
			);

			bg.Position = new Vector2(
				0,
				highest - height
			);
		}
	}
}
