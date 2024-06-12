using System.Diagnostics;
using System.Text;
using Autofac;
using CommandLine;
using Game.GameObjects;
using Server.InterfaceViews;
using Terminal.Gui;
using Attribute = Terminal.Gui.Attribute;

namespace Server {
    internal static class Program {
        public static readonly List<string> Logs = [];

        public static int PlayersAmount;
        public static string PresetPath = "";
        public static bool DisableSoundNotifications = true;
        public static Server Server = null!;
        public static GameState GameState = null!;
        public static SoundNotificationsService SoundManager = new();

        public static readonly ColorScheme ColorScheme = new() {
            Normal = Attribute.Make(
                Color.BrightGreen, Color.Black
            ),
            Focus = Attribute.Make(Color.Brown, Color.Black)
        };

        private static int Main(string[] args) {
            Console.OutputEncoding = Encoding.UTF8;
            Trace.Listeners.Add(new DebugListener());

            Parser.Default.ParseArguments<Options>(args).WithParsed(
                o => {
                    PlayersAmount = o.PlayersAmountParam;
                    PresetPath = o.PresetPathParam;
                    DisableSoundNotifications = o.NoSoundNotificationParam;
                }
            );


            Application.UseSystemConsole = true;

            Application.Init();

            GameState = new GameState(new Preset(PresetPath));
            Server = new Server(GameState) { PlayersAmount = PlayersAmount };
            Server.PeersAmountChanged += PeersAmountChangedHandler;
            if (!DisableSoundNotifications)
                SoundEventsSetup();

            var builder = new ContainerBuilder();

            builder.RegisterInstance(GameState).AsSelf();
            builder.RegisterInstance(Server).AsSelf();

            var container = builder.Build();


            var mainView = new MainView(new Rect(1, 1, 90, 28)) {
                Border = new Border { BorderStyle = BorderStyle.Single }, ColorScheme = ColorScheme
            };
            Application.Top.Add(mainView);


            var gameLoopThread = new Thread(GameLoop);
            var networkThread = new Thread(NetworkLoop);
            var broadcastThread = new Thread(BroadcastLoop);
            var consoleThread = new Thread(ConsoleLoop);

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

        private static void SoundEventsSetup() {
            Server.ConnectedEvent += SoundManager.PlayConnectedSound;
            Server.DisconnectedEvent += SoundManager.PlayDisconnectedSound;
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

        private static void PeersAmountChangedHandler() {
            if (Server.ConnectedPeers == 0)
                GameState.IsPaused = true;
            if (Server.ConnectedPeers > 1)
                GameState.IsPaused = false;
            if (Server.ConnectedPeers == PlayersAmount)
                GameState.InitializeWorld();
        }

        private static void GameLoop() {
            var t0 = DateTime.Now;
            var t1 = DateTime.Now;
            while (true) {
                t1 = DateTime.Now;
                GameState.Update(t1 - t0);
                t0 = t1;
                Thread.Sleep(16); // 60fps
            }
        }

        private static void BroadcastLoop() {
            while (true) {
                // Сериализация и рассылка игрового состояния
                // BroadcastGameState();
                Thread.Sleep(50); // Частота рассылки состояния
            }
        }

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

            [Option(
                's', "silent",
                Default = false, HelpText = "Disable sound notifications"
            )]
            public required bool NoSoundNotificationParam { get; set; }
        }

        private class DebugListener : TraceListener {
            public override void Write(string? message) {
                if (message != null) Logs.Add(message);
            }

            public override void WriteLine(string? message) {
                if (message != null) Logs.Add(message + "\n");
            }
        }
    }
}
