namespace SharedObjects.GameObjects.Units;

public class PlayerBase : BaseUnit {
    public PlayerBase(
        int unitId = 0, int ownerId = 0, int x = 0, int y = 0, string nickname = ""
    ) : base(unitId, ownerId, x, y, nickname) {
        Type = UnitType.PlayerBase;
        CanMove = false;
        CanAttack = true;
        MovementSpeed = 0;
        AttackSpeed = 1;
        AttackDamage = 10;
        VisibleRadius = 1;
    }
}