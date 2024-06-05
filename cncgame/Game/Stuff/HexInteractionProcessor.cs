using System;
using Game.Map;
using Game.Widgets;
using Lime;

namespace Game
{
	public class HexInteractionProcessor : IProcessor
	{
		private HexCell _cell;
		private Viewport2D viewport;

		public HexInteractionProcessor(HexCell cell, Viewport2D viewport)
		{
			this.viewport = viewport;
			_cell = cell;
		}

		public void Update(float delta, Game game)
		{
			var canvasInput = game.Canvas.Input;
			if (IsMouseOver(viewport.ViewportToWorldPoint(Window.Current.Input.MousePosition))) {
				_cell.image.Color = Color4.White;
				if (canvasInput.IsKeyPressed(Key.Mouse0)) {
					_cell.image.Color = Color4.Blue;
				}
			} else {
				_cell.image.Color = Color4.Gray;
			}
		}

		private bool IsMouseOver(Vector2 mousePos)
		{
			var sideSize = 20 * float.Abs(3) / 2;
			var pixelPos = _cell.PixelPosition;
			return Vector2.Distance(pixelPos, mousePos) < sideSize;
		}
	}
}
