using System;
using System.Collections.Generic;
using LiteNetLib.Utils;
using SharedClasses.GameObjects.Units;

namespace SharedClasses.GameObjects;

public class GameState : INetSerializable
{
    public HexGrid Grid = new();
    public List<BaseUnit> Units = [];

    public void AddUnit(BaseUnit newUnit)
    {
        Units.Add(newUnit);
    }

    public void RemoveUnit(BaseUnit unitToRemove)
    {
        Units.Remove(unitToRemove);
    }

    public void Update(TimeSpan timeDelta)
    {
	    throw new NotImplementedException();
    }

    public void Serialize(NetDataWriter writer)
    {
        throw new NotImplementedException();
    }

    public void Deserialize(NetDataReader reader)
    {
        throw new NotImplementedException();
    }
}