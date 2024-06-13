using System;
using Game.Network.ClientPackets;
using LiteNetLib.Utils;

namespace Game.Commands;

public interface ICommand : INetSerializable {
    public BaseCommandPacket ToPacket();
    
    public void Execute();
}