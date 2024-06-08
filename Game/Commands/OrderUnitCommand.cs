using System;
using Game.Network.ClientPackets;
using LiteNetLib.Utils;

namespace Game.Commands;

public struct OrderUnitCommand(int userId, int unitType) : ICommand {
    public int UserId = userId;
    public int UnitType = unitType;

    public void Serialize(NetDataWriter writer) {
        writer.Put(UserId);
        writer.Put(UnitType);
    }

    public void Deserialize(NetDataReader reader) {
        UserId = reader.GetInt();
        UnitType = reader.GetInt();
    }

    public BaseCommandPacket ToPacket() {
        return new OrderUnitPacket { Command = this };
    }

    public void Execute() {
        throw new System.NotImplementedException();
    }
}
