using Godot;
using System;

public partial class Tutorial : Control
{
	private PanelContainer panel;
	private StyleBoxTexture styleBox;
	private AnimatedTexture animatedTexture;

	public override void _Ready()
	{
		panel = GetNode<PanelContainer>("PanelContainer");

		styleBox = panel.GetThemeStylebox("panel") as StyleBoxTexture;

		animatedTexture = styleBox.Texture as AnimatedTexture;
	}

	private void _on_next_pressed()
	{
		animatedTexture.CurrentFrame++;

		if (animatedTexture.CurrentFrame >= animatedTexture.Frames)
			animatedTexture.CurrentFrame = 0;
			
	}

	private void _on_prev_pressed()
	{
		animatedTexture.CurrentFrame--;

		if (animatedTexture.CurrentFrame < 0)
			animatedTexture.CurrentFrame = animatedTexture.Frames - 1;
	}

	private void _on_salir_pressed()
	{
		GetTree().ChangeSceneToFile("res://Escenas/main_menu.tscn");
	}
}
