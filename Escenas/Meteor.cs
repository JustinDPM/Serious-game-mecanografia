using Godot;
using System;

public partial class Meteor : CharacterBody2D
{
	[Export] public float Speed = 120f;

	public string Word = "";

	private Label label;

	public override void _Ready()
	{
		label = GetNode<Label>("Label");
		label.Text = Word;
	}

	public override void _PhysicsProcess(double delta)
	{
		Velocity = new Vector2(0, Speed);
		MoveAndSlide();

		if (Position.Y > 700)
			QueueFree();
	}

	public bool CheckWord(string input)
	{
		if (input == Word)
		{
			QueueFree();
			return true;
		}
		return false;
	}
}
