using System;
using LiteNetLib.Utils;

namespace Game.GameObjects.Units;

public class MarineUnit : BaseUnit
{
	public MarineUnit(uint unitId, uint ownerId, int x, int y) : base(unitId, ownerId, x, y)
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
