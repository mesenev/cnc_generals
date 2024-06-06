using Terminal.Gui;

namespace Server.InterfaceViews;

public sealed class PromptAndStatusView : View {
    private readonly TextField prompt = new("some editable text") {
        X = 3, Y = 0,
        Height = 1, Width = 30
    };
    private readonly Label promptCursor = new(">:") { X = 0, Y = 0, Height = 1, };

    private ProgressBar serverStatus;
        
    public PromptAndStatusView() {
        InitializeComponent();
    }

    private void InitializeComponent() {
        this.Width = Dim.Fill();
        this.Height = Dim.Fill();
        Add(promptCursor);
        Add(prompt);
        var serverLabel = new Label("server is:") { X = 40, Y = 0 };
        Add(serverLabel);
        serverStatus = new ProgressBar {
            ProgressBarFormat = ProgressBarFormat.Simple, ProgressBarStyle = ProgressBarStyle.Continuous,
            X = Pos.Right(serverLabel) + 1, Y = 0, Width = 2,
        };
        Add(serverStatus);
        var gameStatusLabel = new Label("game state:") { X = 40, Y = 1 };
        Add(gameStatusLabel);
        var gameStatus = new Label("waiting players 0/2") { X = Pos.Right(gameStatusLabel) + 1, Y = 1 };
        Add(gameStatus);
        var presetLabel = new Label("preset:") { X = 40, Y = 2 };
        Add(presetLabel);
        var presetName = new Label("default.txt"){ X = Pos.Right(presetLabel) + 1, Y = 2};
        Add(presetName);
        
        Add(new Label("F1 - current") { X = 45, Y = 4 }); 
        Add(new Label("F2 - game state"){ X = 45, Y = 5 });  
        Add(new Label("F3 - logs"){X=45, Y = 6}); 
        
        serverStatus.Pulse();

    }
    
    
}
