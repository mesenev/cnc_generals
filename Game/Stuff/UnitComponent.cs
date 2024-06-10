using Lime;

namespace Game.Stuff
{
	public class UnitComponent : Component
	{
		private readonly int size = 16;
		
		public readonly Image image;
		public Vector2 Position { get => image.Position; set => image.Position = value; }
		
		public UnitComponent(Widget canvas, Vector2 newPos, int unitId, string spritePath="Sprites/Hero")
		{
			EntityId = unitId;
			image = new Image {
				Sprite = new SerializableSprite(spritePath),
				Size = new Vector2(size, size),
				Position = newPos,
				Pivot = Vector2.Half,
			};
			canvas.AddNode(image);
		}
	}
}
