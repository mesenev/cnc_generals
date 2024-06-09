using System;
using Lime;

namespace Game.Map {
    public class HexCell {
        public int XCoord;
        public int YCoord;
        public int GridX;
        public int GridY;
        public int CellUnitId = -1;
        public bool Occupied = false;
        private int size = 64;
        public readonly Image image;

        public Vector2 PixelPosition {
            get => image.Position;
            set => image.Position = value;
        }

        public Vector2 HexPosition;

        public HexCell(Widget canvas, Vector2 newPos, int xCoord, int yCoord, int gridX,
            int gridY) {
            this.XCoord = xCoord;
            this.YCoord = yCoord;
            this.GridX = gridX;
            this.GridY = gridY;
            image = new Image {
                Sprite = new SerializableSprite("Sprites/Cell"), Size = new Vector2(size, size),
                Pivot = Vector2.Half,
            };
            HexPosition = newPos;
            image.Position = GetPosition(newPos.X, newPos.Y);
            canvas.Nodes.Add(image);
        }

        public Vector2 GetPosition(float x, float y) {
            return new Vector2(
                x * image.Height + ((y + 1) % 2 * image.Height / 2) + image.Height / 2,
                (GridY - y) * 0.75f * image.Width
            );
        }
    }
}
