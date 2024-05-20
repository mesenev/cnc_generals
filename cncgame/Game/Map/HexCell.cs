using System;
using Lime;

namespace Game.Map
{
	public class HexCell
	{
		private int size = 64;
		public readonly Image image;
		public Vector2 PixelPosition { get => image.Position; set => image.Position = value; }
		public Vector2 HexPosition;
		
		public HexCell(Widget canvas, Vector2 newPos)
		{
			image = new Image {
				Sprite = new SerializableSprite("Sprites/Cell"),
				Size = new Vector2(size, size),
				Pivot = Vector2.Half,
			};
			HexPosition = newPos;
			image.Position = GetPosition(newPos.X, newPos.Y);
			canvas.Nodes.Add(image);
		}

		private Vector2 GetPosition(float x, float y)
		{
			return new Vector2(
			x * 0.75f * image.Width + image.Width / 2,
			y * image.Height + (x % 2 * image.Height / 2) + image.Height / 2
			);
		}
	}
}
