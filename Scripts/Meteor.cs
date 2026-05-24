using System;
using Godot;

public partial class Meteor : CharacterBody2D, IDamageable
{
    public event Action<Meteor> OnMeteorDestroyed;

    [Export] public float Speed = 250f;
    [Export] public float RotationSpeed = 1f;

    [Export] public PackedScene ScorePopupScene;

    [Export] public float StreakSpeedBonus = 6f;
    [Export] public float ComboSpeedBonus = 80f;
    [Export] public float MaxDynamicSpeed = 550f;

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

    private bool comboMode = false;

    protected bool hadMistake = false;

    public override void _Ready()
    {
        label =
            GetNodeOrNull<RichTextLabel>(
                "RichTextLabel"
            );

        sprite =
            GetNode<AnimatedSprite2D>(
                "AnimatedSprite2D"
            );

        sprite.Play("break");
        sprite.Stop();
        sprite.Frame = 0;

        originalScale = sprite.Scale;

        UpdateDisplay("");

        AdjustSizeToWord();

        baseSpritePos = sprite.Position;

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

    public override void _PhysicsProcess(double delta)
    {
        if (!IsInsideTree())
            return;

        if (target != null)
        {
            Vector2 dir =
                (target.GlobalPosition - GlobalPosition)
                .Normalized();

            if (
                sprite != null &&
                IsInstanceValid(sprite)
            )
            {
                sprite.Rotation +=
                    RotationSpeed * (float)delta;
            }

            float dynamicSpeed = Speed;

            if (turret != null)
            {
                int streak = turret.GetStreak();
                dynamicSpeed += streak * GetStreakSpeedBonus();

                if (turret.IsComboActive())
                {
                    dynamicSpeed += GetComboSpeedBonus();
                }

                dynamicSpeed =
                    Mathf.Min(dynamicSpeed, GetMaxSpeed());
            }

            Velocity = dir * dynamicSpeed;
        }

        MoveAndSlide();

        if (hasHit)
            return;

        for (
            int i = 0;
            i < GetSlideCollisionCount();
            i++
        )
        {
            var collision =
                GetSlideCollision(i);

            var collider =
                collision.GetCollider();

            if (collider is Turret turret)
            {
                hasHit = true;

                turret.TakeDamage(GetDamageToPlayer());

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

    public void SetTurret(Turret t)
    {
        turret = t;

        if (turret != null)
        {
            turret.OnComboStarted += EnableComboMode;
            turret.OnComboEnded += DisableComboMode;
        }
    }

    public void SetHadMistake(bool value)
    {
        hadMistake = value;
    }

    public void RegisterMistake()
    {
        hadMistake = true;
    }

    public virtual void TakeDamage()
    {
        if (isDead)
            return;

        hitsReceived =
            Mathf.Min(
                hitsReceived + 1,
                Word.Length
            );

        PlayHitAnimation();

        UpdateDamageFrame();

        if (hitsReceived >= Word.Length)
            Die();

        GetNode<AudioManager>("/root/AudioManager")
            .PlayMeteorDestroy();
    }

    public void Die()
    {
        if (isDead)
            return;

        isDead = true;

        int score = GetBaseScore();

        if (
            turret != null &&
            turret.IsComboActive()
        )
        {
            score *= 2;
        }

        turret?.AddScore(score);

        SpawnScorePopup(score);

        OnMeteorDestroyed?.Invoke(this);

        GetNode<AudioManager>("/root/AudioManager")
            .PlayMeteorDestroyed();

        QueueFree();
    }

    private void SpawnScorePopup(int amount)
    {

        if (ScorePopupScene == null)
        {
            GD.Print("NO SCENE");
            return;
        }

        Node popup =
            ScorePopupScene.Instantiate();


        if (popup is Node2D node2D)
        {
            node2D.GlobalPosition =
                GlobalPosition;

            GetTree()
                .CurrentScene
                .AddChild(node2D);

            GD.Print("POPUP ADDED");
        }

        if (popup is FloatingScore floatingScore)
        {

            Color color;

            if (amount >= 200)
                color = new Color(1f, 0.85f, 0.1f); 
            else if (amount >= 100)
                color = new Color(0.2f, 1f, 1f); 
            else
                color = new Color(1f, 0.6f, 0.2f); 

            floatingScore.Setup(
                $"+{amount}",
                color
            );
        }
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
                if (
                    !string.IsNullOrEmpty(DisplayWord)
                    && i < DisplayWord.Length
                )
                {
                    result += DisplayWord[i];
                }
                else
                {
                    result += Word[i];
                }
            }
        }

        label.Text = result;
    }

    private void PlayHitAnimation()
    {
        if (
            sprite == null ||
            !IsInstanceValid(sprite)
        )
            return;

        hitTween?.Kill();

        hitTween =
            GetTree().CreateTween();

        hitTween.TweenProperty(
            sprite,
            "modulate",
            new Color(1, 0.3f, 0.3f),
            0.05f
        );

        hitTween.TweenProperty(
            sprite,
            "modulate",
            comboMode
                ? new Color(1.2f, 1.2f, 1.5f)
                : Colors.White,
            0.1f
        );
    }

    public void PlayErrorShake()
    {
        if (
            sprite == null ||
            !IsInstanceValid(sprite)
        )
            return;

        RegisterMistake();

        shakeTween?.Kill();

        sprite.Position = baseSpritePos;

        shakeTween =
            GetTree().CreateTween();

        float strength = 8f;
        float time = 0.03f;

        shakeTween.TweenProperty(
            sprite,
            "position",
            baseSpritePos
                + new Vector2(-strength, 0),
            time
        );

        shakeTween.TweenProperty(
            sprite,
            "position",
            baseSpritePos
                + new Vector2(strength, 0),
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

        float textWidth =
            label.GetContentWidth();

        float textHeight =
            label.GetContentHeight();

        float horizontalPadding = 140f;
        float verticalPadding = 160f;

        Texture2D texture =
            sprite.SpriteFrames
                .GetFrameTexture(
                    "break",
                    0
                );

        float spriteWidth =
            texture.GetWidth()
            * originalScale.X;

        float spriteHeight =
            texture.GetHeight()
            * originalScale.Y;

        float widthScale =
            (textWidth + horizontalPadding)
            / spriteWidth;

        float heightScale =
            (textHeight + verticalPadding)
            / spriteHeight;

        float scaleMultiplier =
            Mathf.Max(
                widthScale,
                heightScale
            );

        scaleMultiplier =
            Mathf.Clamp(
                scaleMultiplier,
                1f,
                3f
            );

        sprite.Scale =
            originalScale
            * scaleMultiplier;
    }

    private void UpdateDamageFrame()
    {
        if (sprite == null)
            return;

        int totalFrames =
            sprite.SpriteFrames
                .GetFrameCount("break");

        float damagePercent =
            (float)hitsReceived
            / Word.Length;

        int frame =
            Mathf.Clamp(
                Mathf.FloorToInt(
                    damagePercent
                    * (totalFrames - 1)
                ),
                0,
                totalFrames - 1
            );

        sprite.Frame = frame;
    }

    private void EnableComboMode()
    {
        comboMode = true;

        if (
            sprite != null &&
            IsInstanceValid(sprite)
        )
        {
            sprite.Modulate =
                new Color(
                    1.2f,
                    1.2f,
                    1.5f
                );
        }
    }

    private void DisableComboMode()
    {
        comboMode = false;

        if (
            sprite != null &&
            IsInstanceValid(sprite)
        )
        {
            sprite.Modulate =
                Colors.White;
        }
    }

    protected virtual int GetDamageToPlayer()
    {
        return 1;
    }

    protected virtual int GetBaseScore()
    {
        return hadMistake ? 50 : 100;
    }

    protected bool HasMistake()
    {
        return hadMistake;
    }

    protected virtual float GetStreakSpeedBonus()
    {
        return StreakSpeedBonus;
    }

    protected virtual float GetComboSpeedBonus()
    {
        return ComboSpeedBonus;
    }

    protected virtual float GetMaxSpeed()
    {
        return MaxDynamicSpeed;
    }
}