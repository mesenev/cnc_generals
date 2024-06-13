using Lime;
using SharedObjects.GameObjects.Units;

namespace Game.GameObjects.Units;

public class AirUnit : IDrawableUnit {
    
    public AirUnit(SharedObjects.GameObjects.Units.AirUnit unit) {
        this.unit = unit;
    }

    public BaseUnit unit { get; set; }

    public Image GetImage() {
        return new Image {
            Sprite = new SerializableSprite("Sprites/Air"),
            Pivot = Vector2.Half,
        };
    }

}
