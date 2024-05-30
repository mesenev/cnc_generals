using LiteNetLib.Utils;

namespace Game.Commands
{
	public struct MoveCommand2 : INetSerializable
	{
		public uint unitId;
		public int x;
		public int y;
		public void Serialize(NetDataWriter writer)
		{
			writer.Put(unitId);
			writer.Put(x);
			writer.Put(y);
		}

		public void Deserialize(NetDataReader reader)
		{
			unitId=reader.GetUInt();
			x=reader.GetInt();
			y=reader.GetInt();
		}
	}
}
