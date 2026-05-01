using Godot;
using System;

public partial class InputManager : Node
{
    private string currentInput = "";
    private Node2D spawner;
    private Turret turret;

    private Meteor currentTarget;

    private int pendingShots = 0;

    private float shootCooldown = 0f;
    private const float cooldownTime = 0.05f;

    public override void _Ready()
    {
        spawner = GetNode<Node2D>("/root/Level1/MeteorSpawner");
        turret = GetNode<Turret>("../Turret");
    }

    public override void _Process(double delta)
    {
        if (shootCooldown > 0)
            shootCooldown -= (float)delta;

        // disparo desde cola
        if (pendingShots > 0 && shootCooldown <= 0f && currentTarget != null)
        {
            turret.Shoot(currentTarget);
            pendingShots--;
            shootCooldown = cooldownTime;
        }

        // limpiar si muere
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
                return;
            }

            currentTarget = GetClosestValidMeteor(currentInput);
            UpdateUI();
            return;
        }

        if (key.Unicode > 0 && char.IsLetterOrDigit((char)key.Unicode))
        {
            char newChar = (char)key.Unicode;

            if (currentTarget == null)
            {
                currentInput += newChar;
                currentTarget = GetClosestValidMeteor(currentInput);

                if (currentTarget == null)
                {
                    currentInput = "";
                    return;
                }
            }
            else
            {
                if (currentTarget.Word.Length <= currentInput.Length ||
                    currentTarget.Word[currentInput.Length] != newChar)
                {
                    return;
                }

                currentInput += newChar;
            }

            UpdateUI();

            // 🔥 SIEMPRE se guarda disparo
            pendingShots++;

            // palabra completa → NO score aquí
            if (currentTarget.Word == currentInput)
            {
                currentInput = "";
                // NO borrar target aquí
            }
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
        pendingShots = 0;
    }
}