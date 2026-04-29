using Godot;
using System;

public partial class InputManager : Node
{
	private string currentInput = "";
	private Node2D spawner;
	private Turret turret;

	private Meteor currentTarget;

	public override void _Ready()
	{
		spawner = GetNode<Node2D>("/root/Level1/MeteorSpawner");
		turret = GetNode<Turret>("../Turret");
	}

	public override void _Input(InputEvent @event)
	{
		if (!(@event is InputEventKey key) || !key.Pressed)
			return;

		// BACKSPACE
		if (key.Keycode == Key.Backspace)
		{
			if (currentInput.Length > 0)
				currentInput = currentInput.Substring(0, currentInput.Length - 1);

			currentTarget = GetClosestValidMeteor(currentInput);
			UpdateUI();
			return;
		}

		// INPUT NORMAL
		if (key.Unicode > 0 && char.IsLetterOrDigit((char)key.Unicode))
		{
			currentInput += (char)key.Unicode;

			currentTarget = GetClosestValidMeteor(currentInput);

			if (currentTarget == null)
			{
				currentInput = currentInput.Substring(0, currentInput.Length - 1);
				return;
			}

			UpdateUI();

			// 💥 PALABRA COMPLETA → DISPARO
			if (currentTarget.Word == currentInput)
			{
				int shots = currentTarget.Word.Length;

				turret.ShootBurst(currentTarget, shots);

				currentInput = "";
				currentTarget = null;
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
}
