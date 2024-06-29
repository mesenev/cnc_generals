using Lime;
using SharedObjects.GameObjects.Units;

namespace Game.GameObjects.Units;

public class DrawableArtilleryUnit: IDrawableUnit {

    public DrawableArtilleryUnit(SharedObjects.GameObjects.Units.ArtilleryUnit unit) {
        this.unit = unit;
    }

    public BaseUnit unit { get; set; }

    public Image GetImage() {
        return new Image {
            Sprite = new SerializableSprite("Sprites/Artillery"),
            Pivot = Vector2.Half,
        };
    }

    public int UnitId => unit.UnitId;
}
