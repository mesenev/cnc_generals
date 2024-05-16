using Lime;

namespace Game.Map
{
	public class HexGrid
	{
		public int width = 20;
		public int height = 11;

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
	}
}
