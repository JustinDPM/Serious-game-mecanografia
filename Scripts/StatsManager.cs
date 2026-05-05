using Godot;
using System;
using Npgsql; // Obligatorio para conectar con Postgres

public partial class StatsManager : Node
{
	private float timeAlive = 0f;

	private int correct = 0;
	private int wrong = 0;
	private int words = 0;
	private int totalInputs = 0;

	[Export] public InputManager input;
	[Export] public Turret turret;
	[Export] public Global global;

	// Cadena de conexión a tu BD
	private string connectionString = "Host=localhost;Username=postgres;Password=040306;Database=astrotype_db";

	public override void _Ready()
	{
		if (global == null)
			global = GetNode<Global>("/root/Global");

		if (input == null)
		{
			GD.PrintErr("StatsManager: InputManager no asignado");
			return;
		}

		input.OnCorrectChar += OnCorrectChar;
		input.OnWrongChar += OnWrongChar;
		input.OnWordCompleted += OnWordCompleted;

		if (turret != null)
			turret.OnGameOver += SaveResults;
	}

	public override void _Process(double delta)
	{
		timeAlive += (float)delta;
	}

	public void OnCorrectChar()
	{
		correct++;
		totalInputs++;
	}

	public void OnWrongChar()
	{
		wrong++;
		totalInputs++;
	}

	public void OnWordCompleted()
	{
		words++;
	}

	public float GetWPM()
	{
		float minutes = timeAlive / 60f;
		if (minutes <= 0) return 0;

		return words / minutes;
	}

	public float GetAccuracy()
	{
		if (totalInputs == 0)
			return 100f;

		return (float)correct / totalInputs * 100f;
	}

	public string GetTime()
	{
		int m = (int)(timeAlive / 60);
		int s = (int)(timeAlive % 60);
		return $"{m:00}:{s:00}";
	}

	private void SaveResults()
	{
		if (global == null || turret == null) return;

		// Guardamos en memoria local (para la pantalla de Game Over)
		global.LastScore = turret.GetScore();
		global.LastAccuracy = GetAccuracy();
		global.LastWPM = GetWPM();

		GD.Print("=== GUARDANDO RESULTADOS ===");
		GD.Print("Score: " + turret.GetScore());
		GD.Print("Accuracy: " + GetAccuracy());
		GD.Print("WPM: " + GetWPM());

		// --- GUARDAR EN LA BASE DE DATOS ---
		// Verificamos que sea un usuario logueado (que no sea un test suelto)
		if (global.IdUsuario > 0)
		{
			try
			{
				using (var conn = new NpgsqlConnection(connectionString))
				{
					conn.Open();
					
					// Ajustamos los nombres exactos a tu tabla PARTIDA y agregamos fecha_fin
					string query = @"INSERT INTO PARTIDA (id_alumno, ppm_promedio, precision_porcentaje, puntaje_final, fecha_fin) 
									 VALUES (@id, @wpm, @acc, @score, CURRENT_TIMESTAMP)";
					
					using (var cmd = new NpgsqlCommand(query, conn))
					{
						cmd.Parameters.AddWithValue("id", global.IdUsuario);
						cmd.Parameters.AddWithValue("wpm", GetWPM()); // Guardamos con decimales
						cmd.Parameters.AddWithValue("acc", GetAccuracy()); 
						cmd.Parameters.AddWithValue("score", turret.GetScore());

						cmd.ExecuteNonQuery();
					}
					GD.Print("¡Éxito! Estadísticas guardadas en PostgreSQL con fecha y decimales.");
				}
			}
			catch (Exception ex)
			{
				GD.PrintErr("Error al guardar la partida en BD: " + ex.Message);
			}
		}
	}

	public void OnMeteorDestroyed(Meteor meteor)
	{
		if (turret == null) return;

		turret.AddScore(1);
		turret.addStreak();
	}
}
