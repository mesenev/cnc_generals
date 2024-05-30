using System;
using LiteNetLib.Utils;

namespace Game.GameObjects.Units;

public abstract class BaseUnit(uint UnitId, uint OwnerId, int x, int y) : INetSerializable
{
	public int Health;
	public int unitType;
	public bool CanMove;
	public bool CanAttack;
	public bool HasAbility;
	public uint UnitId;
	public uint OwnerId;
	public float MovementSpeed;
	public float AttackSpeed;
	public int AttackDamage;
	public int x;
	public int y;

	public void UpdatePosition(HexCell newPosition)
	{
		x = newPosition.x;
		y = newPosition.y;
	}

	public void Serialize(NetDataWriter writer)
	{
		writer.Put(unitType);
		writer.Put(UnitId);
		writer.Put(OwnerId);
		writer.Put(Health);
		writer.Put(x);
		writer.Put(y);
	}

	public static BaseUnit CreateUnitByType(int unitType, uint unitId, uint ownerId, int x, int y)
	{
		if (unitType == 0) {
			return new MarineUnit(unitId, ownerId, x, y);
		}

		if (unitType == 1) {
			return new ArtilleryUnit(unitId, ownerId, x, y);
		}

		return new MarineUnit(unitId, ownerId, x, y);
	}

	public virtual void Deserialize(NetDataReader reader)
	{
		unitType = reader.GetInt();
		UnitId = reader.GetUInt();
		OwnerId = reader.GetUInt();
		Health = reader.GetInt();
		x = reader.GetInt();
		y = reader.GetInt();
	}
}
