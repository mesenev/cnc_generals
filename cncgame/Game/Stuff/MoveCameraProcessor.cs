using System;
using Game.Widgets;
using Lime;
namespace Game
{
	public class MoveCameraProcessor: IProcessor
	{
		private Camera2D camera;
		private bool isUpPressed = false;
		private bool isDownPressed = false;
		public MoveCameraProcessor(Camera2D camera)
		{
			this.camera = camera;
		}
		public void Update(float delta, Game game)
		{
			if (Window.Current.Input.IsKeyPressed(Key.Down)) {
				camera.Y += 1;
			}

			if (Window.Current.Input.IsKeyPressed(Key.Up)) {
				camera.Y -= 1;
			}
			if (Window.Current.Input.IsKeyPressed(Key.Left)) {
				camera.X -= 1;
			}
			if (Window.Current.Input.IsKeyPressed(Key.Right)) {
				camera.X += 1;
			}

			// if (isUpPressed) {
			// 	camera.Y -= 1;
			// }
			//
			// if (isDownPressed) {
			// 	camera.Y += 1;
			// }
		}
	}
}
