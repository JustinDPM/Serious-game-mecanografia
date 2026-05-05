using Godot;
using System;
using System.Collections.Generic;
using Npgsql; // Obligatorio para conectar con Postgres

public partial class MeteorSpawner : Node2D
{
	[Export] public PackedScene MeteorScene;
	[Export] public float SpawnInterval = 2f;

	[Export] public Turret Turret;

	// La lista empieza vacía, la llenaremos con la BD
	private List<string> words = new List<string>(); 
	
	private Global _global;
	private string connectionString = "Host=localhost;Username=postgres;Password=040306;Database=astrotype_db";

	public override void _Ready()
	{
		_global = GetNode<Global>("/root/Global");
		
		// 1. Descargamos las palabras antes de empezar a soltar meteoritos
		CargarPalabrasDesdeBD();

		// 2. Salvavidas: Si la BD está vacía o falló la conexión, ponemos palabras de emergencia
		if (words.Count == 0)
		{
			GD.PrintErr("No hay palabras en la BD. Usando diccionario de emergencia.");
			words = new List<string> { "error", "base", "datos", "vacia", "ayuda" };
		}

		SpawnLoop();
	}

	private void CargarPalabrasDesdeBD()
	{
		try
		{
			using (var conn = new NpgsqlConnection(connectionString))
			{
				conn.Open();
				
				// La consulta: Si es alumno, le mandamos palabras de su grado (y los anteriores).
				// Si es profe/admin, le mandamos TODO el diccionario.
				string query = "SELECT texto FROM PALABRA";
				
				if (_global.Rol == "Alumno")
				{
					query += " WHERE id_grado <= @grado";
				}

				using (var cmd = new NpgsqlCommand(query, conn))
				{
					if (_global.Rol == "Alumno")
					{
						cmd.Parameters.AddWithValue("grado", _global.IdGrado);
					}

					using (var reader = cmd.ExecuteReader())
					{
						while (reader.Read())
						{
							// Agregamos la palabra leída a nuestra lista
							words.Add(reader.GetString(0));
						}
					}
				}
				GD.Print($"Éxito: Se cargaron {words.Count} palabras en memoria.");
			}
		}
		catch (Exception ex)
		{
			GD.PrintErr("Error fatal al cargar el diccionario: " + ex.Message);
		}
	}

	private async void SpawnLoop()
	{
		while (true)
		{
			await ToSignal(GetTree().CreateTimer(SpawnInterval), "timeout");

			if (GetTree().Paused)
				continue;

			SpawnMeteor();
		}
	}

	private void SpawnMeteor()
	{
		var meteor = (Meteor)MeteorScene.Instantiate();

		// Toma una palabra al azar de nuestra lista recién cargada
		meteor.Word = words[GD.RandRange(0, words.Count - 1)];

		float screenWidth = GetViewportRect().Size.X;
		float randomX = (float)GD.RandRange(50, screenWidth - 50);

		meteor.Position = new Vector2(randomX, -100);

		meteor.SetTarget(Turret);
		meteor.SetTurret(Turret);

		var stats = GetNode<StatsManager>("../StatsManager");
		meteor.OnMeteorDestroyed += stats.OnMeteorDestroyed;

		AddChild(meteor);
	}
}
