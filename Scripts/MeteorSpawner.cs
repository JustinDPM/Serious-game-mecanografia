using Godot;
using System.Collections.Generic;

public partial class MeteorSpawner : Node2D
{
    [Export] public PackedScene MeteorScene;
    [Export] public PackedScene MeteorLongScene;

    [Export] public float SpawnInterval = 2f;
    [Export] public Turret Turret;

    [Export] public int LongMeteorEvery = 8;

    [Export] public int NivelSeleccionado = 1;

    [Export] public float NormalMeteorSpeed = 250f;
    [Export] public float LongMeteorSpeed = 140f;

    private List<string> words = new List<string>();
    private List<string> paragraphs = new List<string>();

    private MeteorLong activeLongMeteor = null;
    private int spawnedNormalMeteors = 0;

    private Global global;
    private WordManager wordManager;

    public override void _Ready()
    {
        global =
            GetNode<Global>("/root/Global");

        wordManager =
            GetNode<WordManager>("/root/WordManager");

        wordManager.CargarArchivoTxt(
            global.RutaTxtCustom
        );

        words.AddRange(
            wordManager.ObtenerPalabrasParaJuego()
        );

        paragraphs.AddRange(
            wordManager.ObtenerFrasesParaJuego()
        );

        if (words.Count == 0)
        {
            GD.PrintErr("MeteorSpawner: diccionario vacío, usando respaldo.");
            words = new List<string> { "error", "base", "datos", "vacia", "ayuda" };
        }

        if (paragraphs.Count == 0)
        {
            GD.PrintErr("MeteorSpawner: frases vacío, usando respaldo.");
            paragraphs = new List<string>
            {
                "El gato corre bajo la luna.",
                "La lectura mejora la memoria."
            };
        }

        GD.Print($"MeteorSpawner: {words.Count} palabras cargadas.");
        GD.Print($"MeteorSpawner: {paragraphs.Count} frases cargadas.");

        SpawnLoop();
    }

    private async void SpawnLoop()
    {
        while (true)
        {
            await ToSignal(
                GetTree().CreateTimer(SpawnInterval),
                "timeout"
            );

            if (GetTree().Paused)
                continue;

            if (
                activeLongMeteor != null &&
                IsInstanceValid(activeLongMeteor)
            )
                continue;

            SpawnMeteor();
        }
    }

    private void SpawnMeteor()
    {
        bool shouldSpawnLong =
            MeteorLongScene != null &&
            LongMeteorEvery > 0 &&
            spawnedNormalMeteors >= LongMeteorEvery;

        if (shouldSpawnLong)
        {
            SpawnLongMeteor();
            spawnedNormalMeteors = 0;
            return;
        }

        SpawnNormalMeteor();
        spawnedNormalMeteors++;
    }

    private void SpawnNormalMeteor()
    {
        var meteor =
            MeteorScene.Instantiate<Meteor>();

        meteor.Word =
            words[
                GD.RandRange(
                    0,
                    words.Count - 1
                )
            ];

        meteor.Speed =
            NormalMeteorSpeed;

        SetupMeteor(meteor, -100);
    }

    private void SpawnLongMeteor()
    {
        var meteor =
            MeteorLongScene.Instantiate<MeteorLong>();

        meteor.Word =
            paragraphs[
                GD.RandRange(
                    0,
                    paragraphs.Count - 1
                )
            ];

        meteor.Speed =
            LongMeteorSpeed;

        activeLongMeteor = meteor;

        SetupMeteor(meteor, -160);
    }

    private void SetupMeteor(
        Meteor meteor,
        float yPosition
    )
    {
        float screenWidth =
            GetViewportRect().Size.X;

        float randomX =
            (float)GD.RandRange(
                150,
                screenWidth - 150
            );

        meteor.Position =
            new Vector2(randomX, yPosition);

        meteor.SetTarget(Turret);
        meteor.SetTurret(Turret);

        var stats =
            GetNodeOrNull<StatsManager>(
                "../StatsManager"
            );

        if (stats != null)
            meteor.OnMeteorDestroyed +=
                stats.OnMeteorDestroyed;

        meteor.OnMeteorDestroyed +=
            OnMeteorDestroyed;

        AddChild(meteor);
    }

    private void OnMeteorDestroyed(
        Meteor meteor
    )
    {
        if (meteor == activeLongMeteor)
            activeLongMeteor = null;
    }
}