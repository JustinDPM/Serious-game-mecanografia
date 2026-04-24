using Godot;
using System;
using System.Collections.Generic;

public partial class MeteorSpawner : Node2D
{
	[Export] public PackedScene MeteorScene;
	[Export] public float SpawnInterval = 2f;

	private List<string> words = new List<string>
	{
		"sol", "luna", "astro", "cometa", "galaxia"
	};

	public override void _Ready()
	{
		SpawnLoop();
	}

	private async void SpawnLoop()
	{
		while (true)
		{
			await ToSignal(GetTree().CreateTimer(SpawnInterval), "timeout");
			SpawnMeteor();
		}
	}

	private void SpawnMeteor()
	{
		var meteor = (Meteor)MeteorScene.Instantiate();

		meteor.Word = words[GD.RandRange(0, words.Count - 1)];
		meteor.Position = new Vector2(GD.RandRange(50, 700), 0);

		AddChild(meteor);
	}
}
