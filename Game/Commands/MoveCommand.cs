using System;
using Game.GameObjects;
using Game.GameObjects.Units;
using LiteNetLib.Utils;

namespace Game.Commands;

public class MoveCommand : ICommand {
    private BaseUnit _unit;
    private HexCell _cell;

    public void Execute() {
        if (_cell.Occupied) {
            _cell.UpdateCellUnit(_unit.UnitId);
            _unit.UpdatePosition(_cell);
        } else {
            Console.WriteLine($"Can't move unit to cell: occupied");
        }
    }

    public void Serialize(NetDataWriter writer) {
        _unit.Serialize(writer);
        _cell.Serialize(writer);
    }

    public void Deserialize(NetDataReader reader) {
        _unit.Deserialize(reader);
        _cell.Deserialize(reader);
    }
}
