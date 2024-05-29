using System;
using System.Numerics;

namespace SharedClasses.GameObjects.Units;

public class BaseUnit(uint unitId, uint ownerId)
{
    public int Health;
    public bool CanMove;
    public bool CanAttack;
    public bool HasAbility;
    public uint UnitId = unitId;
    public uint OwnerId = ownerId;
    public float MovementSpeed;
    public float AttackSpeed;
    public int AttackDamage;

    public void UpdatePosition(HexCell newPosition)
    {
	    throw new NotImplementedException();
    }
}