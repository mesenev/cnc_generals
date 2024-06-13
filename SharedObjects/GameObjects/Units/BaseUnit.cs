using Game.GameObjects.Orders;
using Lime;
using LiteNetLib.Utils;
using SharedObjects;

namespace Game.GameObjects.Units;



public abstract class BaseUnit(int unitId, int ownerId, int x, int y, string nickname)
    : INetSerializable {
    public int Health;
    public string nickname = nickname;
    protected UnitType unitType;
    public bool CanMove;
    public bool CanAttack;
    public bool HasAbility;
    public int UnitId = unitId;
    public int OwnerId = ownerId;
    public float MovementSpeed;
    public float AttackSpeed;
    public int AttackDamage;
    public int VisibleRadius;
    public IOrder CurrentOrder;
    public int x = x;
    public int y = y;


    public void UpdatePosition(HexCell newPosition) {
        x = newPosition.XCoord;
        y = newPosition.YCoord;
        newPosition.CellUnitId = unitId;
    }

    public static BaseUnit CreateUnitByType(
        int unitType, int unitId, int ownerId, int x, int y, string nickname
        ) {
        if (unitType == 0) {
            return new InfantryUnit(unitId, ownerId, x, y);
        }

        if (unitType == 1) {
            return new ArtilleryUnit(unitId, ownerId, x, y);
        }

        return new InfantryUnit(unitId, ownerId, x, y);
    }

    public void Serialize(NetDataWriter writer) {
        writer.Put((int)unitType);
        writer.Put(UnitId);
        writer.Put(OwnerId);
        writer.Put(Health);
        writer.Put(x);
        writer.Put(y);
    }

    public virtual void Deserialize(NetDataReader reader) {
        unitType = (UnitType)reader.GetInt();
        UnitId = reader.GetInt();
        OwnerId = reader.GetInt();
        Health = reader.GetInt();
        x = reader.GetInt();
        y = reader.GetInt();
    }
}
