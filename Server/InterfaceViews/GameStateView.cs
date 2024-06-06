using Terminal.Gui;

namespace Server.InterfaceViews;

public sealed class GameStateView : View {
    public GameStateView() {
        Width = Dim.Fill();
        Height = Dim.Fill();
        Add(new Label("game state view"){X=5,Y=5});
    }
}
