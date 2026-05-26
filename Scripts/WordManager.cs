using Godot;
using System.Collections.Generic;

public partial class WordManager : Node
{
    [Export]
    public string RutaActual =
        "res://Diccionarios/nivel1.txt";

    public List<string> ListaPalabras =
        new List<string>();

    public List<string> ListaFrases =
        new List<string>();

    public override void _Ready()
    {
        CargarArchivoTxt(RutaActual);
    }

    public void CargarArchivoTxt(string ruta)
    {
        RutaActual = ruta;

        ListaPalabras.Clear();
        ListaFrases.Clear();

        if (!FileAccess.FileExists(ruta))
        {
            GD.PrintErr(
                "WordManager: No se encontró el archivo: " + ruta
            );

            return;
        }

        using var file =
            FileAccess.Open(ruta, FileAccess.ModeFlags.Read);

        while (!file.EofReached())
        {
            string linea = file.GetLine().Trim();

            if (string.IsNullOrEmpty(linea))
                continue;

            if (EsFrase(linea))
                ListaFrases.Add(linea);
            else
                ListaPalabras.Add(linea);
        }

        GD.Print(
            "WordManager: archivo cargado: " + ruta
        );
    }

    public List<string> ObtenerPalabrasParaJuego()
    {
        return new List<string>(ListaPalabras);
    }

    public List<string> ObtenerFrasesParaJuego()
    {
        return new List<string>(ListaFrases);
    }

    public bool AgregarPalabra(string palabra)
    {
        palabra = palabra.Trim();

        if (string.IsNullOrEmpty(palabra))
            return false;

        using var file =
            FileAccess.Open(
                RutaActual,
                FileAccess.ModeFlags.ReadWrite
            );

        if (file == null)
        {
            GD.PrintErr(
                "WordManager: No se pudo abrir el archivo para escribir: " +
                RutaActual
            );

            return false;
        }

        file.SeekEnd();
        file.StoreLine(palabra);

        CargarArchivoTxt(RutaActual);

        return true;
    }

    private bool EsFrase(string linea)
    {
        string[] partes =
            linea.Split(
                ' ',
                System.StringSplitOptions.RemoveEmptyEntries
            );

        return partes.Length > 1;
    }
}