using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using Game.GameObjects.Units;
using BindingFlags = System.Reflection.BindingFlags;


namespace Game.GameObjects;

public class Preset {
    public int GridHeight;
    public int GridWidth;
    public int UnitsAmount;
    public List<UnitInfo> UnitsInfo = [];
    private string _presetFolder = "GameStatePresets/";

    public Preset(string presetName) {
        var input = File.ReadAllLines(_presetFolder + presetName)
            .Where(x => !x.StartsWith('#')).ToList();


        GridHeight = input.Count;
        GridWidth = input.First().Length;
        var x = 0;
        var y = 0;
        foreach (var lineFeed in input) {
            foreach (char symbol in lineFeed) {
                x += 1;
                var lower = Char.ToLower(symbol).ToString();
                if (!unitTypes.ContainsKey(lower))
                    continue;
                var owner = Char.IsLower(symbol) ? 0 : 1;
                var constructor = unitTypes[lower];
                
            }

            y += 1;
        }

        return;
    }


    private Dictionary<string, Type> unitTypes = new Dictionary<string, Type>() {
        { "i", typeof(InfantryUnit) },
        { "b", typeof(PlayerBase) },
        { "a", typeof(ArtilleryUnit) },
        { "v", typeof(AirUnit) },
    };
}

public struct UnitInfo {
    public int ownerId;
    public int unitType;
    public int x;
    public int y;
}
