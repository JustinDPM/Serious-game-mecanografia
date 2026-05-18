using System;
using Godot;

public partial class Meteor : CharacterBody2D, IDamageable
{
	public event Action<Meteor> OnMeteorDestroyed;

	[Export] public float Speed = 250f;
	[Export] public float RotationSpeed = 1f;

	private Tween hitTween;
	private Tween shakeTween;

	public string Word = "";
	public string DisplayWord = "";

	protected int hitsReceived = 0;

	protected RichTextLabel label;
	protected Node2D target;
	protected AnimatedSprite2D sprite;

	private bool hasHit = false;
	protected bool isDead = false;

	protected Turret turret;

	private Vector2 baseSpritePos;

	private Vector2 originalScale;

	protected string currentDisplayInput = "";

	public override void _Ready()
	{
		label = GetNodeOrNull<RichTextLabel>("RichTextLabel");
		sprite = GetNode<AnimatedSprite2D>("AnimatedSprite2D");

		originalScale = sprite.Scale;

		UpdateDisplay("");

		AdjustSizeToWord();

		baseSpritePos = sprite.Position;
	}

	public override void _PhysicsProcess(double delta)
	{
		if (!IsInsideTree())
			return;

		if (target != null)
		{
			Vector2 dir =
				(target.GlobalPosition - GlobalPosition)
				.Normalized();

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

		if (hasHit)
			return;

		for (int i = 0; i < GetSlideCollisionCount(); i++)
		{
			var collision = GetSlideCollision(i);
			var collider = collision.GetCollider();

			if (collider is Turret turret)
			{
				hasHit = true;

				turret.TakeDamage(1);

				if (IsInsideTree())
					QueueFree();

				return;
			}
		}
	}

	public void SetTarget(Node2D t)
	{
		target = t;
	}

	public virtual void TakeDamage()
	{
		if (isDead)
			return;

		hitsReceived++;

		PlayHitAnimation();

		UpdateDamageFrame();

		if (hitsReceived >= Word.Length)
			Die();
	}

	public void Die()
	{
		if (isDead)
			return;

		isDead = true;

		OnMeteorDestroyed?.Invoke(this);

		QueueFree();
	}

	public virtual void UpdateDisplay(string input)
	{
		if (label == null)
			return;

		currentDisplayInput = input;

		string result = "";

		for (int i = 0; i < Word.Length; i++)
		{

			if (
				i < input.Length &&
				input[i] == Word[i]
			)
			{
				result +=
					"[color=green]" +
					Word[i] +
					"[/color]";
			}

 
			else if (i < input.Length)
			{
				result +=
					"[color=red]" +
					Word[i] +
					"[/color]";
			}

			else
			{
				if (!string.IsNullOrEmpty(DisplayWord))
					result += DisplayWord[i];
				else
					result += Word[i];
			}
		}

		label.Text = result;
	}

	public void SetTurret(Turret t)
	{
		turret = t;
	}

	private void PlayHitAnimation()
	{
		if (sprite == null)
			return;

		hitTween?.Kill();

		hitTween = GetTree().CreateTween();

		hitTween.TweenProperty(
			sprite,
			"modulate",
			new Color(1, 0.3f, 0.3f),
			0.05f
		);

		hitTween.TweenProperty(
			sprite,
			"modulate",
			new Color(1, 1, 1),
			0.1f
		);
	}

	public void PlayErrorShake()
	{
		if (sprite == null)
			return;

		shakeTween?.Kill();

		sprite.Position = baseSpritePos;

		shakeTween = GetTree().CreateTween();

		float strength = 8f;
		float time = 0.03f;

		shakeTween.TweenProperty(
			sprite,
			"position",
			baseSpritePos + new Vector2(-strength, 0),
			time
		);

		shakeTween.TweenProperty(
			sprite,
			"position",
			baseSpritePos + new Vector2(strength, 0),
			time
		);

		shakeTween.TweenProperty(
			sprite,
			"position",
			baseSpritePos + new Vector2(-strength * 0.5f, 0),
			time
		);

		shakeTween.TweenProperty(
			sprite,
			"position",
			baseSpritePos + new Vector2(strength * 0.5f, 0),
			time
		);

		shakeTween.TweenProperty(
			sprite,
			"position",
			baseSpritePos,
			time
		);
	}

	private void AdjustSizeToWord()
	{
		if (label == null || sprite == null)
			return;

		label.Text =
			string.IsNullOrEmpty(DisplayWord)
			? Word
			: DisplayWord;

		float textWidth = label.GetContentWidth();
		float textHeight = label.GetContentHeight();

		float horizontalPadding = 140f;
		float verticalPadding = 160f;

		Texture2D texture =
			sprite.SpriteFrames.GetFrameTexture(
				"break",
				0
			);

		float spriteWidth =
			texture.GetWidth() * originalScale.X;

		float spriteHeight =
			texture.GetHeight() * originalScale.Y;

		float widthScale =
			(textWidth + horizontalPadding) / spriteWidth;

		float heightScale =
			(textHeight + verticalPadding) / spriteHeight;

		float scaleMultiplier =
			Mathf.Max(widthScale, heightScale);

		scaleMultiplier = Mathf.Clamp(scaleMultiplier, 1f, 3f);

		sprite.Scale =
			originalScale * scaleMultiplier;
	}

	private void UpdateDamageFrame()
	{
		if (sprite == null)
			return;

		int totalFrames = sprite.SpriteFrames
			.GetFrameCount("break");

		float damagePercent =
			(float)hitsReceived / Word.Length;

		int frame =
			Mathf.FloorToInt(
				damagePercent * (totalFrames - 1)
			);

		sprite.Frame = frame;
	}
}
