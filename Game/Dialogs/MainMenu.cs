using System;
using Lime;

namespace Game.Dialogs;

public class MainMenu : Dialog<Scenes.Data.MainMenu> {
    public MainMenu() {
        SoundManager.PlayMusic("Theme");
        Scene._TargetPoint.RunAnimationInit();
        Scene._PlayButton.It.Clicked = OpenWaitingScreen;
        Scene._SettingsButton.It.Clicked = Open<Options>;
        Scene._ExitButton.It.Clicked = () => Lime.Application.Exit();
    }

    void OpenWaitingScreen() {
        Scene._DarkScreen.RunAnimationBlackout();
        CrossfadeInto<WaitingGameScreen>();
    }

    protected override bool HandleAndroidBackButton() {
        return false;
    }

    protected override void Update(float delta) {
        if (Input.WasKeyPressed(Key.Escape)) {
            Lime.Application.Exit();
        }
    }
}
