using Godot;
using System;

public partial class InputManager : Node
{
    public event Action OnCorrectChar;
    public event Action OnWrongChar;
    public event Action OnWordCompleted;
    public event Action<Meteor> OnShootRequested;

    private string currentInput = "";

    [Export] public Node2D spawner;
    [Export] public Turret turret;

    private Meteor currentTarget;

    private bool hadMistake = false;

    public override void _Process(double delta)
    {
        if (
            currentTarget != null &&
            !IsInstanceValid(currentTarget)
        )
        {
            ResetInput();
        }
    }

    public override void _Input(InputEvent @event)
    {
        if (
            !(@event is InputEventKey key) ||
            !key.Pressed
        )
        {
            return;
        }

        if (key.Keycode == Key.Backspace)
        {
            if (currentInput.Length > 0)
            {
                currentInput =
                    currentInput.Substring(
                        0,
                        currentInput.Length - 1
                    );
            }

            if (currentInput.Length == 0)
            {
                currentTarget = null;

                UpdateUI();

                return;
            }

            currentTarget =
                GetClosestValidMeteor(
                    currentInput
                );

            UpdateUI();

            return;
        }

        if (
            key.Unicode <= 0 ||
            !char.IsLetterOrDigit(
                (char)key.Unicode
            )
        )
        {
            return;
        }

        char newChar = (char)key.Unicode;

        if (currentTarget == null)
        {
            currentTarget =
                GetClosestValidMeteor(
                    currentInput + newChar
                );

            if (currentTarget == null)
            {
                OnWrongChar?.Invoke();

                hadMistake = true;

                var nearest =
                    GetClosestValidMeteor("");

                nearest?.PlayErrorShake();

                ResetInput();

                return;
            }
        }

        if (
            currentTarget.Word.Length
                <= currentInput.Length ||

            currentTarget.Word[
                currentInput.Length
            ] != newChar
        )
        {
            OnWrongChar?.Invoke();

            hadMistake = true;

            currentTarget?.UpdateDisplay(
                currentInput + newChar
            );

            currentTarget?.PlayErrorShake();

            return;
        }

        OnCorrectChar?.Invoke();

        currentInput += newChar;

        UpdateUI();

        OnShootRequested?.Invoke(
            currentTarget
        );

        if (currentInput == currentTarget.Word)
        {
            currentTarget.SetHadMistake(
                hadMistake
            );

            OnWordCompleted?.Invoke();

            currentInput = "";

            hadMistake = false;

            currentTarget = null;
        }
    }

    private void UpdateUI()
    {
        currentTarget?.UpdateDisplay(
            currentInput
        );
    }

    private Meteor GetClosestValidMeteor(
        string input
    )
    {
        Meteor closest = null;

        float minDistance =
            float.MaxValue;

        foreach (Node child in spawner.GetChildren())
        {
            if (child is Meteor meteor)
            {
                if (
                    !meteor.Word.StartsWith(
                        input
                    )
                )
                {
                    continue;
                }

                float dist =
                    meteor.GlobalPosition
                    .DistanceTo(
                        turret.GlobalPosition
                    );

                if (dist < minDistance)
                {
                    minDistance = dist;

                    closest = meteor;
                }
            }
        }

        return closest;
    }

    public void ResetInput()
    {
        currentInput = "";

        currentTarget = null;

        hadMistake = false;
    }
}