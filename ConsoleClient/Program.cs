using Game;
using Game.Network;
using LiteNetLib;

Console.WriteLine("Hello, World!");


Console.Write("Please provide player name: ");
// var playerName = Console.ReadLine()!;
const string playerName = "consolist"; //Console.ReadLine()!;



var networkClient = new NetworkClient();
networkClient.Connect(playerName);
networkClient.SendPacket(new JoinPacket { username = playerName }, DeliveryMethod.Unreliable);
while (true) {
    networkClient.Update();
    Thread.Sleep(10);
}
