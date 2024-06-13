using Game.Widgets;
using Lime;

namespace Game.Stuff;

public class MoveCameraProcessor : IProcessor {
    public Viewport2D Viewport2D { get; }

    private Camera2D Camera {
        get { return Viewport2D.Camera; }
    }

    public MoveCameraProcessor(Viewport2D viewport2D) {
        Viewport2D = viewport2D;
    }

    public void Update(float delta, GameObjects.Game game) {

        if (Window.Current.Input.IsKeyPressed(Key.Tab)) {
            Viewport2D.UserInterface.TopContainer.Visible = true;
        }

        if (Window.Current.Input.WasKeyReleased(Key.Tab)) {
            Viewport2D.UserInterface.TopContainer.Visible = false;
        }


        if (Window.Current.Input.IsKeyPressed(Key.Down)) {
            if (Camera.X < 64 * 50 - Camera.Parent.AsWidget.Size.X / 2) {
                Camera.Y += 10;
            }
        }

        if (Window.Current.Input.IsKeyPressed(Key.Up)) {
            if (Camera.Y > Camera.Parent.AsWidget.Height / 2) {
                Camera.Y -= 10;
            }
        }

        if (Window.Current.Input.IsKeyPressed(Key.Left)) {
            if (Camera.X > Camera.Parent.AsWidget.Width / 2) {
                Camera.X -= 10;
            }
        }

        if (Window.Current.Input.IsKeyPressed(Key.Right)) {
            if (Camera.X < 64 * 50) {
                   Camera.X += 10;
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
