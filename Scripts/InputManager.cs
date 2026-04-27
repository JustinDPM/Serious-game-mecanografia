using Godot;
using System;

public partial class InputManager : Node
{
	private string currentInput = "";
	private Node2D spawner;
	private Player player;

	public override void _Ready()
	{
		spawner = GetNode<Node2D>("/root/Game/MeteorSpawner");
		player = GetNode<Player>("../Player");
	}

	public override void _Input(InputEvent @event)
	{
		if (@event is InputEventKey key && key.Pressed)
		{

			if (key.Keycode == Key.Backspace)
			{
				if (currentInput.Length > 0)
					currentInput = currentInput.Substring(0, currentInput.Length - 1);

				GD.Print(currentInput);
				CheckMeteors();
				return;
			}

			if (key.Unicode > 0)
			{
				currentInput += (char)key.Unicode;

				Meteor target = GetClosestMeteor();

				if (target == null || !target.Word.StartsWith(currentInput))
				{
					CheckMeteors();
					currentInput = currentInput.Substring(0, currentInput.Length -1);
					return;
				}
				
				player.Shoot(target);
				GD.Print(currentInput);
				CheckMeteors();
			}
		}
	}

	private void CheckMeteors()
	{
		Meteor target = GetClosestMeteor();

		foreach (Node child in spawner.GetChildren())
		{
			if (child is Meteor meteor)
			{
				meteor.UpdateDisplay("");
			}
		}

		if (target != null)
		{
			target.UpdateDisplay(currentInput);

			if (target.CheckWord(currentInput))
			{
				currentInput = "";
			}
		}
	}

	private Meteor GetClosestMeteor()
	{
		Meteor closest = null;
		float minDistance = float.MaxValue;

		foreach (Node child in spawner.GetChildren())
		{
			if (child is Meteor meteor)
			{
				float dist = meteor.GlobalPosition.DistanceTo(player.GlobalPosition);

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
