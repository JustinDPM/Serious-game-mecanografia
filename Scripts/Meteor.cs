using Godot;
using System;

public partial class Meteor : CharacterBody2D
{
[Export] public float Speed = 120f;

	public string Word = "";
	private RichTextLabel label;
	private Node2D target;
	private bool firstAttempt = true;

	public override void _Ready()
	{
		label = GetNode<RichTextLabel>("RichTextLabel");
		label.Text = Word;
	}

	public void SetTarget(Node2D player)
	{
		target = player;
	}

	public override void _PhysicsProcess(double delta)
	{
		if (target != null)
		{
			Vector2 direction = (target.GlobalPosition - GlobalPosition).Normalized();
			Velocity = direction * Speed;
		}

		MoveAndSlide();

		if (Position.Y > 800)
			QueueFree();
	}

	public bool CheckWord(string input)
	{
		if (input == Word)
		{
			QueueFree();
			return true;
		}
		firstAttempt = false;
		return false;
	}
	
	public void UpdateDisplay(string input)
	{
		string result = "";

		for (int i = 0; i < Word.Length; i++)
		{
			if (i < input.Length && input[i] == Word[i])
			{
				result += "[color=green]" + Word[i] + "[/color]";
			}
			else if (input.Length == 0 && !firstAttempt)
			{
				result = "[color=red]" + Word + "[/color]";
			}
			else
			{
				result += Word[i];
			}
		}

		label.Text = result;
	}
	
	

}
