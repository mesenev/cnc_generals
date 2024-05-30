using Game.Commands;
using Game.GameObjects;
using LiteNetLib;
using LiteNetLib.Utils;

namespace Game
{
	public class JoinPacket {
		public string username { get; set; }
	}
	public class JoinAcceptPacket {
		public GameState state { get; set; }
		public ClientPlayer player { get; set; }
	}
	public struct ClientPlayer : INetSerializable {
		public uint playerId;
		public string username;

		public void Serialize(NetDataWriter writer) {
			writer.Put(playerId);
			writer.Put(username);
		}

		public void Deserialize(NetDataReader reader)
		{
			playerId = reader.GetUInt();
			username = reader.GetString();
		}
	}

	public class ServerPlayer {
		public NetPeer peer;
		public string username;
		public uint playerId;
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
		public uint playerId { get; set; }
	}

	public class PlayerAwaitPacket
	{
	}
}