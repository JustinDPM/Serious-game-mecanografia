using Godot;

public partial class Turret : CharacterBody2D
{
	[Export] public PackedScene BulletScene;
	[Export] public Marker2D shootPoint;

	[Export] public int Health = 10;
	[Export] public int Score = 0;

	// 💫 float settings
	[Export] public float FloatAmplitude = 10f;
	[Export] public float FloatSpeed = 2f;

	private RichTextLabel healthLabel;
	private RichTextLabel scoreLabel;
	private Global global;



	private Vector2 startPosition;
	private float time;

	public override void _Ready()
	{
		healthLabel = GetNode<RichTextLabel>("/root/Level1/HealthLabel");
		scoreLabel = GetNode<RichTextLabel>("/root/Level1/ScoreLabel");
		global = GetNode<Global>("/root/Global");

		startPosition = Position;

		UpdateUI();
	}

	public override void _Process(double delta)
	{
   
		time += (float)delta;

		float offsetY = Mathf.Sin(time * FloatSpeed) * FloatAmplitude;

		Position = new Vector2(startPosition.X, startPosition.Y + offsetY);
	}

	public async void ShootBurst(Node2D target, int count)
	{
		for (int i = 0; i < count; i++)
		{
			var bullet = (Bullet)BulletScene.Instantiate();
			bullet.Position = shootPoint.GlobalPosition;
			bullet.SetTarget(target);

			GetTree().CurrentScene.AddChild(bullet);

			await ToSignal(GetTree().CreateTimer(0.05f), "timeout");
		}
	}

	public void TakeDamage(int dmg)
	{
		Health -= dmg;

		UpdateUI();

		if (Health <= 0)
			Die();
	}
	public void AddScore(int value)
	{
		Score += value;
		UpdateUI();
	}
	private void UpdateUI()
	{
		if (healthLabel != null)
			healthLabel.Text = "❤️ " + Health;

		if (scoreLabel != null)
			scoreLabel.Text = "" + Score;
	}


	private void Die()
	{
		GD.Print("💀 GAME OVER");

		SetProcess(false);
		SetPhysicsProcess(false);

		global.CambiarEscena("res://Escenas/main_menu.tscn");
	}
}
