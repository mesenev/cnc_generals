using Terminal.Gui;

namespace Server.InterfaceViews;

public sealed class MainView : View {
    private readonly PromptAndStatusView promptAndStatusView = new() { Y = 2 };
    private readonly GameStateView gameStateView = new();
    private readonly LogView logView = new();
    private int currentViewToDisplay;

    public MainView(Rect rect) : base(rect) {
        Add(new Label("Generals server") {
            X = Pos.Center(), Y = 0,
        });
        Add(promptAndStatusView);
        Add(gameStateView);
        Add(logView);
        this.KeyPress += OnHandleF;
    }

    private void OnHandleF(KeyEventEventArgs args) {
        var k = args.KeyEvent;
        if (k.Key == Key.F1)
            currentViewToDisplay = 0;
        if (k.Key == Key.F2)
            currentViewToDisplay = 1;
        if (k.Key == Key.F3)
            currentViewToDisplay = 2;

        SwitchToOtherView();
        return;
    }

    public void SwitchToOtherView() {
        promptAndStatusView.Visible = false;
        gameStateView.Visible = false;
        logView.Visible = false;

        if (currentViewToDisplay == 0)
            promptAndStatusView.Visible = true;
        if (currentViewToDisplay == 1)
            gameStateView.Visible = true;
        if (currentViewToDisplay == 2)
            gameStateView.Visible = true;

        return;
    }
}
