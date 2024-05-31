using LiteNetLib.Utils;

namespace Game.GameObjects;

public class HexCell(int xCoord = 1, int yCoord = 1) : INetSerializable {
    public int CellUnitId = -1;
    public bool Occupied;
    public int XCoord = xCoord;
    public int YCoord = yCoord;

    public void UpdateCellUnit(int unitId) {
        CellUnitId = unitId;
        Occupied = true;
    }

    public void RemoveCellUnit() {
        CellUnitId = -1;
        Occupied = false;
    }

    public void Serialize(NetDataWriter writer) {
        writer.Put(CellUnitId);
        writer.Put(Occupied);
        writer.Put(XCoord);
        writer.Put(YCoord);
    }

    public void Deserialize(NetDataReader reader) {
        CellUnitId = reader.GetInt();
        Occupied = reader.GetBool();
        XCoord = reader.GetInt();
        YCoord = reader.GetInt();
    }
}
