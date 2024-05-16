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
			if (!_cell.image) {
				_cell.image.Color = Color4.White;
			} else {
				_cell.image.Color = Color4.Gray;
			}
		}
	}
}
