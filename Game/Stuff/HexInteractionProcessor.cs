using System;
using Game.Map;
using Lime;

namespace Game.Stuff {
    public class HexInteractionProcessor : IProcessor {
        private HexCell _cell;
        private bool keyPressed = false;

        public HexInteractionProcessor(HexCell cell) {
            _cell = cell;
        }

        public void Update(float delta, Game game) {
            var canvasInput = game.Canvas.Input;
            if (IsMouseOver(canvasInput.MousePosition)) {
                _cell.image.Color = Color4.White;
                if (keyPressed||canvasInput.IsKeyPressed(Key.Mouse0)) {
                    keyPressed = true;
                    _cell.image.Color = Color4.Blue;
                    if (keyPressed&&canvasInput.WasKeyReleased(Key.Mouse0)) {
                        keyPressed = false;
                        if (game.SelectedCell == null) {
                            game.SelectedCell = _cell;
                            Console.WriteLine($"Selected {_cell.XCoord} {_cell.YCoord}");
                        } else {
                            Console.WriteLine($"Destination {_cell.XCoord} {_cell.YCoord}");
                            game.DestinationCell = _cell;
                        }
                    }
                }
            } else {
                _cell.image.Color = Color4.Gray;
            }
        }

        private bool IsMouseOver(Vector2 mousePos) {
            float sideSize = 20 * float.Abs(3) / 2;
            var pixelPos = _cell.PixelPosition;
            return Vector2.Distance(pixelPos, mousePos) < sideSize;
        }
    }
}
