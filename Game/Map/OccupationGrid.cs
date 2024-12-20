using System.Collections.Generic;
using Game.GameObjects.Units;
using Game.Stuff;
using Lime;
using SharedObjects.GameObjects.Units;

namespace Game.Map;

public class OccupationGrid {
    private HexGrid gridMask;

    private Color4 allyColor;
    private Color4 enemyColor;

    public OccupationGrid(int width, int height) {
        SetColors();
        var canvas = CanvasManager.Instance.GetCanvas(Layers.Occupation);
        gridMask = new HexGrid(canvas, width, height);
        Clear();
    }

    private void SetColors() {
        allyColor = new Color4(87, 246, 57);
        enemyColor = new Color4(255, 70, 70);
    }

    private void Clear() {
        gridMask.ChangeColor(Color4.Transparent);
    }

    public void Occupy(List<IDrawableUnit> units, List<Vector2> visibleCoords, int playerId) {
        Clear();
        foreach (var unit in units) {
            if (visibleCoords.Contains(gridMask.cells[unit.Y, unit.X].HexPosition)) {
                gridMask.cells[unit.Y, unit.X].image.Color =
                    unit.OwnerId == playerId ? allyColor : enemyColor;
            }
        }
    }
}
