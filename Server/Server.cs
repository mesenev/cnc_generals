using System.Net;
using System.Net.Sockets;
using System.Numerics;
using LiteNetLib;
using LiteNetLib.Utils;


public class Server : INetEventListener
{
    private NetManager _server;

    public Vector2 initialPosition = new();
    private NetDataWriter writer;
    private NetPacketProcessor packetProcessor;
    private Dictionary<uint, ServerPlayer> players = new();


    public void Start()
    {
        writer = new NetDataWriter();
        packetProcessor = new NetPacketProcessor();
        packetProcessor.RegisterNestedType((w, v) => w.Put(v), reader => reader.GetVector2());
        packetProcessor.RegisterNestedType<PlayerState>();
        packetProcessor.SubscribeReusable<JoinPacket, NetPeer>(OnJoinReceived);

        packetProcessor.RegisterNestedType<ClientPlayer>();
        packetProcessor.SubscribeReusable<PlayerSendUpdatePacket, NetPeer>(OnPlayerUpdate);

        _server = new NetManager(this) { AutoRecycle = true, UpdateTime = 1};

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
        if (peer != null)
        {
            writer.Reset();
            packetProcessor.Write(writer, packet);
            peer.Send(writer, deliveryMethod);
        }
    }

    public void OnJoinReceived(JoinPacket packet, NetPeer peer)
    {
        Console.WriteLine($"Received join from {packet.username} (pid: {(uint)peer.Id})");

        ServerPlayer newPlayer = (players[(uint)peer.Id] = new ServerPlayer
        {
            peer = peer,
            state = new PlayerState { pid = (uint)peer.Id, position = initialPosition, },
            username = packet.username,
        });

        SendPacket(new JoinAcceptPacket { state = newPlayer.state }, peer, DeliveryMethod.ReliableOrdered);

        foreach (ServerPlayer player in players.Values)
        {
            if (player.state.pid != newPlayer.state.pid)
            {
                SendPacket(
                    new PlayerJoinedGamePacket
                    {
                        player = new ClientPlayer { username = newPlayer.username, state = newPlayer.state, },
                    }, player.peer, DeliveryMethod.ReliableOrdered);

                SendPacket(
                    new PlayerJoinedGamePacket
                    {
                        player = new ClientPlayer { username = player.username, state = player.state, },
                    }, newPlayer.peer, DeliveryMethod.ReliableOrdered);
            }
        }
    }

    public void OnPlayerUpdate(PlayerSendUpdatePacket packet, NetPeer peer)
    {
        players[(uint)peer.Id].state.position = packet.position;
    }

    public void OnPeerDisconnected(NetPeer peer, DisconnectInfo disconnectInfo)
    {
        Console.WriteLine($"Player (pid: {(uint)peer.Id}) left the game");
        if (peer.Tag != null)
        {
            ServerPlayer playerLeft;
            if (players.TryGetValue(((uint)peer.Id), out playerLeft))
            {
                foreach (ServerPlayer player in players.Values)
                {
                    if (player.state.pid != playerLeft.state.pid)
                    {
                        SendPacket(new PlayerLeftGamePacket { pid = playerLeft.state.pid }, player.peer,
                            DeliveryMethod.ReliableOrdered);
                    }
                }

                players.Remove((uint)peer.Id);
            }
        }
    }

    public void OnConnectionRequest(ConnectionRequest request)
    {
        Console.WriteLine($"Incoming connection from {request.RemoteEndPoint.ToString()}");
        request.Accept();
    }

    public void Update()
    {
        _server.PollEvents();

        PlayerState[] states = players.Values.Select(p => p.state).ToArray();
        foreach (ServerPlayer player in players.Values)
        {
            SendPacket(new PlayerReceiveUpdatePacket { states = states }, player.peer, DeliveryMethod.Unreliable);
        }
        
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