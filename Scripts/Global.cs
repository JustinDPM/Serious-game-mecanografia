using Godot;
using System.Collections.Generic;

public class MatchResult
{
	public int Score;
	public float Accuracy;
	public float WPM;
	public string Duration;
	public string LevelName;
}

public partial class Global : Node
{
	public List<MatchResult> MatchHistory = new List<MatchResult>();

	public string EscenaDestino = "";
	public string RutaNivelCustom = "";
	public string RutaTxtCustom = "res://Diccionarios/nivel1.txt";
	
	public string NivelActual = ""; 
	
	public string UsuarioActivo  = "Jugador";
	public string NombreCompleto = "Jugador Local";
	public string Rol            = "Alumno";
	public int    IdUsuario      = 0;   
	public int    IdGrado        = 3;   
	public string RutaFotoPerfil = "res://assets/Perfiles/default.jpg";

	public int   LastScore    = 0;
	public float LastAccuracy = 0f;
	public float LastWPM      = 0f;

	public void CambiarEscena(string rutaEscena)
	{
		Error error = GetTree().ChangeSceneToFile(rutaEscena);
		if (error != Error.Ok)
			GD.PrintErr($"No se pudo cargar la escena: {rutaEscena}");
	}

	public void LimpiarSesion()
	{
		UsuarioActivo = "Jugador";
		GD.Print("Sesión reiniciada.");
	}
}
