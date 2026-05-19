using Godot;

public partial class StatsManager : Node
{
    private float timeAlive = 0f;

    private int correct = 0;
    private int wrong = 0;
    private int words = 0;
    private int totalInputs = 0;

    [Export] public float GameDuration = 120f;

    private bool gameEnded = false;

    private InputManager input;
    private Turret turret;
    private Global global;

    public override void _Ready()
    {
        global = GetNode<Global>("/root/Global");

        input = GetNodeOrNull<InputManager>("../InputManager");

        turret = GetNodeOrNull<Turret>("../Ship/Turret");

        if (input == null)
        {
            GD.PrintErr("StatsManager: InputManager no encontrado.");
            return;
        }

        if (turret == null)
        {
            GD.PrintErr("StatsManager: Turret no encontrado.");
        }

        input.OnCorrectChar += OnCorrectChar;
        input.OnWrongChar += OnWrongChar;
        input.OnWordCompleted += OnWordCompleted;

        if (turret != null)
            turret.OnGameOver += EndGameByDeath;
    }

    public override void _ExitTree()
    {
        if (input != null)
        {
            input.OnCorrectChar -= OnCorrectChar;
            input.OnWrongChar -= OnWrongChar;
            input.OnWordCompleted -= OnWordCompleted;
        }

        if (turret != null)
            turret.OnGameOver -= EndGameByDeath;
    }

    public override void _Process(double delta)
    {
        if (gameEnded)
            return;

        timeAlive += (float)delta;

        if (timeAlive >= GameDuration)
        {
            EndGameByTime();
        }
    }

    public void OnCorrectChar()
    {
        if (gameEnded) return;

        correct++;
        totalInputs++;
    }

    public void OnWrongChar()
    {
        if (gameEnded) return;

        wrong++;
        totalInputs++;
    }

    public void OnWordCompleted()
    {
        if (gameEnded) return;

        words++;
    }

    public float GetWPM()
    {
        float minutes = timeAlive / 60f;

        return minutes <= 0
            ? 0
            : words / minutes;
    }

    public float GetAccuracy()
    {
        if (totalInputs == 0)
            return 100f;

        return (float)correct / totalInputs * 100f;
    }

    public string GetTime()
    {
        float remaining =
            Mathf.Max(
                GameDuration - timeAlive,
                0f
            );

        int m = (int)(remaining / 60);
        int s = (int)(remaining % 60);

        return $"{m:00}:{s:00}";
    }

    private void EndGameByTime()
    {
        if (gameEnded)
            return;

        gameEnded = true;

        SaveResults();

        GD.Print("TIEMPO TERMINADO");

        GetTree().ChangeSceneToFile(
            "res://Escenas/game_over.tscn"
        );
    }

    private void EndGameByDeath()
    {
        if (gameEnded)
            return;

        gameEnded = true;

        SaveResults();
    }

    private void SaveResults()
    {
        if (global == null || turret == null)
            return;

        global.LastScore = turret.GetScore();
        global.LastAccuracy = GetAccuracy();
        global.LastWPM = GetWPM();

        GD.Print("=== PARTIDA TERMINADA ===");
        GD.Print($"Score:    {global.LastScore}");
        GD.Print($"Accuracy: {global.LastAccuracy:0.0}%");
        GD.Print($"WPM:      {global.LastWPM:0.0}");
    }

    public void OnMeteorDestroyed(Meteor meteor)
    {
        if (gameEnded)
            return;

        if (turret == null)
            return;

        turret.AddStreak();
    }
}