using LiteNetLib.Utils;

namespace SharedObjects.GameObjects.Orders;
    public interface IOrder : INetSerializable {
        OrderStatus Update(GameState state);
    }

    public enum OrderStatus {
        Executing,
        Finished
    }