using System;
using LiteNetLib.Utils;

namespace Game.GameObjects;

public class HexGrid : INetSerializable
{
	public int width;
	public int height;

	public HexCell[,] cells;

	public HexGrid(int gridHeight, int gridWidth) {
        height = gridHeight;
        width = gridWidth;
		cells = new HexCell[height, width];

		for (int y = 0; y < height; y++) {
			for (int x = 0; x < width; x++) {
				cells[y, x] = new HexCell(x, y);
			}
		}
	}

	public HexCell GetRandomCell()
	{
		Random random = new Random();
		return cells[random.Next(0, height), random.Next(0, width)];
	}

	public HexCell GetCell(int x, int y)
	{
		return cells[y, x];
	}

	public void Serialize(NetDataWriter writer)
	{
		writer.Put(height);
		writer.Put(width);
		for (var y = 0; y < height; y++) {
			for (var x = 0; x < width; x++) {
				cells[y, x].Serialize(writer);
			}
		}
	}

	public void Deserialize(NetDataReader reader)
	{
		height = reader.GetInt();
		width = reader.GetInt();
		cells = new HexCell[height, width];

		for (var y = 0; y < height; y++) {
			for (var x = 0; x < width; x++) {
				// cells[y, x] = new HexCell(x, y);
				cells[y, x].Deserialize(reader);
			}
		}
	}
}
