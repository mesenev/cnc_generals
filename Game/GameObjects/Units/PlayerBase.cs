using Lime;
using SharedObjects.GameObjects.Units;

namespace Game.GameObjects.Units;

public class PlayerBase : IDrawableUnit {

public PlayerBase(SharedObjects.GameObjects.Units.PlayerBase unit) {
    this.unit = unit;
}

public BaseUnit unit { get; set; }

public Image GetImage() {
        return new Image {
            Sprite = new SerializableSprite("Sprites/Base"),
            Pivot = Vector2.Half,
        };
    }
}