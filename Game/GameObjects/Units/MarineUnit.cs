namespace SharedClasses.GameObjects.Units;

public class MarineUnit : BaseUnit
{
	public MarineUnit( uint unitId, uint ownerId) : base(unitId, ownerId)
	{
		CanMove = true;
		CanAttack = true;
		HasAbility = false;
		Health = 100;
		MovementSpeed = 5f;
		AttackSpeed = 1;
		AttackDamage = 20;
	}
}
