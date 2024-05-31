using System;
using System.Collections.Generic;
using Game.GameObjects.Units;
using LiteNetLib.Utils;

namespace Game.GameObjects;

public class GameState : INetSerializable {
    public HexGrid Grid = new();
    public List<BaseUnit> Units = [];

    public void AddUnit(BaseUnit newUnit) {
        Units.Add(newUnit);
    }

    public void RemoveUnit(BaseUnit unitToRemove) {
        Units.Remove(unitToRemove);
    }

    public BaseUnit GetUnitById(uint unitId) {
        return Units.Find(unit => unit.UnitId == unitId);
    }

    public void Update(TimeSpan timeDelta) {
        throw new NotImplementedException();
    }

    public void Serialize(NetDataWriter writer) {
        Grid.Serialize(writer);
        writer.Put(Units.Count);
        foreach (var unit in Units) {
            unit.Serialize(writer);
        }
    }

    public void Deserialize(NetDataReader reader) {
        Grid.Deserialize(reader);
        for (var i = 0; i < reader.GetInt(); i++) { }
        //
    }
}
