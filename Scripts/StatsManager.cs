using Godot;
using System;
using System.Collections.Generic;

public class TypingMistake
{
    public string Word;
    public char ExpectedChar;
    public char TypedChar;
    public int Position;
}

public partial class StatsManager : Node
{
    private float timeAlive = 0f;

    private int correct = 0;
    private int wrong = 0;
    private int totalInputs = 0;

    private int correctChars = 0;

    [Export] public float GameDuration = 120f;

    private bool gameEnded = false;

    private InputManager input;
    private Turret turret;
    private Global global;

    private List<TypingMistake> mistakes =
        new List<TypingMistake>();

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
        input.OnTypingMistake += OnTypingMistake;

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
            input.OnTypingMistake -= OnTypingMistake;
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
        if (gameEnded)
            return;

        correct++;
        correctChars++;
        totalInputs++;
    }

    public void OnWrongChar()
    {
        if (gameEnded)
            return;

        wrong++;
        totalInputs++;
    }

    private void OnTypingMistake(
        string word,
        char expectedChar,
        char typedChar,
        int position
    )
    {
        if (gameEnded)
            return;

        mistakes.Add(
            new TypingMistake
            {
                Word = word,
                ExpectedChar = expectedChar,
                TypedChar = typedChar,
                Position = position
            }
        );
    }

    public void OnWordCompleted(Meteor meteor)
    {
        if (gameEnded)
            return;
    }

    public float GetWPM()
    {
        float minutes = timeAlive / 60f;

        if (minutes <= 0)
            return 0;

        float estimatedWords = correctChars / 5f;

        return estimatedWords / minutes;
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

        GetTree().ChangeSceneToFile(
            "res://Escenas/game_over.tscn"
        );
    }

    private void SaveResults()
    {
        if (global == null || turret == null)
            return;

        global.LastScore = turret.GetScore();
        global.LastAccuracy = GetAccuracy();
        global.LastWPM = GetWPM();

        global.MatchHistory.Add(
            new MatchResult
            {
                Score = global.LastScore,
                Accuracy = global.LastAccuracy,
                WPM = global.LastWPM,
                Duration = GetTime(),
                LevelName = GetTree().CurrentScene.Name
            }
        );

        GD.Print("=== PARTIDA TERMINADA ===");
        GD.Print($"Score:    {global.LastScore}");
        GD.Print($"Accuracy: {global.LastAccuracy:0.0}%");
        GD.Print($"WPM:      {global.LastWPM:0.0}");
        GD.Print($"Correct chars: {correctChars}");
        GD.Print($"Partidas guardadas: {global.MatchHistory.Count}");

        GD.Print("=== ERRORES DE TIPEO ===");

        if (mistakes.Count == 0)
        {
            GD.Print("Sin errores registrados.");
            return;
        }

        foreach (TypingMistake mistake in mistakes)
        {
            GD.Print(
                $"Palabra: {mistake.Word} | " +
                $"Esperada: {mistake.ExpectedChar} | " +
                $"Escrita: {mistake.TypedChar} | " +
                $"Posición: {mistake.Position}"
            );
        }
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