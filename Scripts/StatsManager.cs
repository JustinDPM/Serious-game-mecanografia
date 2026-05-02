using Godot;

public partial class StatsManager : Node
{

    private float timeAlive = 0f;

    private int correct = 0;
    private int wrong = 0;
    private int words = 0;
    private int totalInputs = 0;

    [Export] public InputManager input;
    [Export] public Turret turret;
    [Export] public Global global;

    public override void _Ready()
    {
        if (global == null)
            global = GetNode<Global>("/root/Global");

        if (input == null)
        {
            GD.PrintErr("StatsManager: InputManager no asignado");
            return;
        }

        input.OnCorrectChar += OnCorrectChar;
        input.OnWrongChar += OnWrongChar;
        input.OnWordCompleted += OnWordCompleted;

        if (turret != null)
            turret.OnGameOver += SaveResults;
    }

    public override void _Process(double delta)
    {
        timeAlive += (float)delta;
    }

    public void OnCorrectChar()
    {
        correct++;
        totalInputs++;
    }

    public void OnWrongChar()
    {
        wrong++;
        totalInputs++;
    }

    public void OnWordCompleted()
    {
        words++;
    }

    public float GetWPM()
    {
        float minutes = timeAlive / 60f;
        if (minutes <= 0) return 0;

        return words / minutes;
    }

    public float GetAccuracy()
    {
        if (totalInputs == 0)
            return 100f;

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

        global.LastScore = turret.GetScore();
        global.LastAccuracy = GetAccuracy();
        global.LastWPM = GetWPM();

        GD.Print("=== GUARDANDO RESULTADOS ===");
        GD.Print("Score: " + turret.GetScore());
        GD.Print("Accuracy: " + GetAccuracy());
        GD.Print("WPM: " + GetWPM());
    }

    public void OnMeteorDestroyed(Meteor meteor)
    {
        if (turret == null) return;

        turret.AddScore(1);
        turret.addStreak();
    }
}