using Godot;
using System;

public partial class BackgroundMenu : Node2D
{
	[Export] public float BaseSpeed = 150f; 
	// Ya no es necesario que tú escales en el Inspector, el código lo hará solo.
	[Export] public Vector2 BackgroundScale = new Vector2(1f, 1f); 
	[Export] public PackedScene EscenaNave; 

	private Sprite2D bg1;
	private Sprite2D bg2;
	private Sprite2D bg3;
	private float width; 
	private Random _random = new Random(); 

	public override void _Ready()
	{
		bg1 = GetNode<Sprite2D>("BG1");
		bg2 = GetNode<Sprite2D>("BG2");
		bg3 = GetNode<Sprite2D>("BG3");

		// --- CORRECCIÓN PUNTO 1: Estirar fondo automáticamente abajo ---
		// Medimos cuánto mide tu pantalla de juego (Viewport)
		float screenHeight = GetViewportRect().Size.Y;
		// Medimos cuánto mide tu imagen original sin escalar
		float textureHeight = bg1.Texture.GetHeight();
		
		// Calculamos matemáticamente cuánta escala necesitamos para cubrir todo el alto
		float neededYScale = screenHeight / textureHeight;
		
		// Aseguramos que la escala predeterminada (1,1) no rompa esto.
		// Aplicamos la escala horizontal que tú quieras (X) y la escala necesaria vertical (Y)
		Vector2 scaleFinal = new Vector2(BackgroundScale.X, Mathf.Max(BackgroundScale.Y, neededYScale));
		
		bg1.Scale = scaleFinal;
		bg2.Scale = scaleFinal;
		bg3.Scale = scaleFinal;
		// -------------------------------------------------------------

		// Calculamos el ancho de la imagen ya escalada (para el bucle infinito)
		width = bg1.Texture.GetWidth() * bg1.Scale.X;

		bg1.Position = new Vector2(0, 0);
		bg2.Position = new Vector2(width, 0);
		bg3.Position = new Vector2(width * 2, 0);
	}

	public override void _Process(double delta)
	{
		float move = BaseSpeed * (float)delta;
		
		bg1.Position -= new Vector2(move, 0);
		bg2.Position -= new Vector2(move, 0);
		bg3.Position -= new Vector2(move, 0);

		Loop(bg1);
		Loop(bg2);
		Loop(bg3);
	}

	private void Loop(Sprite2D bg)
	{
		if (bg.Position.X <= -width)
		{
			float rightmost = Mathf.Max(
				bg1.Position.X,
				Mathf.Max(bg2.Position.X, bg3.Position.X)
			);

			bg.Position = new Vector2(rightmost + width, 0);
		}
	}

	public void _on_ship_spawn_timer_timeout()
	{
		if (EscenaNave == null) return;

		Node2D nave = EscenaNave.Instantiate<Node2D>();
		AddChild(nave);

		// --- CORRECCIÓN PUNTO 2 y 4: Coordenadas dinámicas basadas en pantalla ---
		// Obtenemos el ancho de la pantalla actual para que funcione en cualquier resolución
		Vector2 viewportSize = GetViewportRect().Size;
		float screenWidth = viewportSize.X;
		float screenHeight = viewportSize.Y;
		
		// Un colchón grande de 600px garantiza que naves grandes no "aparezcan" de la nada
		float margin = 600.0f; 

		float farLeft = 0 - margin;
		float farRight = screenWidth + margin;

		int ladoInicio = _random.Next(0, 2);
		
		float startX = ladoInicio == 0 ? farLeft : farRight;
		float endX = ladoInicio == 0 ? farRight : farLeft;
		
		// Posiciones Y aleatorias dentro de la pantalla
		float startY = (float)_random.NextDouble() * (screenHeight - 100) + 50;
		float endY = (float)_random.NextDouble() * (screenHeight - 100) + 50;

		Vector2 startPos = new Vector2(startX, startY);
		Vector2 endPos = new Vector2(endX, endY);
		// -------------------------------------------------------------------------
		
		nave.Position = startPos;
		
		float duracionVuelo = (float)(_random.NextDouble() * 3 + 12); // Vuelo entre 4 y 7 segs
		float amplitudCurva = (_random.Next(0, 3) == 0 ? 300f : -300f); // Un poco más de amplitud para notar la curva

		Tween tween = GetTree().CreateTween();
		
		// Mantenemos tu genial idea del Seno
		tween.TweenMethod(Callable.From<float>((float progreso) => 
		{
			if (!IsInstanceValid(nave)) return;
			
			// Avanza en línea recta de A a B
			Vector2 basePos = startPos.Lerp(endPos, progreso);
			
			// Onda de la curva usando Sin
			float onda = Mathf.Sin(progreso * Mathf.Pi) * amplitudCurva;
			
			// Sacamos el vector perpendicular
			Vector2 direccion = (endPos - startPos).Normalized();
			Vector2 perpendicular = new Vector2(-direccion.Y, direccion.X);
			
			// Sumamos la onda a la posición base
			Vector2 nuevaPos = basePos + (perpendicular * onda);
			
			// Mirar hacia adelante + girar 180 grados (¡Punto 3 Check!)
			nave.LookAt(nuevaPos);
			nave.Rotation += Mathf.Pi; 
			
			nave.Position = nuevaPos;
			
		}), 0.0f, 1.0f, duracionVuelo);

		tween.TweenCallback(Callable.From(nave.QueueFree));
	}
}
