using System.Collections.Generic;
using System.IO;
using System.Linq;

namespace Game.GameObjects {
    public class Preset {
        public int GridHeight;
        public int GridWidth;
        public int UnitsAmount;
        public List<UnitInfo> UnitsInfo = [];
        private string _presetFolder = "GameStatePresets/";

        public Preset(string presetName) {
            var input = File.ReadAllLines(_presetFolder + presetName)
                .Where(x => !x.StartsWith('#'));

            var data = string.Join(" ", input).Split().Select(int.Parse).ToList();

            GridHeight = data[0];
            GridWidth = data[1];
            UnitsAmount = data[2];
            var units = new List<List<int>>();
            data = data.Slice(3, data.Count - 3);

            for (int i = 0; i < UnitsAmount; i++) {
                units.Add(data.Slice(i * 4, 4));
            }

            foreach (var unitData in units) {
                UnitsInfo.Add(new UnitInfo {
                    ownerId = unitData[0], unitType = unitData[1], x = unitData[2], y = unitData[3],
                });
            }

            return;
        }
    }

    public struct UnitInfo {
        public int ownerId;
        public int unitType;
        public int x;
        public int y;
    }
}
