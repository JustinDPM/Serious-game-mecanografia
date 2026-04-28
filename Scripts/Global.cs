using Godot;
using System;

public partial class Global : Node
{
	public string UsuarioActivo { get; set; } = "";

	public void CambiarEscena(string rutaEscena)
	{
		Error error = GetTree().ChangeSceneToFile(rutaEscena);
		if (error != Error.Ok)
		{
			GD.PrintErr($"No se pudo cargar la escena: {rutaEscena}");
		}
	}

	public void LimpiarSesion()
	{
		UsuarioActivo = "";
		GD.Print("Sesión cerrada.");
	}
}
