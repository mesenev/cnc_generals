namespace Game.GameObjects.Units;

public class ArtilleryUnit : BaseUnit
{
    public ArtilleryUnit(uint unitId, uint ownerId, int x, int y) : base(unitId, ownerId,x,y)
    {
        CanMove = false;
        CanAttack = false;
        HasAbility = true;
        Health = 250;
        MovementSpeed = 0.5f;
        AttackSpeed = 0;
        AttackDamage = 0;
    }

    public void AttackCell(HexCell cell)
    {
        cell.GetCellUnit().Health -= 100;
    }
	
}