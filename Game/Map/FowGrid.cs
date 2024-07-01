using System;
using System.Collections.Generic;
using System.Linq;
using Game.GameObjects.Units;
using Game.Stuff;
using Lime;

namespace Game.Map;

public class FowGrid {
    private HexGrid gridMask;
    private Dictionary<Vector2, HexCell> axialGridMask = new();
    private List<IDrawableUnit> allies;

    public FowGrid(int width, int height, List<IDrawableUnit> allies) {
        UpdateAllies(allies);
        InitFow(width, height);
    }

    private void InitAxialGrid() {
        foreach (var cell in gridMask.cells) {
            axialGridMask.Add(cell.AxialCoords, cell);
        }
    }

    public void InitFow(int width, int height) {
        var canvas = CanvasManager.Instance.GetCanvas(Layers.FogMask);
        gridMask = new HexGrid(canvas, width, height);
        InitAxialGrid();

        RecalculateFow();
    }

    private void SetFog() {
        foreach (var cell in gridMask.cells) {
            cell.image.Color = Color4.Gray;
            cell.image.Opacity = 0.75f;
        }
    }

    public void RecalculateFow() {
        SetFog();
        var visibleArea = GetVisibleArea();
        foreach (var cell in gridMask.cells) {
            if (visibleArea.Contains(cell)) cell.image.Color = Color4.Transparent;
        }
    }

    public List<HexCell> GetVisibleArea() {
        List<HexCell> visibleArea = new();
        foreach (var unit in allies) {
            visibleArea.AddRange(GetVisibleCellsByUnit(unit));
        }

        return visibleArea;
    }

    public List<Vector2> GetVisibleCoords() {
        List<Vector2> visibleCoords = new();
        foreach (var unit in allies) {
            visibleCoords.AddRange(GetVisibleCellsByUnit(unit).Select(cell => cell.HexPosition));
        }

        return visibleCoords;
    }

    private List<HexCell> GetVisibleCellsByUnit(IDrawableUnit unit) {
        var visibleCells = new List<HexCell>();
        var visionRadius = unit.VisibleRadius;
        var unitCellAxialCoords = gridMask.cells[unit.Y, unit.X].AxialCoords;

        for (int i = -visionRadius; i <= visionRadius; i++) {
            for (int j = int.Max(-visionRadius, -i - visionRadius);
                 j <= int.Min(visionRadius, -i + visionRadius);
                 j++) {
                if (axialGridMask.TryGetValue(
                        unitCellAxialCoords + new Vector2(j, i),
                        out HexCell visibleCell
                    )) {
                    visibleCells.Add(visibleCell);
                }
            }
        }

        return visibleCells;
    }

    public void UpdateAllies(List<IDrawableUnit> newAllies) {
        allies = newAllies;
    }
}
