using Godot;

public partial class Hud : CanvasLayer
{
	[Export] public Turret player;
	[Export] public StatsManager stats;

	private RichTextLabel scoreLabel;
	private RichTextLabel wpmLabel;
	private RichTextLabel accuracyLabel;
	private RichTextLabel timeLabel;
	private RichTextLabel comboLabel;

	private ColorRect criticalOverlay;
	private bool criticalActive = false;
	private float criticalTime = 0f;

	private TextureRect[] hearts;

	private int previousHp = -1;

	private Tween comboTween;
	private bool comboActive = false;
	private float comboTime = 0f;
	private Vector2 comboBasePosition;

	public override void _Ready()
	{
		scoreLabel = GetNode<RichTextLabel>("MarginContainer/Root/TopRight/ScoreLabel");
		wpmLabel = GetNode<RichTextLabel>("MarginContainer/Root/TopRight/WpmLabel");
		accuracyLabel = GetNode<RichTextLabel>("MarginContainer/Root/TopRight/AccuracyLabel");
		timeLabel = GetNode<RichTextLabel>("MarginContainer/Root/TopCenter/TimeLabel");
		comboLabel = GetNode<RichTextLabel>("MarginContainer/Root/TopCenter/ComboLabel");

		criticalOverlay = GetNode<ColorRect>("CriticalMode");
		criticalOverlay.Color = new Color(1f, 0f, 0f, 0f);
		criticalOverlay.MouseFilter = Control.MouseFilterEnum.Ignore;

		comboLabel.Text = "[center][wave amp=25 freq=7][color=#ffd84a]x2[/color][/wave][/center]";
		comboLabel.Visible = false;
		comboLabel.Modulate = new Color(1.4f, 1.2f, 0.4f, 0f);
		comboLabel.Scale = Vector2.One;
		comboLabel.PivotOffset = comboLabel.Size / 2f;

		comboBasePosition = comboLabel.Position;

		hearts = new TextureRect[]
		{
			GetNode<TextureRect>("MarginContainer/Root/TopLeft/LivesContainer/Heart1"),
			GetNode<TextureRect>("MarginContainer/Root/TopLeft/LivesContainer/Heart2"),
			GetNode<TextureRect>("MarginContainer/Root/TopLeft/LivesContainer/Heart3"),
			GetNode<TextureRect>("MarginContainer/Root/TopLeft/LivesContainer/Heart4"),
			GetNode<TextureRect>("MarginContainer/Root/TopLeft/LivesContainer/Heart5"),
		};

		if (player != null)
		{
			player.OnComboStarted += ShowCombo;
			player.OnComboEnded += HideCombo;

			player.OnCriticalStarted += ShowCritical;
			player.OnCriticalEnded += HideCritical;
		}
	}

	public override void _ExitTree()
	{
		if (player != null)
		{
			player.OnComboStarted -= ShowCombo;
			player.OnComboEnded -= HideCombo;

			player.OnCriticalStarted -= ShowCritical;
			player.OnCriticalEnded -= HideCritical;
		}
	}

	public override void _Process(double delta)
	{
		if (player == null) return;
		if (stats == null) return;

		wpmLabel.Text = ((int)stats.GetWPM()).ToString();
		accuracyLabel.Text = stats.GetAccuracy().ToString("0.0") + "%";
		timeLabel.Text = stats.GetTime();
		scoreLabel.Text = player.GetScore().ToString();

		if (comboActive)
			AnimateComboIdle((float)delta);

		if (criticalActive)
			AnimateCritical((float)delta);

		UpdateHearts();
	}

	private void AnimateComboIdle(float delta)
	{
		comboTime += delta;

		comboLabel.RotationDegrees = Mathf.Sin(comboTime * 6f) * 8f;

		float pulse = 1f + Mathf.Sin(comboTime * 8f) * 0.08f;
		comboLabel.Scale = new Vector2(pulse, pulse);

		float glow = 1.1f + Mathf.Sin(comboTime * 10f) * 0.25f;
		comboLabel.Modulate = new Color(
			glow,
			glow * 0.85f,
			0.25f,
			1f
		);
	}

	private void AnimateCritical(float delta)
	{
		criticalTime += delta;

		float pulse =
			(Mathf.Sin(criticalTime * 5f) + 1f)
			* 0.5f;

		float alpha =
			Mathf.Lerp(0.05f, 0.14f, pulse);

		criticalOverlay.Color =
			new Color(1f, 0f, 0f, alpha);
	}

	private void ShowCritical()
	{
		criticalActive = true;
		criticalTime = 0f;
	}

	private void HideCritical()
	{
		criticalActive = false;
		criticalOverlay.Color = new Color(1f, 0f, 0f, 0f);
	}

	private void ShowCombo()
	{
		comboTween?.Kill();

		comboActive = false;
		comboTime = 0f;

		comboLabel.Visible = true;
		comboLabel.Position = comboBasePosition + new Vector2(0, -30);
		comboLabel.RotationDegrees = -25f;
		comboLabel.Scale = new Vector2(0.2f, 0.2f);
		comboLabel.Modulate = new Color(1.8f, 1.4f, 0.2f, 0f);

		comboTween = GetTree().CreateTween();

		comboTween.TweenProperty(comboLabel, "modulate:a", 1f, 0.12f);

		comboTween.Parallel().TweenProperty(comboLabel, "scale", new Vector2(1.5f, 1.5f), 0.22f)
			.SetTrans(Tween.TransitionType.Back)
			.SetEase(Tween.EaseType.Out);

		comboTween.Parallel().TweenProperty(comboLabel, "position", comboBasePosition, 0.22f)
			.SetTrans(Tween.TransitionType.Back)
			.SetEase(Tween.EaseType.Out);

		comboTween.Parallel().TweenProperty(comboLabel, "rotation_degrees", 18f, 0.15f)
			.SetTrans(Tween.TransitionType.Sine)
			.SetEase(Tween.EaseType.Out);

		comboTween.TweenProperty(comboLabel, "rotation_degrees", -12f, 0.12f)
			.SetTrans(Tween.TransitionType.Sine)
			.SetEase(Tween.EaseType.InOut);

		comboTween.TweenProperty(comboLabel, "rotation_degrees", 6f, 0.1f)
			.SetTrans(Tween.TransitionType.Sine)
			.SetEase(Tween.EaseType.InOut);

		comboTween.TweenProperty(comboLabel, "rotation_degrees", 0f, 0.08f)
			.SetTrans(Tween.TransitionType.Sine)
			.SetEase(Tween.EaseType.Out);

		comboTween.TweenProperty(comboLabel, "scale", Vector2.One, 0.1f);

		comboTween.TweenCallback(Callable.From(() =>
		{
			comboActive = true;
		}));
	}

	private void HideCombo()
	{
		comboTween?.Kill();

		comboActive = false;

		comboTween = GetTree().CreateTween();

		comboTween.TweenProperty(comboLabel, "rotation_degrees", 25f, 0.15f)
			.SetTrans(Tween.TransitionType.Sine)
			.SetEase(Tween.EaseType.In);

		comboTween.Parallel().TweenProperty(comboLabel, "position", comboBasePosition + new Vector2(0, 90), 0.35f)
			.SetTrans(Tween.TransitionType.Back)
			.SetEase(Tween.EaseType.In);

		comboTween.Parallel().TweenProperty(comboLabel, "modulate:a", 0f, 0.35f);

		comboTween.Parallel().TweenProperty(comboLabel, "scale", new Vector2(0.7f, 0.7f), 0.35f)
			.SetTrans(Tween.TransitionType.Sine)
			.SetEase(Tween.EaseType.In);

		comboTween.TweenCallback(Callable.From(() =>
		{
			comboLabel.Visible = false;
			comboLabel.Position = comboBasePosition;
			comboLabel.Scale = Vector2.One;
			comboLabel.RotationDegrees = 0f;
		}));
	}

	private void UpdateHearts()
	{
		int hp = player.GetHealth();

		if (previousHp == -1)
		{
			for (int i = 0; i < hearts.Length; i++)
			{
				hearts[i].Visible = i < hp;
				hearts[i].Scale = Vector2.One;
				hearts[i].Modulate = Colors.White;
			}

			previousHp = hp;
			return;
		}

		if (hp > previousHp)
		{
			for (int i = previousHp; i < hp && i < hearts.Length; i++)
			{
				hearts[i].Visible = true;
				AnimateHeartGain(hearts[i]);
			}
		}
		else if (hp < previousHp)
		{
			for (int i = hp; i < previousHp && i < hearts.Length; i++)
			{
				AnimateHeartLoss(hearts[i]);
			}
		}

		for (int i = 0; i < hearts.Length; i++)
		{
			if (i < hp)
				hearts[i].Visible = true;
		}

		previousHp = hp;
	}

	private void AnimateHeartGain(TextureRect heart)
	{
		heart.Visible = true;
		heart.Scale = Vector2.One;
		heart.Modulate = new Color(0.4f, 1.8f, 0.6f, 1f);

		Tween tween = GetTree().CreateTween();

		tween.TweenProperty(
			heart,
			"modulate:a",
			0.15f,
			0.14f
		);

		tween.TweenProperty(
			heart,
			"modulate:a",
			1f,
			0.14f
		);

		tween.TweenProperty(
			heart,
			"modulate:a",
			0.15f,
			0.14f
		);

		tween.TweenProperty(
			heart,
			"modulate:a",
			1f,
			0.14f
		);

		tween.TweenProperty(
			heart,
			"modulate",
			Colors.White,
			0.22f
		);
	}

	private void AnimateHeartLoss(TextureRect heart)
	{
		heart.Visible = true;
		heart.Scale = Vector2.One;
		heart.Modulate = new Color(1.8f, 0.2f, 0.2f, 1f);

		Tween tween = GetTree().CreateTween();

		tween.TweenProperty(
			heart,
			"modulate:a",
			0.15f,
			0.14f
		);

		tween.TweenProperty(
			heart,
			"modulate:a",
			1f,
			0.14f
		);

		tween.TweenProperty(
			heart,
			"modulate:a",
			0.15f,
			0.14f
		);

		tween.TweenProperty(
			heart,
			"modulate:a",
			1f,
			0.14f
		);

		tween.TweenProperty(
			heart,
			"modulate:a",
			0f,
			0.22f
		);

		tween.TweenCallback(Callable.From(() =>
		{
			heart.Visible = false;
			heart.Scale = Vector2.One;
			heart.Modulate = Colors.White;
		}));
	}
}
