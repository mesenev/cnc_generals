using System.Net;
using System.Net.Sockets;
using Game;
using Game.Commands;
using Game.GameObjects;
using LiteNetLib;
using LiteNetLib.Utils;


namespace Server;

public class Server : INetEventListener {
    private readonly NetManager netManager;
    private readonly NetDataWriter writer = new();
    private readonly NetPacketProcessor packetProcessor = new();
    private readonly Dictionary<uint, ServerPlayer> players = new();
    private readonly GameState gameState;
    public ServerState ServerState = ServerState.PlayerAwait;
    public required int PlayersAmount;
    public TimeSpan? TimeAlive { get; set; } = null;

    public Server(GameState gameState) {
        this.gameState = gameState;
        netManager = new NetManager(this) { AutoRecycle = true, UpdateTime = 1 };
    }

    public int ConnectedPeers {
        get => netManager.ConnectedPeersCount;
    }

    public void Start() {
        packetProcessor.RegisterNestedType(
            (w, v) => w.Put(v), reader => reader.GetVector2()
        );
        packetProcessor.RegisterNestedType<ClientPlayer>();
        packetProcessor.RegisterNestedType<MoveCommand2>();
        packetProcessor.RegisterNestedType(() => new GameState());

        packetProcessor.SubscribeReusable<JoinPacket, NetPeer>(OnJoinReceived);
        packetProcessor.SubscribeReusable<MoveCommandPacket, NetPeer>(OnPlayerMove);

        TimeAlive = new TimeSpan(0);
        var port = 12345;
        netManager.Start(port);
        Program.Logs.Add($"Server is up! port: {port}");
    }

    public void Stop() {
        netManager.Stop();
    }

    public void SendPacket<T>(T packet, NetPeer peer, DeliveryMethod deliveryMethod) where T : class, new() {
        writer.Reset();
        packetProcessor.Write(writer, packet);
        peer.Send(writer, DeliveryMethod.ReliableOrdered);
    }

    public void OnJoinReceived(JoinPacket packet, NetPeer peer) {
        Program.Logs.Add($"Received join from {packet.username} (pid: {(uint)peer.Id})");

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
        currentUnit.x = packet.command.x;
        currentUnit.y = packet.command.y;
        getCurrentCell.RemoveCellUnit();
        getNewCell.UpdateCellUnit(currentUnit.UnitId);
    }

    public void OnPeerDisconnected(NetPeer peer, DisconnectInfo disconnectInfo) {
        Program.Logs.Add($"Player (pid: {peer.Id}) left the game");
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
        Program.Logs.Add($"Incoming connection from {request.RemoteEndPoint}");
        request.Accept();
    }


    public void Update() {
        var time = DateTime.Now;
        _update();
        TimeAlive += DateTime.Now - time;
    }

    private void _update() {
        netManager.PollEvents();
        if (players.Count == PlayersAmount)
            ServerState = ServerState.Running;

        if (ServerState == ServerState.PlayerAwait) {
            foreach (var player in players.Values)
                SendPacket(new PlayerAwaitPacket(), player.peer, DeliveryMethod.Unreliable);
            return;
        }

        if (ServerState == ServerState.ShuttingDown)
            return;

        if (ServerState == ServerState.Running)
            BroadcastStateUpdate();
    }

    private void BroadcastStateUpdate() {
        foreach (ServerPlayer player in players.Values) {
            SendPacket(
                new PlayerReceiveUpdatePacket { state = gameState },
                player.peer,
                DeliveryMethod.Unreliable
            );
        }
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

public enum ServerState {
    PlayerAwait,
    Running,
    ShuttingDown
}
