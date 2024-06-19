namespace CommandRecognitionModule.ApiClient;

public class CommandParser {
    private List<string> knownCommands;

    public CommandParser(List<string> known_commands) {
        this.knownCommands = known_commands;
    }

    public string[] parseCommandsFromString(string input_string) {
        return input_string.Split();
    }
}
