using Godot;
using System;

public partial class Meteor : CharacterBody2D
{
[Export] public float Speed = 120f;

	public string Word = "";
	private RichTextLabel label;
	private Node2D target;
	private bool firstAttempt = true;
	private int Health;

	public override void _Ready()
	{
		label = GetNode<RichTextLabel>("RichTextLabel");
		label.Text = Word;
	}

	public void SetTarget(Node2D player)
	{
		target = player;
	}
	
	public void SetHealth(int Health){
		this.Health = Health;
	}
	
	public int GetHealth(){
		return Health;
	}
	public override void _PhysicsProcess(double delta)
	{
		if (target != null)
		{
			Vector2 direction = (target.GlobalPosition - GlobalPosition).Normalized();
			Velocity = direction * Speed;
		}

		MoveAndSlide();

        for (int i = 0; i < GetSlideCollisionCount(); i++)
        {
            var collision = GetSlideCollision(i);
            var collider = collision.GetCollider();

            if (collider is Turret turret)
            {
                turret.TakeDamage(1);
                QueueFree();
                break; // 🔥 ESTO ARREGLA TODO
            }
        }
    }

	public bool CheckWord(string input)
	{
		if (Word == input)
		{
			return true;
		}
		firstAttempt = false;
		return false;
	}
	
	public void Die(){
		QueueFree();
	}
	
	public void TakeDamage(){
		Health -= 1;
		if(Health == 0){
			Die();
		}
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
			else if (i < input.Length && input[i] != Word[i])
			{
				result += "[color=red]" + Word[i] + "[/color]";
			}
			else
			{
				result += Word[i];
			}
		}

		label.Text = result;
	}
	
	

}
