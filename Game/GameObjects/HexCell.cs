using SharedClasses.GameObjects.Units;

namespace SharedClasses.GameObjects;

public class HexCell
{
    public BaseUnit? CellUnit = null;
    public bool Occupied = false;

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
}