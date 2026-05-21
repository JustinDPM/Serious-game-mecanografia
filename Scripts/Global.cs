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

    public string UsuarioActivo  = "Jugador";
	public string NombreCompleto = "Jugador Local";
	public string Rol            = "Alumno";
	public int    IdUsuario      = 0;   
	public int    IdGrado        = 3;   // Grado por defecto: Secundaria
	public string RutaFotoPerfil = "res://assets/Perfiles/default.jpg";


	public int   LastScore    = 0;
	public float LastAccuracy = 0f;
	public float LastWPM      = 0f;


    public static readonly string[] Diccionario = new string[]
	{
		"gato", "perro", "casa", "árbol", "libro",
		"cielo", "luna", "sol", "mar", "río",
		"fuego", "agua", "viento", "tierra", "nube",
		"ciudad", "campo", "montaña", "valle", "bosque",
		"amor", "miedo", "sueño", "tiempo", "vida",
		"escuela", "mesa", "silla", "puerta", "ventana",
		"avión", "cohete", "barco", "tren", "coche",
		"música", "pintura", "teatro", "danza", "poema",
		"estrella", "planeta", "galaxia", "universo", "cosmos",
		"dragón", "espada", "escudo", "héroe", "magia",
		"computadora", "teclado", "pantalla", "ratón", "código",
		"tiempo", "espacio", "energía", "materia", "átomo",
		"naranja", "manzana", "uva", "sandía", "limón",
		"México", "Guerrero", "Acapulco", "Chilpancingo", "Taxco",
		"rápido", "lento", "grande", "pequeño", "fuerte",
		"victoria", "derrota", "batalla", "guerra", "paz"
	};

    public static readonly string[] Parrafos = new string[]
	{
		"El gato corre bajo la luna",
		"La lectura mejora la ortografía y la memoria",
		"Escribir rápido también requiere precisión",
		"Los meteoritos grandes son más peligrosos"
	};

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
