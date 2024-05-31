using CommandLine;

namespace Server;

public class Program {
    private class Options {
        [Option('a', "players", Required = true, HelpText = "Set amount of players for server to await")]
        public int PlayersAmount { get; set; }

        [Option('p', "preset", Required = true, HelpText = "Path to the preset of initial game state")]
        public string? PresetPath { get; set; }
    }

    public static void Main(string[] args) {
        StreamReader sr = new StreamReader("GameStatePresets/default.txt");
        Console.WriteLine(sr.ReadLine());
        int playersAmount = default;
        string? presetPath = default!;
        Parser.Default.ParseArguments<Options>(args).WithParsed<Options>(o => {
            playersAmount = o.PlayersAmount;
            presetPath = o.PresetPath;
        });
        Console.WriteLine(playersAmount);
        Console.WriteLine(presetPath);
        var server = new Server(playersAmount, presetPath);
        while (true) {
            server.Update();
        }
    }
}
