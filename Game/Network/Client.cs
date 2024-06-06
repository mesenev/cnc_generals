using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Sockets;
using Game.Commands;
using Game.GameObjects;
using Lime;
using LiteNetLib;
using LiteNetLib.Utils;
using Game.GameObjects;
using Game.GameObjects.Units;
using Environment = System.Environment;

namespace Game.Network {
    public class Client : INetEventListener {
        public static readonly Client Instance = new();

        public bool isPLayerJoined = false;

        private NetManager _client;
        private NetPeer _server;

        private NetDataWriter writer;
        private NetPacketProcessor packetProcessor;
        private ClientPlayer clientPlayer;
        private Dictionary<int, ServerPlayer> serverPlayers = new();
        public GameState gameState;
        public Queue<MoveCommand2> commands = new();

        public List<ServerPlayer> GetServerPlayers() {
            return serverPlayers.Values.ToList();
        }

        public ClientPlayer GetClientPlayer() {
            return clientPlayer;
        }

        public void Connect(string username) {
            clientPlayer.username = username;
            writer = new NetDataWriter();
            packetProcessor = new NetPacketProcessor();

            packetProcessor.RegisterNestedType(() => new GameState());
            packetProcessor.RegisterNestedType(
                (w, v) => w.Put(v), reader => reader.GetVector2()
            );
            packetProcessor.RegisterNestedType<ClientPlayer>();
            packetProcessor.RegisterNestedType<MoveCommand2>();
            packetProcessor.RegisterNestedType(() => new TestClass());


            packetProcessor.SubscribeReusable<JoinAcceptPacket>(OnJoinAccept);
            packetProcessor.SubscribeReusable<PlayerJoinedGamePacket>(OnPlayerJoin);
            packetProcessor.SubscribeReusable<PlayerReceiveUpdatePacket>(OnReceiveUpdate);
            packetProcessor.SubscribeReusable<PlayerLeftGamePacket>(OnPlayerLeave);
            packetProcessor.SubscribeReusable<PlayerAwaitPacket>(OnPlayerAwait);
            packetProcessor.SubscribeReusable<SimplePacket>(OnSimplePacket);

            _client = new NetManager(this) { AutoRecycle = true };

            _client.Start();
            Console.WriteLine("Connecting to server");
            _client.Connect("localhost", 12345, "");
        }

        public void Disconnect() {
            _client.Stop();
        }

        public void OnPeerConnected(NetPeer peer) {
            Console.WriteLine("Connected to server");
            _server = peer;

            SendPacket(new JoinPacket { username = clientPlayer.username }, DeliveryMethod.ReliableOrdered);
        }

        public void SendPacket<T>(T packet, DeliveryMethod deliveryMethod) where T : class, new() {
            Console.WriteLine($"Trying to send packet: {packet.GetType()}");
            if (_server == null)
                return;


            writer.Reset();
            packetProcessor.Write(writer, packet);
            _server.Send(writer, deliveryMethod);
        }

        public void OnJoinAccept(JoinAcceptPacket packet) {
            Console.WriteLine($"Join accepted by server (pid: {packet.state})");
            gameState = packet.state;
            isPLayerJoined = true;
        }

        public void OnReceiveUpdate(PlayerReceiveUpdatePacket packet) {
            gameState = packet.state;
        }

        public void OnPlayerJoin(PlayerJoinedGamePacket packet) {
            Console.WriteLine($"Player '{packet.player.username}' (pid: {packet.player.playerId}) joined the game");
            ServerPlayer newServerPlayer =
                new ServerPlayer { username = packet.player.username, playerId = packet.player.playerId };
            serverPlayers.Add(newServerPlayer.playerId, newServerPlayer);
        }

        public void OnPlayerLeave(PlayerLeftGamePacket packet) {
            Console.WriteLine($"Player (pid: {packet.playerId}) left the game");
            serverPlayers.Remove(packet.playerId);
        }

        public void OnSimplePacket(SimplePacket packet) {
            Console.WriteLine(packet.testVariable.firstVal);
        }

        public void Update() {
            if (_client == null)
                return;

            _client.PollEvents();
            if (clientPlayer.username != null && commands.Count != 0)
                SendPacket(
                    new MoveCommandPacket { command = commands.Dequeue() },
                    DeliveryMethod.Unreliable
                );
        }

        public void OnPlayerAwait(PlayerAwaitPacket packet) { }

        public void OnConnectionRequest(ConnectionRequest request) { }

        public void OnNetworkError(IPEndPoint endPoint, SocketError socketError) { }

        public void OnNetworkReceive(NetPeer peer, NetPacketReader reader, byte channelNumber,
            DeliveryMethod deliveryMethod) {
            packetProcessor.ReadAllPackets(reader);
        }

        public void OnNetworkReceiveUnconnected(IPEndPoint remoteEndPoint, NetPacketReader reader,
            UnconnectedMessageType messageType) { }

        public void OnPeerDisconnected(NetPeer peer, DisconnectInfo disconnectInfo) { }

        public void OnNetworkLatencyUpdate(NetPeer peer, int latency) { }
    }
}
