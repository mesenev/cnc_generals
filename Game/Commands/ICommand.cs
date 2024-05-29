using LiteNetLib.Utils;

namespace Game.Commands;

public interface ICommand : INetSerializable
{
    public void Execute();
}