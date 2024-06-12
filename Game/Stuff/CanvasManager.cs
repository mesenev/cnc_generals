using System.Collections.Generic;
using Lime;

namespace Game.Stuff;

public class CanvasManager {
    public static readonly CanvasManager Instance = new();

    private readonly Dictionary<int, Frame> canvases = new();

    public void InitLayers(Node parent) {
        CreateNewCanvas(Layers.Background, parent);
        CreateNewCanvas(Layers.HexMap, canvases[Layers.Background]);
        CreateNewCanvas(Layers.Occupation, canvases[Layers.HexMap]);
        CreateNewCanvas(Layers.Entities, canvases[Layers.Occupation]);
        CreateNewCanvas(Layers.FogMask, canvases[Layers.Entities]);
        CreateNewCanvas(Layers.TerrainStatus, canvases[Layers.FogMask]);
    }

    private void CreateNewCanvas(int layer, Node parent) {
        if (canvases.ContainsKey(layer)) return;
        var newCanvas = new Frame { Id = layer.ToString() };
        newCanvas.Unlink();
        parent.PushNode(newCanvas);
        canvases.Add(layer, newCanvas);
    }

    public Frame GetCanvas(int layer) {
        canvases.TryGetValue(layer, out Frame frame);
        return frame;
    }
}
