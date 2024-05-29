using System.Net;
using System.Net.Sockets;
using System.Numerics;
using LiteNetLib;
using LiteNetLib.Utils;
using SharedClasses.GameObjects;


public class Server : INetEventListener
{
	private NetManager _server;

	public Vector2 initialPosition = new();
	private NetDataWriter writer;
	private NetPacketProcessor packetProcessor;
	private Dictionary<uint, ServerPlayer> players = new();
	private GameState gameState;
	private DateTime? t0;
	private DateTime? t1;
	private ServerState serverState;

	public Server()
	{
		gameState = new GameState();
		serverState = ServerState.playerAwait;
		Start();
	}

	public void Start()
	{
		writer = new NetDataWriter();
		packetProcessor = new NetPacketProcessor();
		packetProcessor.RegisterNestedType((w, v) => w.Put(v), reader => reader.GetVector2());
		packetProcessor.RegisterNestedType<GameState>((w, v) => w.Put(v), reader => reader.GetGameState());
		packetProcessor.SubscribeReusable<JoinPacket, NetPeer>(OnJoinReceived);

		packetProcessor.RegisterNestedType<ClientPlayer>();
		packetProcessor.SubscribeReusable<SendCommandPacket, NetPeer>(OnPlayerUpdate);

		_server = new NetManager(this) { AutoRecycle = true, UpdateTime = 1 };

		Console.WriteLine("Starting server");
		_server.Start(12345);
	}

	public void Stop()
	{
		_server.Stop();
	}

	public uint GetNewPID()
	{
		return players.Count != 0 ? players.Keys.Max() - 1 : 0;
	}

	public void SendPacket<T>(T packet, NetPeer peer, DeliveryMethod deliveryMethod) where T : class, new()
	{
		if (peer != null) {
			writer.Reset();
			packetProcessor.Write(writer, packet);
			peer.Send(writer, deliveryMethod);
		}
	}

	public void OnJoinReceived(JoinPacket packet, NetPeer peer)
	{
		Console.WriteLine($"Received join from {packet.username} (pid: {(uint)peer.Id})");

		ServerPlayer newPlayer =
			(players[(uint)peer.Id] = new ServerPlayer { peer = peer, username = packet.username, });

		SendPacket(new JoinAcceptPacket { state = gameState }, peer, DeliveryMethod.ReliableOrdered);

		foreach (ServerPlayer player in players.Values) {
			if (player.playerId != newPlayer.playerId) {
				SendPacket(
					new PlayerJoinedGamePacket { player = new ClientPlayer { username = newPlayer.username }, },
					player.peer, DeliveryMethod.ReliableOrdered);

				SendPacket(
					new PlayerJoinedGamePacket { player = new ClientPlayer { username = player.username }, },
					newPlayer.peer, DeliveryMethod.ReliableOrdered);
			}
		}
	}

	public void OnPlayerUpdate(SendCommandPacket packet, NetPeer peer)
	{
	}

	public void OnPeerDisconnected(NetPeer peer, DisconnectInfo disconnectInfo)
	{
		Console.WriteLine($"Player (pid: {peer.Id}) left the game");
		if (peer.Tag == null)
			return;
		

		ServerPlayer playerLeft;
		if (!players.TryGetValue((uint)peer.Id, out playerLeft))
			return;
		

		foreach (ServerPlayer player in players.Values) {
			if (player.playerId != playerLeft.playerId) {
				SendPacket(new PlayerLeftGamePacket { pid = playerLeft.playerId }, player.peer,
					DeliveryMethod.ReliableOrdered);
			}
		}

		players.Remove((uint)peer.Id);
	}

	public void OnConnectionRequest(ConnectionRequest request)
	{
		Console.WriteLine($"Incoming connection from {request.RemoteEndPoint.ToString()}");
		request.Accept();
	}

	public void Update()
	{
		_server.PollEvents();
		if (this.serverState == ServerState.playerAwait) {
			foreach (var player in players.Values)
				SendPacket(new PlayerAwaitPacket(), player.peer, DeliveryMethod.Unreliable);

			return;
		}

		if (this.serverState == ServerState.shuttingDown) {
			return;
		}

		t0 ??= DateTime.Now;
		t1 = DateTime.Now;
		var timeSpan = (TimeSpan)(t1 - t0);
		gameState.Update(timeSpan);
		foreach (ServerPlayer player in players.Values) {
			SendPacket(new PlayerReceiveUpdatePacket { state = gameState }, player.peer, DeliveryMethod.Unreliable);
		}

		t0 = t1;

		// Thread.Sleep(1000);
	}

	public void OnNetworkError(IPEndPoint endPoint, SocketError socketError)
	{
	}

	public void OnNetworkReceive(NetPeer peer, NetPacketReader reader, byte channelNumber,
		DeliveryMethod deliveryMethod)
	{
		packetProcessor.ReadAllPackets(reader, peer);
	}

	public void OnPeerConnected(NetPeer peer)
	{
	}

	public void OnNetworkLatencyUpdate(NetPeer peer, int latency)
	{
	}

	public void OnNetworkReceiveUnconnected(IPEndPoint remoteEndPoint, NetPacketReader reader,
		UnconnectedMessageType messageType)
	{
	}
}
