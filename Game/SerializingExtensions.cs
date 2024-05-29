using System;
using System.Numerics;
using LiteNetLib.Utils;
using SharedClasses.GameObjects;

public static class SerializingExtensions {
    public static void Put(this NetDataWriter writer, Vector2 vector) {
        writer.Put(vector.X);
        writer.Put(vector.Y);
    }

    public static Vector2 GetVector2(this NetDataReader reader) {
        return new Vector2(reader.GetFloat(), reader.GetFloat());
    }
    
    public static void Put(this NetDataWriter writer, GameState vector) {
        
    }

    public static GameState GetGameState(this NetDataReader reader)
    {
        throw new NotImplementedException();
    }
}