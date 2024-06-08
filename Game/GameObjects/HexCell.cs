using LiteNetLib.Utils;

namespace Game.GameObjects;

public class HexCell(int xCoord = 1, int yCoord = 1) : INetSerializable {
    public int CellUnitId = -1;
    public int XCoord = xCoord;
    public int YCoord = yCoord;

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
    }

    public void Deserialize(NetDataReader reader) {
        CellUnitId = reader.GetInt();
        XCoord = reader.GetInt();
        YCoord = reader.GetInt();
    }
}
