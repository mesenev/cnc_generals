namespace SharedObjects.GameObjects.Units;

public class ArtilleryUnit : BaseUnit {
    public ArtilleryUnit(int unitId = 0, int ownerId = 0, int x = 0, int y = 0, string nickname = "") : base(
        unitId, ownerId, x, y, nickname
    ) {
        Type = UnitType.ArtilleryUnit;
        CanMove = false;
        CanAttack = false;
        HasAbility = true;
        Health = 250;
        MovementSpeed = 0.5f;
        AttackSpeed = 0;
        AttackDamage = 0;
        VisibleRadius = 1;
    }


    public void AttackCell(GameState state, HexCell cell) {
        // cell.GetCellUnit().Health -= 100;
    }
}
