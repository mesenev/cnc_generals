namespace Game.GameObjects.Units;

public class PlayerBase : BaseUnit {
    public PlayerBase(int unitId, int ownerId, int x, int y) : base(unitId, ownerId, x, y) {
        unitType = 4;
        CanMove = false;
        CanAttack = true;
        MovementSpeed = 0;
        AttackSpeed = 1;
        AttackDamage = 10;
    }
}
