using System.Numerics;
using LiteNetLib;
using LiteNetLib.Utils;
using SharedClasses.Commands;
using SharedClasses.GameObjects;

public class JoinPacket {
    public string username { get; set; }
}
public class JoinAcceptPacket {
    public GameState state { get; set; }
}
public struct ClientPlayer : INetSerializable {
    public string username;
    public uint playerId;

    public void Serialize(NetDataWriter writer) {
        writer.Put(username);
    }

    public void Deserialize(NetDataReader reader) {
        username = reader.GetString();
    }
}

public class ServerPlayer {
    public NetPeer peer;
    public string username;
    public uint playerId;
}

public class SendCommandPacket {
    public ICommand command { get; set; }
}

public class PlayerReceiveUpdatePacket {
    public GameState state { get; set; }
}

public class PlayerJoinedGamePacket {
    public ClientPlayer player { get; set; }
}

public class PlayerLeftGamePacket {
    public uint pid { get; set; }
}

public class PlayerAwaitPacket
{
	
}