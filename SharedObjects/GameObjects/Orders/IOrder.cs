using LiteNetLib.Utils;

namespace Game.GameObjects.Orders {
    public interface IOrder : INetSerializable {
        OrderStatus Update(GameState state);
    }

    public enum OrderStatus {
        Executing,
        Finished
    }
}
