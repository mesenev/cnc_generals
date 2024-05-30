using System.Net;
using System.Net.Sockets;
using System.Numerics;
using Game;
using Game.Commands;
using Game.GameObjects;
using LiteNetLib;
using LiteNetLib.Utils;
using Game.GameObjects;
using Game.GameObjects.Units;

namespace Server;

public class Server : INetEventListener
{
	private NetManager server;

	public Vector2 initialPosition = new();
	private NetDataWriter writer;
	private NetPacketProcessor packetProcessor;
	private Dictionary<uint, ServerPlayer> players = new();
	private GameState gameState;
	private DateTime? t0;
	private DateTime? t1;
	private ServerState serverState;
	private uint unitCounter;

	public Server()
	{
		unitCounter = 0;
		gameState = new GameState();
		serverState = ServerState.PlayerAwait;
		Start();
	}

	public void Start()
	{
		writer = new NetDataWriter();
		packetProcessor = new NetPacketProcessor();
		packetProcessor.RegisterNestedType((w, v) => w.Put(v), reader => reader.GetVector2());
		packetProcessor.RegisterNestedType<ClientPlayer>();
		packetProcessor.RegisterNestedType<MoveCommand2>();
		
		packetProcessor.SubscribeReusable<JoinPacket, NetPeer>(OnJoinReceived);
		packetProcessor.SubscribeReusable<MoveCommandPacket, NetPeer>(OnPlayerMove);

		server = new NetManager(this) { AutoRecycle = true, UpdateTime = 1 };

		Console.WriteLine("Starting server");
		server.Start(12345);
	}

	public void Stop()
	{
		server.Stop();
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

		uint peerId = (uint)peer.Id;
		ServerPlayer newPlayer =
			(players[peerId] = new ServerPlayer { peer = peer, username = packet.username, playerId = peerId });
		var randomCell = gameState.Grid.GetRandomCell();
		gameState.AddUnit(new MarineUnit(unitId: unitCounter++, ownerId: peerId, randomCell.x, randomCell.y));

		SendPacket(
			new JoinAcceptPacket {
				state = gameState, player = new ClientPlayer { playerId = peerId, username = packet.username }
			}, peer, DeliveryMethod.ReliableOrdered);

		foreach (var player in players.Values.Where(player => player.playerId != newPlayer.playerId)) {
			SendPacket(
				new PlayerJoinedGamePacket {
					player = new ClientPlayer { playerId = newPlayer.playerId, username = newPlayer.username },
				},
				player.peer, DeliveryMethod.ReliableOrdered);
			SendPacket(
				new PlayerReceiveUpdatePacket() { state = gameState },
				player.peer, DeliveryMethod.ReliableOrdered);

			// SendPacket(
			// 	new PlayerJoinedGamePacket { player = new ClientPlayer { username = player.username }, },
			// 	newPlayer.peer, DeliveryMethod.ReliableOrdered);
		}
	}

	public void OnPlayerMove(MoveCommandPacket packet, NetPeer peer)
	{
		var currentUnit = gameState.GetUnitById(packet.command.unitId);
		var getCurrentCell = gameState.Grid.GetCell(currentUnit.x, currentUnit.y);
		var getNewCell = gameState.Grid.GetCell(packet.command.x, packet.command.y);
		getCurrentCell.RemoveCellUnit();
		getNewCell.UpdateCellUnit(currentUnit);
	}

	public void OnPeerDisconnected(NetPeer peer, DisconnectInfo disconnectInfo)
	{
		Console.WriteLine($"Player (pid: {peer.Id}) left the game");
		if (peer.Tag == null)
			return;


		ServerPlayer playerLeft;
		if (!players.TryGetValue((uint)peer.Id, out playerLeft))
			return;


		foreach (var player in players.Values.Where(player => player.playerId != playerLeft.playerId)) {
			SendPacket(new PlayerLeftGamePacket { playerId = playerLeft.playerId }, player.peer,
				DeliveryMethod.ReliableOrdered);
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
		server.PollEvents();
		if (players.Count == 2) {
			serverState = ServerState.Running;
		}

		if (this.serverState == ServerState.PlayerAwait) {
			foreach (var player in players.Values)
				SendPacket(new PlayerAwaitPacket(), player.peer, DeliveryMethod.Unreliable);

			return;
		}

		if (this.serverState == ServerState.ShuttingDown) {
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
