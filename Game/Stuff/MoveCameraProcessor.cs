using Game.Widgets;
using Lime;

namespace Game.Stuff;

public class MoveCameraProcessor : IProcessor {
    private Camera2D camera;

    public MoveCameraProcessor(Camera2D camera) {
        this.camera = camera;
    }

    public void Update(float delta, Game game) {
        // Logger.Instance.Debug(
        // $"CameraPosition: {camera.Position} "
        // + $" HexLayerSize:"
        // + $" {CanvasManager.Instance.GetCanvas(Layers.HexMap).Size}"
        // );

        if (Window.Current.Input.IsKeyPressed(Key.Down)) {
            if (camera.X < 64 * 50 - camera.Parent.AsWidget.Size.X / 2) {
                camera.Y += 1;
            }
        }

        if (Window.Current.Input.IsKeyPressed(Key.Up)) {
            if (camera.Y > camera.Parent.AsWidget.Height / 2) {
                camera.Y -= 1;
            }
        }

        if (Window.Current.Input.IsKeyPressed(Key.Left)) {
            if (camera.X > camera.Parent.AsWidget.Width / 2) {
                camera.X -= 1;
            }
        }

        if (Window.Current.Input.IsKeyPressed(Key.Right)) {
            if (camera.X < 64 * 50) {
                camera.X += 1;
            }
        }

        // if (isUpPressed) {
        // 	camera.Y -= 1;
        // }
        //
        // if (isDownPressed) {
        // 	camera.Y += 1;
        // }
    }
}
