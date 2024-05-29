using System.Numerics;

namespace SharedClasses.GameObjects.Units;

public class BaseUnit(HexCell position, uint unitId, uint ownerId)
{
    public int Health;
    public HexCell Position = position;
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
        Position = newPosition;
    }
}