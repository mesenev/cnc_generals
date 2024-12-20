using Lime;
using SharedObjects.GameObjects.Units;

namespace Game.GameObjects.Units;

public class InfantryUnit : IDrawableUnit {
    public InfantryUnit(SharedObjects.GameObjects.Units.BaseUnit unit) {
        this.unit = unit;
    }
    public BaseUnit unit { get; set; }

    public Image GetImage() {
        return new Image {
            Sprite = new SerializableSprite("Sprites/Infantry"),
            Pivot = Vector2.Half,
        };
    }
}
