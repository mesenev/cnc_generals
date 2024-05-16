using System.Numerics;
using LiteNetLib;
using LiteNetLib.Utils;
// using Lime;

public class JoinPacket {
	public string username { get; set; }
}

public class JoinAcceptPacket {
	public PlayerState state { get; set; }
}

public struct PlayerState : INetSerializable {
	public uint pid;
	public Vector2 position;

	public void Serialize(NetDataWriter writer) {
		writer.Put(pid);
		writer.Put(position);
	}

	public void Deserialize(NetDataReader reader) {
		pid = reader.GetUInt();
		position = reader.GetVector2();
	}
}

public struct ClientPlayer : INetSerializable {
	public PlayerState state;
	public string username;

	public void Serialize(NetDataWriter writer) {
		state.Serialize(writer);
		writer.Put(username);
	}

	public void Deserialize(NetDataReader reader) {
		state.Deserialize(reader);
		username = reader.GetString();
	}
}

public class ServerPlayer {
	public NetPeer peer;
	public PlayerState state;
	public string username;
}

public class PlayerSendUpdatePacket {
	public Vector2 position { get; set; }
}

public class PlayerReceiveUpdatePacket {
	public PlayerState[] states { get; set; }
}

public class PlayerJoinedGamePacket {
	public ClientPlayer player { get; set; }
}

public class PlayerLeftGamePacket {
	public uint pid { get; set; }
}
