using Terminal.Gui;

namespace Server.InterfaceViews;

public sealed class LogView : View {
    public ListView Data;
    
    public LogView() {
        Width = Dim.Fill();
        Height = Dim.Fill();
        Data = new ListView(new List<string>(){"123", "234"}) {
            X = 1, Y = 1,
            Width = 60, Height = 30, AllowsMarking = false,
        };
        Add(Data);
    }

    public void Update() {
        // Data.SetNeedsDisplay();
    }
}
