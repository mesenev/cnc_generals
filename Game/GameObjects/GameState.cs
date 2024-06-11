using System;
using System.Collections.Generic;
using System.Linq;
using Game.Commands;
using Game.GameObjects.Orders;
using Game.GameObjects.Units;
using LiteNetLib.Utils;

namespace Game.GameObjects;

public class GameState : INetSerializable {
    public HexGrid Grid;
    public List<InfantryUnit> MarineUnits = [];
    public List<ArtilleryUnit> ArtilleryUnits = [];
    public List<AirUnit> AirUnits = [];
    public List<PlayerBase> PlayerBases = [];
    public List<MoveOrder> MoveOrders = [];
    private int unitIdCounter;
    public TimeSpan ElapsedGameTime { get; set; } = new(0);
    public bool GameInitiated { get; set; }
    public bool IsPaused { get; set; } = true;


    public GameState(Preset preset) {
        Grid = new HexGrid(preset.GridHeight, preset.GridWidth);
        foreach (var info in preset.UnitsInfo) {
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
                .Concat(AirUnits)
                .Concat(PlayerBases);
        }
    }

    public IEnumerable<IOrder> Orders {
        get { return MoveOrders; }
    }


    public BaseUnit GetUnitById(int id) {
        return Units.First(unit => unit.UnitId == id);
    }

    public void AddUnit(UnitType unitType, int ownerId, int x, int y) {
        if (unitType == UnitType.InfantryUnit)
            MarineUnits.Add(new InfantryUnit(unitIdCounter, ownerId, x, y));
        if (unitType == UnitType.ArtilleryUnit)
            ArtilleryUnits.Add(new ArtilleryUnit(unitIdCounter, ownerId, x, y));
        if (unitType == UnitType.AirUnit)
            AirUnits.Add(new AirUnit(unitIdCounter, ownerId, x, y));
        if (unitType == UnitType.PlayerBase)
            PlayerBases.Add(new PlayerBase(unitIdCounter, ownerId, x, y));

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

    public void AddOrder(IOrder order) {
        if (order.GetType() == typeof(MoveOrder)) {
            MoveOrders.Add(order as MoveOrder);
        }
    }

    public void RemoveOrder(IOrder orderToRemove) {
        if (orderToRemove.GetType() == typeof(MoveOrder)) {
            MoveOrders.Remove(orderToRemove as MoveOrder);
        }
    }

    public void Update(TimeSpan timeDelta) {
        if (!GameInitiated | IsPaused)
            return;

        foreach (IOrder order in Orders) {
            if (order.Update(this) == OrderStatus.Finished)
                RemoveOrder(order);
        }

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
        writer.Put(PlayerBases.Count);
        writer.Put(MoveOrders.Count);

        foreach (var unit in MarineUnits)
            unit.Serialize(writer);

        foreach (var unit in ArtilleryUnits)
            unit.Serialize(writer);

        foreach (var unit in PlayerBases)
            unit.Serialize(writer);

        foreach (var order in MoveOrders) {
            order.Serialize(writer);
        }
    }

    public void Deserialize(NetDataReader reader) {
        Grid.Deserialize(reader);
        int marineCount = reader.GetInt();
        int artilleryCount = reader.GetInt();
        int airCount = reader.GetInt();
        int basesCount = reader.GetInt();
        int moveOrdersCount = reader.GetInt();
        for (var i = 0; i < marineCount; i++)
            MarineUnits.Add(reader.Get(() => new InfantryUnit()));

        for (var i = 0; i < artilleryCount; i++)
            ArtilleryUnits.Add(reader.Get(() => new ArtilleryUnit()));

        for (var i = 0; i < airCount; i++)
            AirUnits.Add(reader.Get(() => new AirUnit()));

        for (var i = 0; i < basesCount; i++)
            PlayerBases.Add(reader.Get(() => new PlayerBase()));

        for (var i = 0; i < moveOrdersCount; i++)
            MoveOrders.Add(reader.Get(() => new MoveOrder()));

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
