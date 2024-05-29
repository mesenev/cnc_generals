using SharedClasses.Commands;

namespace SharedClasses.GameObjects.Units;

public class ArtilleryUnit : BaseUnit
{
    public ArtilleryUnit(HexCell position, uint unitId, uint ownerId) :
        base(position, unitId, ownerId)
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