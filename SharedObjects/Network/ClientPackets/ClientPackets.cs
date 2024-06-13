using Game.Commands;

namespace Game.Network.ClientPackets;

public abstract class BaseCommandPacket { 
}


public class MoveCommandPacket : BaseCommandPacket {
    public MoveCommand Command { get; set; }
}

public class OrderUnitPacket : BaseCommandPacket {
    public OrderUnitCommand Command { get; set; }
}
