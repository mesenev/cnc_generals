using LiteNetLib.Utils;

namespace Game.GameObjects.Units;

public abstract class BaseUnit(int unitId = 0, int ownerId = 0, int x = 0, int y = 0) : INetSerializable {
    public int Health;
    public int unitType;
    public bool CanMove;
    public bool CanAttack;
    public bool HasAbility;
    public int UnitId = unitId;
    public int OwnerId = ownerId;
    public float MovementSpeed;
    public float AttackSpeed;
    public int AttackDamage;
    public int x = x;
    public int y = y;

    public void UpdatePosition(HexCell newPosition) {
        x = newPosition.XCoord;
        y = newPosition.YCoord;
    }

    public static BaseUnit CreateUnitByType(int unitType, int unitId, int ownerId, int x, int y) {
        if (unitType == 0) {
            return new MarineUnit(unitId, ownerId, x, y);
        }

        if (unitType == 1) {
            return new ArtilleryUnit(unitId, ownerId, x, y);
        }

        return new MarineUnit(unitId, ownerId, x, y);
    }

    public void Serialize(NetDataWriter writer) {
        writer.Put(unitType);
        writer.Put(UnitId);
        writer.Put(OwnerId);
        writer.Put(Health);
        writer.Put(x);
        writer.Put(y);
    }

    public virtual void Deserialize(NetDataReader reader) {
        unitType = reader.GetInt();
        UnitId = reader.GetInt();
        OwnerId = reader.GetInt();
        Health = reader.GetInt();
        x = reader.GetInt();
        y = reader.GetInt();
    }
}
