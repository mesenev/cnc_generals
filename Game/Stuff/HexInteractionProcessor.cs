using System;
using Game.Map;
using Lime;
using Game.Widgets;

namespace Game.Stuff;

public class HexInteractionProcessor(HexCell cell, Viewport2D viewport) : IProcessor {
    private readonly HexCell _cell = cell;
    private bool keyPressed;
    private Viewport2D viewport = viewport;

    public void Update(float delta, GameObjects.Game game) {
        var canvasInput = game.Canvas.Input;
        if (!IsMouseOver(viewport.ViewportToWorldPoint(Window.Current.Input.MousePosition))) {
            // _cell.image.Color = Color4.Gray;
            return;
        }

        _cell.image.Color = Color4.White;

        if (!keyPressed && !canvasInput.IsKeyPressed(Key.Mouse0)) return;

        keyPressed = true;
        _cell.image.Color = Color4.Blue;

        if (!keyPressed || !canvasInput.WasKeyReleased(Key.Mouse0)) return;

        keyPressed = false;
        if (game.SelectedCell == null) {
            game.SelectedCell = _cell;
            Console.WriteLine($"Selected {_cell.XCoord} {_cell.YCoord}");
        } else {
            Console.WriteLine($"Destination {_cell.XCoord} {_cell.YCoord}");
            game.DestinationCell = _cell;
        }
    }

    private bool IsMouseOver(Vector2 mousePos) {
        float sideSize = 20 * float.Abs(3) / 2;
        var pixelPos = _cell.PixelPosition;
        return Vector2.Distance(pixelPos, mousePos) < sideSize;
    }
}
