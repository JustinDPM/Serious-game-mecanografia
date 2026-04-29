using Godot;
using System;
using System.Collections.Generic;

public partial class MeteorSpawner : Node2D
{
	[Export] public PackedScene MeteorScene;
	[Export] public float SpawnInterval = 2f;

	[Export] public Node2D Turret;

	private List<string> words = new List<string>
	{
		"sol", "luna", "astro", "cometa", "galaxia", "dia"
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

		// palabra aleatoria
		meteor.Word = words[GD.RandRange(0, words.Count - 1)];

		// posición random arriba
		float screenWidth = GetViewportRect().Size.X;
		float randomX = (float)GD.RandRange(50, screenWidth - 50);

		meteor.Position = new Vector2(randomX, 0);

		// 💥 SOLO SET TARGET (vida ya se calcula sola)
		meteor.SetTarget(Turret);

		AddChild(meteor);
	}
}
