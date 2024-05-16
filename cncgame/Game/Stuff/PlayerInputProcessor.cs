using System;
using Lime;
using System.Linq;

namespace Game
{
	public class PlayerInputProcessor : IProcessor
	{
		private bool isKeyPressed = false;
		private PlayerComponent player;
		private Vector2 destination;
		private float speed = 5f;

		public PlayerInputProcessor(PlayerComponent playerComponent)
		{
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
				destination = game.Canvas.Input.MousePosition;
				isKeyPressed = true;
			}
			
			if (isKeyPressed && game.Canvas.Input.WasKeyReleased(Key.Mouse1)) {
				isKeyPressed = false;
			}
		}
	}
}
