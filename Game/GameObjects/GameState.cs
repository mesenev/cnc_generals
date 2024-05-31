using System;
using System.Collections.Generic;
using Game.GameObjects.Units;
using LiteNetLib.Utils;

namespace Game.GameObjects;

public class GameState : INetSerializable {
    public HexGrid Grid;
    public List<BaseUnit> Units = [];
    private uint unitIdCounter = 0;

    public GameState(Preset preset) {
        Grid = new HexGrid(preset.GridHeight, preset.GridWidth);
        for (int i = 0; i < preset.UnitsAmount; i++) {
            UnitInfo info = preset.UnitsInfo[i];
            AddUnit(info.unitType, info.ownerId, info.x, info.y);
        }
    }

    public void AddUnit(uint unitType, uint ownerId, int x, int y) {
        BaseUnit newUnit = default;
        if (unitType == 0)
            newUnit = new MarineUnit(unitIdCounter, ownerId, x, y);
        if (unitType == 1)
            newUnit = new ArtilleryUnit(unitIdCounter, ownerId, x, y);
        Units.Add(newUnit);
        Grid.cells[newUnit.y,newUnit.x].UpdateCellUnit(newUnit);
        unitIdCounter++;
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

    public void PrintGameState() {
        for (int y = Grid.height - 1; y >= 0; y--) {
            for (int x = 0; x < Grid.width; x++) {
                string sign = Grid.cells[y, x].Occupied ? (Grid.cells[y, x].CellUnit.OwnerId == 0 ? "?" : "!") : " ";
                Console.Write($"|{sign}");
            }

            Console.Write("\n" + (y % 2 == 0 ? "": " "));
        }
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
