using Lime;
using SharedObjects;

namespace Game.GameObjects.Units;

public class PlayerBase : BaseUnit {
    public PlayerBase(
        int unitId = 0, int ownerId = 0, int x = 0, int y = 0, string nickname = ""
    ) : base(unitId, ownerId, x, y, nickname) {
        unitType = UnitType.PlayerBase;
        CanMove = false;
        CanAttack = true;
        MovementSpeed = 0;
        AttackSpeed = 1;
        AttackDamage = 10;
        VisibleRadius = 1;
    }

    public override Image GetImage() {
        return new Image {
            Sprite = new SerializableSprite("Sprites/Base"),
            Pivot = Vector2.Half,
        };
    }
}
