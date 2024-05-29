using System;

namespace SharedClasses.GameObjects;

public class HexGrid
{
    public int width = 20;
    public int height = 11;

    public HexCell[,] cells;

    public HexGrid()
    {
        cells = new HexCell[height, width];

        for (int y = 0; y < height; y++) {
            for (int x = 0; x < width; x++) {
                cells[y, x] = new HexCell();
            }
        }
    }
    public HexCell GetRandomCell()
    {
        Random random = new Random();
        return cells[random.Next(0, height), random.Next(0, width)];
    }
}