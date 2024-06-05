using System.Collections.Generic;
using System.Runtime.CompilerServices;
using Lime;

namespace Game
{
	public class PlayerComponent : Component
	{
		private readonly int size = 16;
		
		public readonly Image image;
		public new Vector2 Position { get => image.Position; set => image.Position = value; }
		
		public PlayerComponent(Widget canvas, Vector2 newPos, int newPID=0, string spritePath="Sprites/Hero")
		{
			EntityId = newPID;
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
