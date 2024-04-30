using System.Numerics;

namespace test_app;

class Parser
{
    List<string> list_of_commands;

    public Parser(List<string> commands)
    {
        list_of_commands = commands;
    }

    public Vector<string> parseStringToCommands(string inpurt_string)
    {
        return new Vector<string>(inpurt_string.Split());
    }
}