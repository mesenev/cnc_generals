using System.Net;
using System.Net.Sockets;
using System.Numerics;
using Game;
using Game.Commands;
using Game.GameObjects;
using LiteNetLib;
using LiteNetLib.Utils;

namespace Server;

public class Server : INetEventListener {
    private NetManager server;

    public Vector2 initialPosition = new();
    private NetDataWriter writer;
    private NetPacketProcessor packetProcessor;
    private Dictionary<uint, ServerPlayer> players = new();
    private GameState gameState;
    private DateTime? t0;
    private DateTime? t1;
    private ServerState serverState;
    private int unitCounter;
    private string? presetPath;
    private int playersAmount;

    public Server(int playersAmount = 2, string? presetPath = "") {
        this.playersAmount = playersAmount;
        this.presetPath = presetPath;
        unitCounter = 0;
        gameState = new GameState(new Preset(this.presetPath));
        serverState = ServerState.PlayerAwait;
        gameState.PrintGameState();
        Start();
    }

    public void Start() {
        writer = new NetDataWriter();
        packetProcessor = new NetPacketProcessor();
        packetProcessor.RegisterNestedType(
            (w, v) => w.Put(v), reader => reader.GetVector2()
        );
        packetProcessor.RegisterNestedType<ClientPlayer>();
        packetProcessor.RegisterNestedType<MoveCommand2>();
        packetProcessor.RegisterNestedType(() => new GameState());


        packetProcessor.SubscribeReusable<JoinPacket, NetPeer>(OnJoinReceived);
        packetProcessor.SubscribeReusable<MoveCommandPacket, NetPeer>(OnPlayerMove);

        server = new NetManager(this) { AutoRecycle = true, UpdateTime = 1 };

        Console.WriteLine("Starting server");
        server.Start(12345);
    }

    public void Stop() {
        server.Stop();
    }


    public void SendPacket<T>(T packet, NetPeer peer, DeliveryMethod deliveryMethod) where T : class, new() {
        writer.Reset();
        packetProcessor.Write(writer, packet);
        peer.Send(writer, DeliveryMethod.ReliableOrdered);
    }

    public void OnJoinReceived(JoinPacket packet, NetPeer peer) {
        Console.WriteLine($"Received join from {packet.username} (pid: {(uint)peer.Id})");

        int peerId = peer.Id;
        ServerPlayer newPlayer =
            (players[(uint)peer.Id] = new ServerPlayer { peer = peer, username = packet.username, });
        // var _packet = new SimplePacket { testVariable = new TestClass{firstVal = 228} };
        // SendPacket(_packet, peer, DeliveryMethod.Unreliable);

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

    public void OnPlayerMove(MoveCommandPacket packet, NetPeer peer) {
        var currentUnit = gameState.GetUnitById(packet.command.unitId);
        var getCurrentCell = gameState.Grid.GetCell(currentUnit.x, currentUnit.y);
        var getNewCell = gameState.Grid.GetCell(packet.command.x, packet.command.y);
        getCurrentCell.RemoveCellUnit();
        getNewCell.UpdateCellUnit(currentUnit.UnitId);
    }

    public void OnPeerDisconnected(NetPeer peer, DisconnectInfo disconnectInfo) {
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

    public void OnConnectionRequest(ConnectionRequest request) {
        Console.WriteLine($"Incoming connection from {request.RemoteEndPoint}");
        request.Accept();
    }

    public void Update() {
        server.PollEvents();
        if (players.Count == playersAmount) {
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
        // gameState.Update(timeSpan);
        // var packet = new SimplePacket { testVariable = new TestClass{firstVal = 228} };
        var packet = new PlayerReceiveUpdatePacket { state = gameState };
        foreach (ServerPlayer player in players.Values) {
            SendPacket(packet, player.peer, DeliveryMethod.Unreliable);
            // SendPacket(new PlayerReceiveUpdatePacket { state = gameState }, player.peer, DeliveryMethod.Unreliable);
        }

        t0 = t1;

        // Thread.Sleep(1000);
    }

    public void OnNetworkError(IPEndPoint endPoint, SocketError socketError) { }

    public void OnNetworkReceive(NetPeer peer, NetPacketReader reader, byte channelNumber,
        DeliveryMethod deliveryMethod) {
        packetProcessor.ReadAllPackets(reader, peer);
    }

    public void OnPeerConnected(NetPeer peer) { }

    public void OnNetworkLatencyUpdate(NetPeer peer, int latency) { }

    public void OnNetworkReceiveUnconnected(IPEndPoint remoteEndPoint, NetPacketReader reader,
        UnconnectedMessageType messageType) { }
}
