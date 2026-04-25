using Godot;
using System;

public partial class Meteor : CharacterBody2D
{
[Export] public float Speed = 120f;

	public string Word = "";

	private Label label;
	private Node2D target;

	public override void _Ready()
	{
		label = GetNode<Label>("Label");
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
		return false;
	}
}
