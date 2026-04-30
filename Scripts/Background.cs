using Godot;

public partial class Background : Node2D
{
    [Export] public float Speed = 100f;

    private Sprite2D bg1;
    private Sprite2D bg2;
    private Sprite2D bg3;

    private float height;

    public override void _Ready()
    {
        bg1 = GetNode<Sprite2D>("BG1");
        bg2 = GetNode<Sprite2D>("BG2");
        bg2 = GetNode<Sprite2D>("BG2");
        bg3 = GetNode<Sprite2D>("BG3");

        // altura del fondo (ej: 1080)
        height = bg1.Texture.GetHeight();

        // posicionarlos en torre
        bg1.Position = new Vector2(0, 0);
        bg2.Position = new Vector2(0, -height);
        bg3.Position = new Vector2(0, -height * 2);
    }

    public override void _Process(double delta)
    {
        float move = Speed * (float)delta;

        bg1.Position += new Vector2(0, move);
        bg2.Position += new Vector2(0, move);
        bg3.Position += new Vector2(0, move);

        Loop(bg1);
        Loop(bg2);
        Loop(bg3);
    }

    private void Loop(Sprite2D bg)
    {
        if (bg.Position.Y >= height)
        {
            // obtener el más alto (más negativo)
            float highest = bg1.Position.Y;
            highest = Mathf.Min(highest, bg2.Position.Y);
            highest = Mathf.Min(highest, bg3.Position.Y);

            // reposicionar arriba
            bg.Position = new Vector2(0, highest - height);
        }
    }
}