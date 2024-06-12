using LiteNetLib.Utils;

namespace Game.GameObjects;

public class HexCell(int xCoord = 1, int yCoord = 1) : INetSerializable {
    public int CellUnitId = -1;
    public int XCoord = xCoord;
    public int YCoord = yCoord;

    //Cost from start cell to current
    public int g = 0;

    //Cost from current cell to destination tile
    public int h = 0;
    public int F => g + h;

    public void UpdateCellUnit(int unitId) {
        CellUnitId = unitId;
    }

    public void RemoveCellUnit() {
        CellUnitId = -1;
    }

    public void Serialize(NetDataWriter writer) {
        writer.Put(CellUnitId);
        writer.Put(XCoord);
        writer.Put(YCoord);
        writer.Put(g);
        writer.Put(h);
    }

    public void Deserialize(NetDataReader reader) {
        CellUnitId = reader.GetInt();
        XCoord = reader.GetInt();
        YCoord = reader.GetInt();
        g = reader.GetInt();
        h = reader.GetInt();
    }
}
