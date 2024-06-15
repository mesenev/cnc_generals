using Lime;

namespace Game.GameObjects.Units;

public interface IDrawableUnit {
    public SharedObjects.GameObjects.Units.BaseUnit unit { get; set; }
    public Image GetImage();
    public int UnitId => unit.UnitId;
    public int OwnerId => unit.PlayerId;
    public int VisibleRadius => unit.VisibleRadius;
    public int Y => unit.Y;
    public int X => unit.X;
}
