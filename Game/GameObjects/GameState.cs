using System;
using System.Collections.Generic;
using System.Linq;
using Game.GameObjects.Units;
using LiteNetLib.Utils;

namespace Game.GameObjects;

public class GameState : INetSerializable {
    public HexGrid Grid;
    public List<MarineUnit> MarineUnits = [];
    public List<ArtilleryUnit> ArtilleryUnits = [];
    private int unitIdCounter;

    public GameState(Preset preset) {
        Grid = new HexGrid(preset.GridHeight, preset.GridWidth);
        for (var i = 0; i < preset.UnitsAmount; i++) {
            UnitInfo info = preset.UnitsInfo[i];
            AddUnit(info.unitType, info.ownerId, info.x, info.y);
        }
    }

    public GameState() {
        Grid = new HexGrid(0, 0);
    }


    public IEnumerable<BaseUnit> Units {
        get {
            return MarineUnits
                .Concat(ArtilleryUnits.Cast<BaseUnit>());
        }
    }

    public BaseUnit GetUnitById(int id) {
        return Units.First(unit => unit.UnitId == id);
    }

    public void AddUnit(int unitType, int ownerId, int x, int y) {
        if (unitType == 0)
            MarineUnits.Add(new MarineUnit(unitIdCounter, ownerId, x, y));
        if (unitType == 1)
            ArtilleryUnits.Add(new ArtilleryUnit(unitIdCounter, ownerId, x, y));
        Grid.cells[y, x].UpdateCellUnit(unitIdCounter);
        unitIdCounter++;
    }

    public void RemoveUnit(BaseUnit unitToRemove) {
        if (unitToRemove.GetType() == typeof(MarineUnit)) {
            MarineUnits.Remove(unitToRemove as MarineUnit);
        }

        if (unitToRemove.GetType() == typeof(ArtilleryUnit)) {
            ArtilleryUnits.Remove(unitToRemove as ArtilleryUnit);
        }
    }

    public void Update(TimeSpan timeDelta) {
        throw new NotImplementedException();
    }

    public void PrintGameState() {
        for (int y = Grid.height - 1; y >= 0; y--) {
            for (var x = 0; x < Grid.width; x++) {
                var sign = " ";

                if (Grid.cells[y, x].Occupied)
                    sign = GetUnitById(Grid.cells[y, x].CellUnitId).OwnerId == 0 ? "?" : "!";

                Console.Write($"|{sign}");
            }

            Console.Write("\n" + (y % 2 == 0 ? "" : " "));
        }
    }

    public void Serialize(NetDataWriter writer) {
        Grid.Serialize(writer);
        writer.Put(MarineUnits.Count);
        writer.Put(ArtilleryUnits.Count);
        foreach (var unit in MarineUnits)
            unit.Serialize(writer);


        foreach (var unit in ArtilleryUnits)
            unit.Serialize(writer);
    }

    public void Deserialize(NetDataReader reader) {
        Grid.Deserialize(reader);
        int marineCount = reader.GetInt();
        int artilleryCount = reader.GetInt();
        for (var i = 0; i < marineCount; i++)
            MarineUnits.Add(reader.Get(() => new MarineUnit()));


        for (var i = 0; i < artilleryCount; i++)
            ArtilleryUnits.Add(reader.Get(() => new ArtilleryUnit()));

        return;
    }
}
