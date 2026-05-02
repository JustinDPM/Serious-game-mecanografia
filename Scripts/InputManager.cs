using Godot;
using System;

public partial class InputManager : Node
{
    public event Action OnCorrectChar;
    public event Action OnWrongChar;
    public event Action OnWordCompleted;
    public event Action<Meteor> OnShootRequested;

    private string currentInput = "";
    private Node2D spawner;
    private Turret turret;

    private Meteor currentTarget;

    public override void _Ready()
    {
        spawner = GetNode<Node2D>("/root/Level1/MeteorSpawner");
        turret = GetNode<Turret>("../Turret");
    }

    public override void _Process(double delta)
    {
        if (currentTarget != null && !IsInstanceValid(currentTarget))
            ResetInput();
    }

    public override void _Input(InputEvent @event)
    {
        if (!(@event is InputEventKey key) || !key.Pressed)
            return;

        if (key.Keycode == Key.Backspace)
        {
            if (currentInput.Length > 0)
                currentInput = currentInput.Substring(0, currentInput.Length - 1);

            if (currentInput.Length == 0)
            {
                currentTarget = null;
                UpdateUI();
                return;
            }

            currentTarget = GetClosestValidMeteor(currentInput);
            UpdateUI();
            return;
        }

        if (key.Unicode <= 0 || !char.IsLetterOrDigit((char)key.Unicode))
            return;

        char newChar = (char)key.Unicode;

        if (currentTarget == null)
        {
            currentTarget = GetClosestValidMeteor(currentInput + newChar);

            if (currentTarget == null)
            {
                ResetInput();
                OnWrongChar?.Invoke();
                return;
            }
        }

        if (currentTarget.Word.Length <= currentInput.Length ||
            currentTarget.Word[currentInput.Length] != newChar)
        {
            OnWrongChar?.Invoke();
            return;
        }

        OnCorrectChar?.Invoke();

        currentInput += newChar;
        UpdateUI();

        OnShootRequested?.Invoke(currentTarget);

        if (currentInput == currentTarget.Word)
        {
            OnWordCompleted?.Invoke();

            currentInput = "";
            currentTarget = null; 
        }
    }

    private void UpdateUI()
    {
        if (currentTarget != null)
            currentTarget.UpdateDisplay(currentInput);
    }

    private Meteor GetClosestValidMeteor(string input)
    {
        Meteor closest = null;
        float minDistance = float.MaxValue;

        foreach (Node child in spawner.GetChildren())
        {
            if (child is Meteor meteor)
            {
                if (!meteor.Word.StartsWith(input))
                    continue;

                float dist = meteor.GlobalPosition.DistanceTo(turret.GlobalPosition);

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
    }
}