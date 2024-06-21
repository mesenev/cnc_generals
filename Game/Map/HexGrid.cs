using System;
using Lime;

namespace Game.Map;

public class HexGrid {
    public int width;
    public int height;

    public HexCell[,] cells;

    public HexGrid(Widget canvas, int width, int height, string spritePath = "Sprites/WhiteCell") {
        this.width = width;
        this.height = height;
        cells = new HexCell[height, width];

        for (int y = 0, i = 0; y < height; y++) {
            if (y != 0 && y % 2 == 0) {
                i++;
            }

            for (int x = 0; x < width; x++) {
                cells[y, x] = new HexCell(
                    canvas, new Vector2(x, y), x, y, width, height, spritePath
                );
                var coords = cells[y, x].HexPosition;
                cells[y, x].AxialCoords = new Vector2(coords.Y, coords.X - i);
            }
        }
    }

    public void ChangeColor(Color4 color) {
        foreach (var cell in cells) {
            cell.image.Color = color;
        }
    }

    public Vector2 getRandomCellPosition() {
        Random random = new Random();
        return cells[random.Next(0, height), random.Next(0, width)].PixelPosition;
    }
}
