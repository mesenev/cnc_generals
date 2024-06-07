using System.Net;
using System.Net.Sockets;
using System.Runtime.CompilerServices;
using Game;
using Game.Commands;
using Game.GameObjects;
using Game.GameObjects.Units;
using LiteNetLib;
using LiteNetLib.Utils;

namespace Server {
    public class Server : INetEventListener {
        public delegate void Handler();

        private readonly GameState gameState;
        private readonly NetManager netManager;
        private readonly NetPacketProcessor packetProcessor = new();
        private readonly Dictionary<uint, ServerPlayer> players = new();
        private readonly NetDataWriter writer = new();
        public required int PlayersAmount;
        public ServerState ServerState = ServerState.PlayerAwait;

        public Server(GameState gameState) {
            this.gameState = gameState;
            netManager = new NetManager(this) { AutoRecycle = true, UpdateTime = 1 };
        }

        public TimeSpan? TimeAlive { get; set; }

        public int ConnectedPeers => netManager.ConnectedPeersCount;

        public static event Handler ConnectedEvent = () => PeersAmountChanged.Invoke();
        public static event Handler DisconnectedEvent = () => PeersAmountChanged.Invoke();
        public static event Action PeersAmountChanged = () => { };

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

        private void SendPacket<T>(T packet, NetPeer peer, DeliveryMethod deliveryMethod) where T : class, new() {
            writer.Reset();
            packetProcessor.Write(writer, packet);
            peer.Send(writer, deliveryMethod);
        }

        private void OnJoinReceived(JoinPacket packet, NetPeer peer) {
            Program.Logs.Add($"Received join from {packet.username} (pid: {(uint)peer.Id})");

            int peerId = peer.Id;
            ServerPlayer newPlayer =
                players[(uint)peer.Id] = new ServerPlayer { peer = peer, username = packet.username };
            // var _packet = new SimplePacket { testVariable = new TestClass{firstVal = 228} };
            // SendPacket(_packet, peer, DeliveryMethod.Unreliable);

            SendPacket(
                new JoinAcceptPacket {
                    state = gameState, player = new ClientPlayer { playerId = peerId, username = packet.username }
                }, peer, DeliveryMethod.ReliableOrdered);

            foreach (ServerPlayer player in players.Values.Where(player => player.playerId != newPlayer.playerId)) {
                SendPacket(
                    new PlayerJoinedGamePacket {
                        player = new ClientPlayer { playerId = newPlayer.playerId, username = newPlayer.username }
                    },
                    player.peer, DeliveryMethod.ReliableOrdered);
                SendPacket(
                    new PlayerReceiveUpdatePacket { state = gameState },
                    player.peer, DeliveryMethod.ReliableOrdered);

                // SendPacket(
                // 	new PlayerJoinedGamePacket { player = new ClientPlayer { username = player.username }, },
                // 	newPlayer.peer, DeliveryMethod.ReliableOrdered);
            }
        }

        private void OnPlayerMove(MoveCommandPacket packet, NetPeer peer) {
            BaseUnit? currentUnit = gameState.GetUnitById(packet.command.unitId);
            HexCell? getCurrentCell = gameState.Grid.GetCell(currentUnit.x, currentUnit.y);
            HexCell? getNewCell = gameState.Grid.GetCell(packet.command.x, packet.command.y);
            currentUnit.x = packet.command.x;
            currentUnit.y = packet.command.y;
            getCurrentCell.RemoveCellUnit();
            getNewCell.UpdateCellUnit(currentUnit.UnitId);
        }

        public void OnPeerDisconnected(NetPeer peer, DisconnectInfo disconnectInfo) {
            DisconnectedEvent.Invoke();
            Program.Logs.Add($"Player (pid: {peer.Id}) left the game");
            if (peer.Tag == null)
                return;


            ServerPlayer playerLeft;
            if (!players.TryGetValue((uint)peer.Id, out playerLeft!))
                return;


            foreach (ServerPlayer player in players.Values.Where(player => player.playerId != playerLeft.playerId)) {
                SendPacket(new PlayerLeftGamePacket { playerId = playerLeft.playerId }, player.peer,
                    DeliveryMethod.ReliableOrdered);
            }

            players.Remove((uint)peer.Id);
        }

        public void OnConnectionRequest(ConnectionRequest request) {
            Program.Logs.Add($"Incoming connection from {request.RemoteEndPoint}");
            request.Accept();
            ConnectedEvent.Invoke();
        }


        public void Update() {
            DateTime time = DateTime.Now;
            _update();
            TimeAlive += DateTime.Now - time;
        }

        private void _update() {
            netManager.PollEvents();
            if (players.Count == PlayersAmount)
                ServerState = ServerState.Running;

            if (ServerState == ServerState.PlayerAwait) {
                foreach (ServerPlayer player in players.Values)
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
                    DeliveryMethod.ReliableOrdered
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
}
