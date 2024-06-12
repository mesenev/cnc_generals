using Lime;

namespace Game.GameObjects.Units;

public class InfantryUnit : BaseUnit {
    public InfantryUnit(int unitId = 0, int ownerId = 0, int x = 0, int y = 0) : base(
        unitId, ownerId, x, y
    ) {
        unitType = 0;
        CanMove = true;
        CanAttack = true;
        HasAbility = false;
        Health = 100;
        MovementSpeed = 5f;
        AttackSpeed = 1;
        AttackDamage = 20;
        VisibleRadius = 3;
    }

    public override Image GetImage() {
        return new Image {
            Sprite = new SerializableSprite("Sprites/Infantry"),
            Pivot = Vector2.Half,
        };
    }
}
