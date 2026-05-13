using Godot;
using System;

public partial class Ship : Node2D
{
    [Export] public float FloatAmplitude = 5f;
    [Export] public float FloatSpeed = 2f;

    private Vector2 startPosition;
    private float time;

    public override void _Ready()
    {
        startPosition = Position;
    }

    public override void _Process(double delta)
    {
        time += (float)delta;

        float offsetY =
            Mathf.Sin(time * FloatSpeed) * FloatAmplitude;

        Position = new Vector2(
            startPosition.X,
            startPosition.Y + offsetY
        );
    }
}