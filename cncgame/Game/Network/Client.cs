using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Sockets;
using Game;
using Lime;
using LiteNetLib;
using LiteNetLib.Utils;

public class Client : INetEventListener
{
	public static readonly Client Instance = new ();
	
    private NetManager _client;
    private NetPeer _server;
    
    private NetDataWriter writer;
    private NetPacketProcessor packetProcessor;
    private ClientPlayer clientPlayer = new();
    private Dictionary<uint, ServerPlayer> serverPlayers = new ();

    public List<ServerPlayer> GetServerPlayers()
    {
	    return serverPlayers.Values.ToList();
    }

    public void UpdateClientPlayer(Vector2 newPos, uint pid)
    {
	    clientPlayer.state.position = newPos;
	    clientPlayer.state.pid = pid;
    }

    public void Connect(string username) {
        clientPlayer.username = username;
        writer = new NetDataWriter();
        packetProcessor = new NetPacketProcessor();
        packetProcessor.RegisterNestedType((w, v) => w.Put(v), reader => reader.GetVector2());
        packetProcessor.RegisterNestedType<PlayerState>();
        packetProcessor.SubscribeReusable<JoinAcceptPacket>(OnJoinAccept);
        
        packetProcessor.RegisterNestedType<ClientPlayer>();
        packetProcessor.SubscribeReusable<PlayerReceiveUpdatePacket>(OnReceiveUpdate);
        packetProcessor.SubscribeReusable<PlayerJoinedGamePacket>(OnPlayerJoin);
        packetProcessor.SubscribeReusable<PlayerLeftGamePacket>(OnPlayerLeave);

        
        _client = new NetManager(this) {
            AutoRecycle = true,
        };
        
        _client.Start();
        Console.WriteLine("Connecting to server");
        _client.Connect("localhost", 12345, "");
    }

    public void Disconnect()
    {
	    _client.Stop();
    }

    public void OnPeerConnected(NetPeer peer) {
        Console.WriteLine("Connected to server");
        _server = peer;
        
        SendPacket(new JoinPacket { username = clientPlayer.username }, DeliveryMethod.ReliableOrdered);
    }

    public void SendPacket<T>(T packet, DeliveryMethod deliveryMethod) where T : class, new() {
        if (_server != null) {
            writer.Reset();
            packetProcessor.Write(writer, packet);
            _server.Send(writer, deliveryMethod);
        }
    }
    
    public void OnJoinAccept(JoinAcceptPacket packet) {
        Console.WriteLine($"Join accepted by server (pid: {packet.state.pid})");
        clientPlayer.state = packet.state;
    }
    
    public void OnReceiveUpdate(PlayerReceiveUpdatePacket packet) {
        foreach (PlayerState state in packet.states) {
            if (state.pid == clientPlayer.state.pid) {
                continue;
            }

            if (serverPlayers.ContainsKey(state.pid)) {
	            serverPlayers[state.pid].state.position = state.position;
            } else {
	            var newServerPlayer = new ServerPlayer{state = state};
	            serverPlayers.Add(state.pid, newServerPlayer);
            }
        }
    }
    
    public void OnPlayerJoin(PlayerJoinedGamePacket packet) {
        Console.WriteLine($"Player '{packet.player.username}' (pid: {packet.player.state.pid}) joined the game");
        ServerPlayer newServerPlayer = new ServerPlayer{state = packet.player.state};
        serverPlayers.Add(newServerPlayer.state.pid, newServerPlayer);
    }
    
    public void OnPlayerLeave(PlayerLeftGamePacket packet) {
        Console.WriteLine($"Player (pid: {packet.pid}) left the game");
        serverPlayers.Remove(packet.pid);
    }
    
    public void Update() {
        if (_client != null) {
            _client.PollEvents();
            if (clientPlayer.username != null) {
                SendPacket(new PlayerSendUpdatePacket { position = clientPlayer.state.position }, DeliveryMethod.Unreliable);
            }
        }
    }

    public void OnConnectionRequest(ConnectionRequest request)
    {
        
    }

    public void OnNetworkError(IPEndPoint endPoint, SocketError socketError)
    {
        
    }

    public void OnNetworkReceive(NetPeer peer, NetPacketReader reader, byte channelNumber, DeliveryMethod deliveryMethod)
    {
        packetProcessor.ReadAllPackets(reader);
    }

    public void OnNetworkReceiveUnconnected(IPEndPoint remoteEndPoint, NetPacketReader reader, UnconnectedMessageType messageType)
    {
        
    }

    public void OnPeerDisconnected(NetPeer peer, DisconnectInfo disconnectInfo)
    {
        
    }

    public void OnNetworkLatencyUpdate(NetPeer peer, int latency)
    {
        
    }
}