using System;
using Lime;

namespace Game.Map
{
	public class HexGrid
	{
		public int width = 40;
		public int height = 40;

		public HexCell[,] cells;

		public HexGrid(Widget canvas)
		{
			cells = new HexCell[height, width];

			for (int y = 0; y < height; y++) {
				for (int x = 0; x < width; x++) {
					cells[y, x] = new HexCell(canvas, new Vector2(x,y));
				}
			}
		}
		public Vector2 getRandomCellPosition()
		{
			Random random = new Random();
			return cells[random.Next(0, height), random.Next(0, width)].PixelPosition;
		}
	}
}
