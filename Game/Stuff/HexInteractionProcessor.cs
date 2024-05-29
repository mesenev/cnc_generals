using System;
using Game.Map;
using Lime;

namespace Game
{
	public class HexInteractionProcessor : IProcessor
	{
		private HexCell _cell;

		public HexInteractionProcessor(HexCell cell)
		{
			_cell = cell;
		}

		public void Update(float delta, Game game)
		{
			var canvasInput = game.Canvas.Input;
			if (IsMouseOver(canvasInput.MousePosition)) {
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
