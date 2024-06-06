using Server.InterfaceViews;
using CommandLine;
using Game.GameObjects;
using Terminal.Gui;

namespace Server;

internal static class Program {
    public class Options {
        [Option(
            'a', "players",
            Default = 1, HelpText = "Set amount of players for server to await"
        )]
        public required int PlayersAmountParam { get; set; }

        [Option(
            'p', "preset",
            Default = "default.txt", HelpText = "Path to the preset of initial game state"
        )]
        public required string PresetPathParam { get; set; }
    }

    public static int PlayersAmount;
    public static string PresetPath = "";
    public static Server Server;
    public static GameState GameState;

    private static int Main(string[] args) {
        Console.OutputEncoding = System.Text.Encoding.UTF8;

        Parser.Default.ParseArguments<Options>(args).WithParsed(o => {
            PlayersAmount = o.PlayersAmountParam;
            PresetPath = o.PresetPathParam;
        });

        Application.Init();
        Application.Top.Add(new MainView(new Rect(1, 1, 75, 20)) {
            Border = new Border { BorderStyle = BorderStyle.Single }
        });
        Console.WriteLine("Initializing server ...");
        GameState = new GameState(new Preset(PresetPath));
        Server = new Server(GameState) { PlayersAmount = PlayersAmount};

        Thread gameLoopThread = new Thread(GameLoop);
        Thread networkThread = new Thread(NetworkLoop);
        Thread broadcastThread = new Thread(BroadcastLoop);
        Thread consoleThread = new Thread(ConsoleLoop);
        
        gameLoopThread.Start();
        networkThread.Start();
        broadcastThread.Start();
        consoleThread.Start();
        
        gameLoopThread.Join();
        networkThread.Join();
        broadcastThread.Join();
        consoleThread.Join();

        return 0;
    }

    private static void NetworkLoop() {
        while (true) {
            Server.Update();
            Thread.Sleep(10);
        }
    }

    private static void ConsoleLoop() {
        Application.Run();
        Application.Shutdown();
    }

    private static void GameLoop() {
        Server.Start();
        while (true) {
            UpdateGameState();
            Thread.Sleep(16); // 60fps
        }
    }

    private static void UpdateGameState() {
        GameState.Update(new TimeSpan(123));
    }

    private static void ReceivePlayerPackets() {

    }

    private static void BroadcastLoop() {
        while (true) {
            // Сериализация и рассылка игрового состояния
            // BroadcastGameState();
            Thread.Sleep(50); // Частота рассылки состояния
        }
    }
}
