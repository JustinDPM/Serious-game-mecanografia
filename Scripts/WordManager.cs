using Godot;
using System.Collections.Generic;

public partial class WordManager : Node
{
    [Export]
    public string RutaActual =
        "res://Diccionarios/nivel1.txt";

    public List<string> ListaPrimariaBaja =
        new List<string>();

    public List<string> ListaPrimariaAlta =
        new List<string>();

    public List<string> ListaSecundaria =
        new List<string>();

    public List<string> ListaPreparatoria =
        new List<string>();

    public override void _Ready()
    {
        CargarArchivoTxt(RutaActual);
    }

    public void CargarArchivoTxt(string ruta)
    {
        RutaActual = ruta;

        ListaPrimariaBaja.Clear();
        ListaPrimariaAlta.Clear();
        ListaSecundaria.Clear();
        ListaPreparatoria.Clear();

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

            int dificultad =
                EvaluarDificultadLinea(linea);

            if (dificultad == 1)
                ListaPrimariaBaja.Add(linea);
            else if (dificultad == 2)
                ListaPrimariaAlta.Add(linea);
            else if (dificultad == 3)
                ListaSecundaria.Add(linea);
            else if (dificultad == 4)
                ListaPreparatoria.Add(linea);
        }

        GD.Print(
            "WordManager: archivo cargado: " + ruta
        );
    }

    public List<string> ObtenerPalabrasParaJuego(
        int nivelSeleccionado
    )
    {
        List<string> palabrasParaJugar =
            new List<string>();

        if (nivelSeleccionado >= 1)
            palabrasParaJugar.AddRange(ListaPrimariaBaja);

        if (nivelSeleccionado >= 2)
            palabrasParaJugar.AddRange(ListaPrimariaAlta);

        if (nivelSeleccionado >= 3)
            palabrasParaJugar.AddRange(ListaSecundaria);

        if (nivelSeleccionado >= 4)
            palabrasParaJugar.AddRange(ListaPreparatoria);

        return palabrasParaJugar;
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

    private int EvaluarDificultadLinea(string linea)
    {
        string[] partes =
            linea.Split(
                ' ',
                System.StringSplitOptions.RemoveEmptyEntries
            );

        int cantidadPalabras = partes.Length;

        if (cantidadPalabras == 1)
        {
            int letras = linea.Length;

            if (letras < 5) return 1;
            if (letras < 7) return 2;
            if (letras < 10) return 3;

            return 4;
        }
        else
        {
            if (cantidadPalabras <= 3) return 1;
            if (cantidadPalabras <= 5) return 2;
            if (cantidadPalabras <= 7) return 3;

            return 4;
        }
    }
}