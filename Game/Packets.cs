using Game.Commands;
using Game.GameObjects;
using LiteNetLib;
using LiteNetLib.Utils;

namespace Game {
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

    public class MoveCommandPacket {
        public MoveCommand2 command { get; set; }
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

    public class TestClass : INetSerializable {
        public int firstVal;

        public void Serialize(NetDataWriter writer) {
            writer.Put(firstVal);
        }

        public void Deserialize(NetDataReader reader) {
            firstVal = reader.GetInt();
        }
    }

    public class SimplePacket {
        public TestClass testVariable { get; set; }
    }

    public class PlayerAwaitPacket { }
}
