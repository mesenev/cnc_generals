using System.Collections.Generic;
using System.Runtime.CompilerServices;
using Lime;

namespace Game
{
	public class UnitComponent : Component
	{
		private readonly int size = 16;
		
		public readonly Image image;
		public Vector2 Position { get => image.Position; set => image.Position = value; }
		
		public UnitComponent(Widget canvas, Vector2 newPos, uint unitId, string spritePath="Sprites/Hero")
		{
			EntityId = unitId;
			image = new Image {
				Sprite = new SerializableSprite(spritePath),
				Size = new Vector2(size, size),
				Position = newPos,
				Pivot = Vector2.Half,
			};
			canvas.Nodes.Add(image);
		}
	}
}
