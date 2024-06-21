using System.Collections.Generic;
using Game.GameObjects.Units;
using Lime;

namespace Game.Map;

public class HexCell {
    public int XCoord;
    public int YCoord;
    public int GridX;
    public int GridY;
    public int CellUnitId = -1;
    public bool Occupied = false;
    public string TerrainStatus = "H";
    private int size = 64;
    public readonly Image image;
    private UnitComponent unitComponent;

    public Vector2 PixelPosition {
        get => image.Position;
        set => image.Position = value;
    }

    public Vector2 HexPosition;
    public Vector2 AxialCoords;

    public HexCell(Widget canvas, Vector2 newPos, int xCoord, int yCoord, int gridX,
        int gridY, string spritePath) {
        XCoord = xCoord;
        YCoord = yCoord;
        GridX = gridX;
        GridY = gridY;
        image = new Image {
            Sprite = new SerializableSprite(spritePath),
            Size = new Vector2(size, size),
            Pivot = Vector2.Half,
        };
        HexPosition = newPos;
        AxialCoords = newPos;
        image.Position = GetPosition(newPos.X, newPos.Y);
        canvas.AddNode(image);
    }

    public void SetUnit(UnitComponent unitComponent) {
        this.unitComponent = unitComponent;
    }

    public void EraseUnit(Widget canvas) {
        if (unitComponent == null) return;
        canvas.Nodes.Remove(unitComponent.Image);
        unitComponent = null;
    }

    public void DrawUnit(Widget canvas) {
        if (unitComponent == null) return;
        canvas.Nodes.Remove(unitComponent.Image);
        canvas.Nodes.Add(unitComponent.Image);
    }

    public void AddCoords(Widget canvas) {
        var text = new SimpleText {
            Text = $"{AxialCoords.Y},{AxialCoords.X}",
            TextColor = Color4.White,
            FontHeight = 16,
            Position = image.Position
        };
        canvas.PushNode(text);
    }

    public Vector2 GetPosition(float y, float x) {
        return new Vector2(
            // x * image.Height + ((y + 1) % 2 * image.Height / 2) + image.Height / 2,
            // (GridY - y) * 0.75f * image.Width
            y * image.Height + (x % 2 * image.Height / 2) + image.Height / 2,
            x * 0.75f * image.Width + image.Width / 2
        );
    }
}
