using System;

namespace Application;

public class Application
{
[STAThread]
public static void Main(string[] args)
{
Lime.Application.Initialize(new Lime.ApplicationOptions());
Game.Application.Application.Initialize();
Lime.Application.Run();
}
}
