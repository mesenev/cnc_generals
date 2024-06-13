using System;
using Lime;

namespace Game.Dialogs;

public class InGameScreen : Dialog<Scenes.Data.GameScreen> {
    
    public InGameScreen() {
        // SoundManager.PlayMusic("Ingame");
        Scene._BtnExit.It.Clicked = ReturnToMenu;
    }


    protected override void Shown() {
        var canvas = Scene.It["Scene1"];
        Console.WriteLine("Hello from ingame screen");
    }

    protected override void Closing() {
        NetworkClient.Disconnect();
    }

    protected override void Update(float delta) {
        if (Input.WasKeyPressed(Key.Escape)) {
            ReturnToMenu();
        }
    }

    protected override bool HandleAndroidBackButton() {
        ReturnToMenu();
        return true;
    }

    private void ReturnToMenu() {
        var confirmation = new Confirmation("Are you sure?");
        confirmation.OkClicked += CrossfadeInto<MainMenu>;
        Open(confirmation);
    }
}
