using Game.Widgets;
using Lime;


namespace Game
{
	public class PlayerInputProcessor : IProcessor
	{
		private bool isKeyPressed = false;
		private PlayerComponent player;
		private Vector2 destination;
		private float speed = 5f;
		private Viewport2D viewport;

		public PlayerInputProcessor(PlayerComponent playerComponent, Viewport2D viewport)
		{
			this.viewport = viewport;
			player = playerComponent;
			destination = playerComponent.Position;
		}

		public void Update(float delta, Game game)
		{
			var direction = destination - player.Position;
			if (direction != Vector2.Zero) {
				player.Position += direction * speed * delta;
			}
			
			if (!isKeyPressed && game.Canvas.Input.IsKeyPressed(Key.Mouse1)) {
				destination = viewport.ViewportToWorldPoint(Window.Current.Input.MousePosition);
				isKeyPressed = true;
			}
			
			if (isKeyPressed && game.Canvas.Input.WasKeyReleased(Key.Mouse1)) {
				isKeyPressed = false;
			}
		}
	}
}
