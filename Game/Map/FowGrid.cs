using System.Collections.Generic;
using Game.Stuff;
using Lime;
using SharedObjects.GameObjects.Units;

namespace Game.Map {
    public class FowGrid {
        private HexGrid gridMask;
        private Dictionary<Vector2, HexCell> axialGridMask = new();
        private List<BaseUnit> units;

        public FowGrid(List<BaseUnit> units) {
            UpdateUnits(units);
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
            }
        }

        public void RecalculateFow() {
            SetFog();
            foreach (var unit in units) {
                var visibleUnitCells = GetVisibleCellsByUnit(unit);
                foreach (var cell in gridMask.cells) {
                    if (visibleUnitCells.Contains(cell)) cell.image.Color = Color4.Transparent;
                }
            }
        }

        private List<HexCell> GetVisibleCellsByUnit(BaseUnit unit) {
            var visibleCells = new List<HexCell>();
            var visionRadius = unit.VisibleRadius;
            var unitCellAxialCoords = gridMask.cells[unit.y, unit.x].AxialCoords;

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

        public void UpdateUnits(List<BaseUnit> newUnits) {
            this.units = newUnits;
        }
    }
}
