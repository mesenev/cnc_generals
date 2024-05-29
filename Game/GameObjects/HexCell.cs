using Game.GameObjects.Units;
using LiteNetLib.Utils;

namespace Game.GameObjects;

public class HexCell(int x, int y) : INetSerializable
{
    public BaseUnit CellUnit = null;
    public bool Occupied = false;
    public int x = x;
    public int y = y;

    public void UpdateCellUnit(BaseUnit newUnit)
    {
        CellUnit = newUnit;
        Occupied = true;
    }

    public void RemoveCellUnit()
    {
        CellUnit = null;
        Occupied = false;
    }

    public BaseUnit GetCellUnit()
    {
        return CellUnit;
    }

    public void Serialize(NetDataWriter writer)
    {
	    CellUnit.Serialize(writer);
	    writer.Put(Occupied);
	    writer.Put(x);
	    writer.Put(y);
    }

    public void Deserialize(NetDataReader reader)
    {
	    CellUnit.Deserialize(reader);
	    Occupied = reader.GetBool();
	    x = reader.GetInt();
	    y = reader.GetInt();
    }
}