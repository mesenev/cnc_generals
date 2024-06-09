using System;
using System.Collections.Generic;
using System.Linq;
using Game.Commands;
using Game.GameObjects.Units;
using LiteNetLib.Utils;

namespace Game.GameObjects;

public class GameState : INetSerializable {
    public HexGrid Grid;
    public List<InfantryUnit> MarineUnits = [];
    public List<ArtilleryUnit> ArtilleryUnits = [];
    public List<AirUnit> AirUnits = [];
    private int unitIdCounter;
    public TimeSpan ElapsedGameTime { get; set; } = new(0);
    public bool GameInitiated { get; set; }
    public bool IsPaused { get; set; } = true;


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
                .Concat(ArtilleryUnits.Cast<BaseUnit>())
                .Concat(AirUnits);
        }
    }


    public BaseUnit GetUnitById(int id) {
        return Units.First(unit => unit.UnitId == id);
    }

    public void AddUnit(int unitType, int ownerId, int x, int y) {
        if (unitType == 0)
            MarineUnits.Add(new InfantryUnit(unitIdCounter, ownerId, x, y));
        if (unitType == 1)
            ArtilleryUnits.Add(new ArtilleryUnit(unitIdCounter, ownerId, x, y));
        if (unitType == 2)
            AirUnits.Add(new AirUnit(unitIdCounter, ownerId, x, y));
        Grid.cells[y, x].UpdateCellUnit(unitIdCounter);
        unitIdCounter++;
    }

    public void RemoveUnit(BaseUnit unitToRemove) {
        if (unitToRemove.GetType() == typeof(InfantryUnit)) {
            MarineUnits.Remove(unitToRemove as InfantryUnit);
        }

        if (unitToRemove.GetType() == typeof(ArtilleryUnit)) {
            ArtilleryUnits.Remove(unitToRemove as ArtilleryUnit);
        }
    }

    public void Update(TimeSpan timeDelta) {
        if (!GameInitiated | IsPaused)
            return;

        ElapsedGameTime += timeDelta;
    }

    public string GameStateAsString() {
        var answer = "";
        for (int y = Grid.height - 1; y >= 0; y--) {
            answer += " ";
            for (var x = 0; x < Grid.width; x++) {
                var sign = " ";

                if (Grid.cells[y, x].CellUnitId != -1)
                    sign = GetUnitById(Grid.cells[y, x].CellUnitId).OwnerId == 0 ? "?" : "!";

                answer += ($"{sign}");
            }

            answer += ("\n" + (y % 2 == 0 ? "" : " "));
        }

        return answer;
    }

    public void Serialize(NetDataWriter writer) {
        Grid.Serialize(writer);
        writer.Put(MarineUnits.Count);
        writer.Put(ArtilleryUnits.Count);
        writer.Put(AirUnits.Count);
        foreach (var unit in MarineUnits)
            unit.Serialize(writer);

        foreach (var unit in ArtilleryUnits)
            unit.Serialize(writer);

        foreach (var unit in AirUnits)
            unit.Serialize(writer);
    }

    public void Deserialize(NetDataReader reader) {
        Grid.Deserialize(reader);
        int marineCount = reader.GetInt();
        int artilleryCount = reader.GetInt();
        int airCount = reader.GetInt();
        for (var i = 0; i < marineCount; i++)
            MarineUnits.Add(reader.Get(() => new InfantryUnit()));

        for (var i = 0; i < artilleryCount; i++)
            ArtilleryUnits.Add(reader.Get(() => new ArtilleryUnit()));

        for (var i = 0; i < airCount; i++)
            AirUnits.Add(reader.Get(() => new AirUnit()));

        return;
    }

    public void InitializeWorld() {
        GameInitiated = true;
        IsPaused = false;
    }

    public void OrderUnit(OrderUnitCommand packetCommand) {
        AddUnit(packetCommand.UnitType, packetCommand.UserId, 6, 6);
    }
}
