using System;
using Lime;

namespace Game.Map {
    public class HexGrid {
        public int width;
        public int height;

        public HexCell[,] cells;

        public HexGrid(Widget canvas, int width, int height) {
            this.width = width;
            this.height = height;
            cells = new HexCell[height, width];

            for (int y = 0; y < height; y++) {
                for (int x = 0; x < width; x++) {
                    cells[y, x] = new HexCell(
                        canvas, new Vector2(x, y), x, y, width, height
                    );
                }
            }
        }

        public Vector2 getRandomCellPosition() {
            Random random = new Random();
            return cells[random.Next(0, height), random.Next(0, width)].PixelPosition;
        }
    }
}
