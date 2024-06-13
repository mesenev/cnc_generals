using Lime;

namespace Game.GameObjects.Units;

public class UnitComponent : Component {
    private readonly int size = 32;

    public readonly Image Image;

    public Vector2 Position {
        get => Image.Position;
        set => Image.Position = value;
    }

    public UnitComponent(Widget canvas, Vector2 newPos, IDrawableUnit unit) {
        EntityId = unit.UnitId;
        Image = unit.GetImage();
        Image.Position = newPos;
        Image.Size = new Vector2(size, size);
        canvas.AddNode(Image);
    }
}