using Game.Network;

Console.WriteLine("Hello, World!");


Console.Write("Please provide player name: ");
// var playerName = Console.ReadLine()!;
var playerName = "consolist";//Console.ReadLine()!;

var networkClient = new Client();

networkClient.Connect(playerName);

while (true) {
	networkClient.Update();
}
