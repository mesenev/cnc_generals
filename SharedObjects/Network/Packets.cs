using LiteNetLib;
using LiteNetLib.Utils;
using SharedObjects.GameObjects;

namespace SharedObjects.Network;

public class JoinPacket {
    public string username { get; set; }
}

public class JoinAcceptPacket {
    public GameState state { get; set; }
    public ClientPlayer player { get; set; }
}

public struct ClientPlayer : INetSerializable {
    public int playerId;
    public string username;

    public void Serialize(NetDataWriter writer) {
        writer.Put(playerId);
        writer.Put(username);
    }

    public void Deserialize(NetDataReader reader) {
        playerId = reader.GetInt();
        username = reader.GetString();
    }
}

public class ServerPlayer {
    public NetPeer peer;
    public string username;
    public int playerId;
}



public class PlayerReceiveUpdatePacket {
    public GameState state { get; set; }
}

public class PlayerJoinedGamePacket {
    public ClientPlayer player { get; set; }
}

public class PlayerLeftGamePacket {
    public int playerId { get; set; }
}

public class PlayerAwaitPacket { }

