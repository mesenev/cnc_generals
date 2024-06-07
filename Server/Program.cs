using System.Collections;
using System.Text;
using Autofac;
using CommandLine;
using Game.GameObjects;
using Server.InterfaceViews;
using Terminal.Gui;
using Attribute = Terminal.Gui.Attribute;

namespace Server;

internal static class Program {
    public static readonly List<string> Logs = [];

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

    public static ColorScheme ColorScheme = new() {
        Normal = Attribute.Make(
            Color.BrightGreen, Color.Black
        ),
        Focus = Attribute.Make(Color.Brown, Color.Black),
    };

    private static int Main(string[] args) {
        Console.OutputEncoding = Encoding.UTF8;

        Parser.Default.ParseArguments<Options>(args).WithParsed(o => {
            PlayersAmount = o.PlayersAmountParam;
            PresetPath = o.PresetPathParam;
        });


        Application.UseSystemConsole = true;

        Application.Init();

        GameState = new GameState(new Preset(PresetPath));
        Server = new Server(GameState) { PlayersAmount = PlayersAmount };

        var builder = new ContainerBuilder();

        builder.RegisterInstance(GameState).AsSelf();
        builder.RegisterInstance(Server).AsSelf();

        var container = builder.Build();

        var mainView = new MainView(new Rect(1, 1, 75, 20)) {
            Border = new Border { BorderStyle = BorderStyle.Single }, ColorScheme = ColorScheme,
        };
        Application.Top.Add(mainView);


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
        Server.Start();
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
        while (true) {
            UpdateGameState();
            Thread.Sleep(16); // 60fps
        }
    }

    private static void UpdateGameState() {
        GameState.Update(new TimeSpan(16));
    }

    private static void BroadcastLoop() {
        while (true) {
            // Сериализация и рассылка игрового состояния
            // BroadcastGameState();
            Thread.Sleep(50); // Частота рассылки состояния
        }
    }
}
