using CommandLine;

namespace Server;

public class Program {
    private class Options {
        [Option(
            'a', "players",
            Default = 1, HelpText = "Set amount of players for server to await"
        )]
        public int PlayersAmount { get; set; }

        [Option(
            'p', "preset",
            Default = "default.txt", HelpText = "Path to the preset of initial game state"
        )]
        public string? PresetPath { get; set; }
    }

    public static void Main(string[] args) {
        double tickrate = 30.0;
        double interval = 1000.0 / tickrate;

        int playersAmount = default;
        string? presetPath = default!;
        Parser.Default.ParseArguments<Options>(args).WithParsed<Options>(o => {
            playersAmount = o.PlayersAmount;
            presetPath = o.PresetPath;
        });

        Console.WriteLine(playersAmount);
        Console.WriteLine(presetPath);
        var server = new Server(playersAmount, presetPath);

        var gameTimer = new System.Timers.Timer(interval) { AutoReset = true, Enabled = true };
        gameTimer.Elapsed += server.Update;
        while (true) { }
    }
}
