﻿namespace Game.GameObjects.Units;

public class AirUnit : BaseUnit {
    public AirUnit(
        int unitId = 0, int ownerId = 0, int x = 0, int y = 0) : base(
        unitId, ownerId, x, y
    ) {
        unitType = 1;
        CanMove = false;
        CanAttack = false;
        HasAbility = true;
        Health = 250;
        MovementSpeed = 0.5f;
        AttackSpeed = 0;
        AttackDamage = 0;
    }

    public void AttackCell(GameState state, HexCell cell) {
        // cell.GetCellUnit().Health -= 100;
    }
}