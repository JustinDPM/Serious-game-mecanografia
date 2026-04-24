using Godot;
using System;

public partial class InputManager : Node
{
	private string currentInput = "";
	private Node2D spawner;

	public override void _Ready()
	{
		spawner = GetNode<Node2D>("../MeteorSpawner");
	}

	public override void _Input(InputEvent @event)
	{
		if (@event is InputEventKey key && key.Pressed)
		{
			// 🔴 BACKSPACE
			if (key.Keycode == Key.Backspace)
			{
				if (currentInput.Length > 0)
					currentInput = currentInput.Substring(0, currentInput.Length - 1);

				GD.Print(currentInput);
				return;
			}

			// 🟢 LETRAS
			if (key.Unicode > 0)
			{
				currentInput += (char)key.Unicode;

				// 🔥 VALIDACIÓN INTELIGENTE
				if (!MatchesAnyMeteor(currentInput))
				{
					currentInput = "";
				}

				GD.Print(currentInput);
				CheckMeteors();
			}
		}
	}

	private void CheckMeteors()
	{
		foreach (Node child in spawner.GetChildren())
		{
			if (child is Meteor meteor)
			{
				if (meteor.CheckWord(currentInput))
				{
					currentInput = "";
					break;
				}
			}
		}
	}

	private bool MatchesAnyMeteor(string input)
	{
		foreach (Node child in spawner.GetChildren())
		{
			if (child is Meteor meteor)
			{
				if (meteor.Word.StartsWith(input))
					return true;
			}
		}
		return false;
	}
}
