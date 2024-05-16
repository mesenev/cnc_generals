using System;
using Lime;
using System.Linq;

namespace Game
{
	public class PlayerInputProcessor : IProcessor
	{
		private bool isKeyPressed = false;
		private PlayerComponent player;

		public PlayerInputProcessor(PlayerComponent playerComponent)
		{
			player = playerComponent;
		}

		public void Update(float delta, Game game)
		{
			if (!isKeyPressed && game.Canvas.Input.IsKeyPressed(Key.Mouse1)) {
				player.Position = new Vector2(game.Canvas.Input.MousePosition.X, game.Canvas.Input.MousePosition.Y);
				isKeyPressed = true;
			}

			if (isKeyPressed && game.Canvas.Input.WasKeyReleased(Key.Mouse1)) {
				isKeyPressed = false;
			}
		}
	}
}
