using LiteNetLib.Utils;
using SharedClasses.GameObjects;
using SharedClasses.GameObjects.Units;
using System;

namespace SharedClasses.Commands;

public class MoveCommand : ICommand, INetSerializable
{
    private BaseUnit _unit;
    private HexCell _cell;
    public void Execute()
    {
        if (_cell.Occupied)
        {
            _cell.UpdateCellUnit(_unit);
            _unit.UpdatePosition(_cell);
        }
        else
        {
            Console.WriteLine($"Can't move unit to cell: occupied");
        }
    }

    public void Serialize(NetDataWriter writer)
    {
        throw new NotImplementedException();
    }

    public void Deserialize(NetDataReader reader)
    {
        throw new NotImplementedException();
    }
}