using Game;
using Game.Network;
using LiteNetLib;

Console.WriteLine("Hello, World!");


Console.Write("Please provide player name: ");
// var playerName = Console.ReadLine()!;
var playerName = "consolist"; //Console.ReadLine()!;


var networkClient = new Client();
networkClient.Connect(playerName);
// networkClient.Update();
networkClient.SendPacket(new JoinPacket { username = playerName }, DeliveryMethod.Unreliable);
while (true) {
    networkClient.Update();
    // Thread.Sleep(15000);
}
