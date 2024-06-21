using Game.Stuff;
using Lime;

namespace Game.Map;

public class StatusGrid {
    private Widget Canvas;

    public StatusGrid(int width, int height) {
        Canvas = CanvasManager.Instance.GetCanvas(Layers.TerrainStatus);
        var gridMask = new HexGrid(Canvas, width, height);
        gridMask.ChangeColor(Color4.Transparent);

        foreach (var cell in gridMask.cells) {
            AddCoords(cell);
            AddStatus(cell);
        }
    }

    public void AddStatus(HexCell cell) {
        var text = new SimpleText {
            Text = $"{cell.TerrainStatus}",
            TextColor = Color4.Black,
            FontHeight = 16,
            Position = cell.image.Position - new Vector2(-15, 15)
        };
        Canvas.AddNode(text);
    }

    private void AddCoords(HexCell cell) {
        var text = new SimpleText {
            Text = $"{cell.AxialCoords.Y},{cell.AxialCoords.X}",
            TextColor = Color4.Black,
            FontHeight = 16,
            Position = cell.image.Position - new Vector2(5, -10)
        };
        Canvas.AddNode(text);
    }
}
