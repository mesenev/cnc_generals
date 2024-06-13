using System.Collections.Generic;
using System.Linq;
using Game.GameObjects.Units;
using SharedObjects.GameObjects;
using SharedObjects.GameObjects.Units;
using Air = Game.GameObjects.Units.AirUnit;
using AirUnit = SharedObjects.GameObjects.Units.AirUnit;
using Artillery = Game.GameObjects.Units.ArtilleryUnit;
using ArtilleryUnit = SharedObjects.GameObjects.Units.ArtilleryUnit;
using Infantry = Game.GameObjects.Units.InfantryUnit;
using InfantryUnit = SharedObjects.GameObjects.Units.InfantryUnit;
using PlayerBase = SharedObjects.GameObjects.Units.PlayerBase;
using PlayerBaseDrawable = Game.GameObjects.Units.PlayerBase;

namespace Game.GameObjects;

public class ClientGameState {
    private GameState gameState;
    public HexGrid Grid => gameState.Grid;
    public List<Infantry> InfantryUnits { get; set; } = [];
    public List<Artillery> ArtilleryUnits { get; set; } = [];
    public List<Air> AirUnits { get; set; } = [];
    public List<PlayerBaseDrawable> PlayerBaseUnits { get; set; } = [];

    public IEnumerable<IDrawableUnit> Units => InfantryUnits.Cast<IDrawableUnit>()
        .Concat(ArtilleryUnits)
        .Concat(AirUnits)
        .Concat(PlayerBaseUnits);

    public ClientGameState(GameState gameState) {
        this.gameState = gameState;
        foreach (BaseUnit baseUnit in this.gameState.Units) {
            UnitsProcessing(baseUnit);
        }
    }

    private void UnitsProcessing(BaseUnit unit) {
        if (unit.GetType() == typeof(InfantryUnit))
            InfantryUnits.Add(new Infantry(unit as InfantryUnit));
        if (unit.GetType() == typeof(ArtilleryUnit))
            ArtilleryUnits.Add(new Artillery(unit as ArtilleryUnit));
        if (unit.GetType() == typeof(AirUnit))
            AirUnits.Add(new Air(unit as AirUnit));
        if (unit.GetType() == typeof(PlayerBase))
            PlayerBaseUnits.Add(new PlayerBaseDrawable(unit as PlayerBase));
    }
}
