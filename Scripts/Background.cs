using Godot;

public partial class Background : Node2D
{
    [Export] public float Speed = 100f;

    private Sprite2D bg1;
    private Sprite2D bg2;
    private Sprite2D bg3;

    private GpuParticles2D stars1;
    private GpuParticles2D stars2;

    private float height;
    private Turret turret;

    public override void _Ready()
    {
        bg1 = GetNode<Sprite2D>("BG1");
        bg2 = GetNode<Sprite2D>("BG2");
        bg3 = GetNode<Sprite2D>("BG3");

        stars1 = GetNode<GpuParticles2D>("Stars");
        stars2 = GetNode<GpuParticles2D>("BlueStars");

        turret = GetTree().Root.GetNodeOrNull<Turret>("Level1/Turret");

        height = bg1.Texture.GetHeight();

        bg1.Position = new Vector2(0, 0);
        bg2.Position = new Vector2(0, -height);
        bg3.Position = new Vector2(0, -height * 2);
    }

    public override void _Process(double delta)
    {
        int streak = turret != null ? turret.GetStreak() : 0;

        float dynamicSpeed = Mathf.Min(Speed + (streak * 12f), 350f);

        float move = dynamicSpeed * (float)delta;

        bg1.Position += new Vector2(0, move);
        bg2.Position += new Vector2(0, move);
        bg3.Position += new Vector2(0, move);

        Loop(bg1);
        Loop(bg2);
        Loop(bg3);

        float factor = 1f + (streak * 0.08f);

        if (stars1 != null)
            stars1.SpeedScale = Mathf.Min(1.2f * factor, 4f);

        if (stars2 != null)
            stars2.SpeedScale = Mathf.Min(0.7f * factor, 3f);
    }

    private void Loop(Sprite2D bg)
    {
        if (bg.Position.Y >= height)
        {
            float highest = bg1.Position.Y;
            highest = Mathf.Min(highest, bg2.Position.Y);
            highest = Mathf.Min(highest, bg3.Position.Y);

            bg.Position = new Vector2(0, highest - height);
        }
    }
}