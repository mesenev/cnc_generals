using System;
using Game.GameObjects.Units;
using Lime;

namespace Game.Stuff;

public class PlayerInputProcessor(UnitComponent unitComponent) : IProcessor
{
    private bool isKeyPressed = false;
    private Vector2 destination = unitComponent.Position;
    private float speed = 5f;

    public void Update(float delta, GameObjects.Game game)
    {
			ArgumentNullException.ThrowIfNull(game);

			// var direction = destination - unitComponent.Position;
			// if (direction != Vector2.Zero) {
			// 	unitComponent.Position += direction * speed * delta;
			// }
			
			if (!isKeyPressed && game.Canvas.Input.IsKeyPressed(Key.Mouse1)) {
				// destination = game.Canvas.Input.MousePosition;
				isKeyPressed = true;
			}
			
			if (isKeyPressed && game.Canvas.Input.WasKeyReleased(Key.Mouse1)) {
				isKeyPressed = false;
			}
		}
}