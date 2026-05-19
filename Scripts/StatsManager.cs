using Godot;
 
public partial class StatsManager : Node
{
	private float timeAlive   = 0f;
	private int   correct     = 0;
	private int   wrong       = 0;
	private int   words       = 0;
	private int   totalInputs = 0;
 
	// Ya no son [Export] — los buscamos por código
	private InputManager input;
	private Turret       turret;
	private Global       global;
 
	public override void _Ready()
	{
		global = GetNode<Global>("/root/Global");
 
		// Busca InputManager en la escena
		input  = GetNodeOrNull<InputManager>("../InputManager");
 
		// Busca Turret dentro de Ship
		turret = GetNodeOrNull<Turret>("../Ship/Turret");
 
		if (input == null)
		{
			GD.PrintErr("StatsManager: InputManager no encontrado.");
			return;
		}
 
		if (turret == null)
			GD.PrintErr("StatsManager: Turret no encontrado.");
 
		input.OnCorrectChar   += OnCorrectChar;
		input.OnWrongChar     += OnWrongChar;
		input.OnWordCompleted += OnWordCompleted;
 
		if (turret != null)
			turret.OnGameOver += SaveResults;
	}
 
	public override void _Process(double delta)
	{
		timeAlive += (float)delta;
	}
 
	public void OnCorrectChar()   { correct++;  totalInputs++; }
	public void OnWrongChar()     { wrong++;    totalInputs++; }
	public void OnWordCompleted() { words++; }
 
	public float GetWPM()
	{
		float minutes = timeAlive / 60f;
		return minutes <= 0 ? 0 : words / minutes;
	}
 
	public float GetAccuracy()
	{
		if (totalInputs == 0) return 100f;
		return (float)correct / totalInputs * 100f;
	}
 
	public string GetTime()
	{
		int m = (int)(timeAlive / 60);
		int s = (int)(timeAlive % 60);
		return $"{m:00}:{s:00}";
	}
 
	private void SaveResults()
	{
		if (global == null || turret == null) return;
 
		global.LastScore    = turret.GetScore();
		global.LastAccuracy = GetAccuracy();
		global.LastWPM      = GetWPM();
 
		GD.Print($"=== PARTIDA TERMINADA ===");
		GD.Print($"Score:    {global.LastScore}");
		GD.Print($"Accuracy: {global.LastAccuracy:0.0}%");
		GD.Print($"WPM:      {global.LastWPM:0.0}");
	}
 
	public void OnMeteorDestroyed(Meteor meteor)
	{
		if (turret == null) return;
		turret.AddScore(1);
		turret.addStreak();
	}
}
