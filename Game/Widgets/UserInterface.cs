using System;
using DevPanel;
using Game.Commands;
using Lime;
using Yuzu;
using Menu = Lime.Menu;

namespace Game.Widgets;

// [TangerineRegisterNode]
public class UserInterface {
    private static Menu instance;
    private Widget parentContainer;
    public readonly Frame topContainer;
    private SerializableFont Font = new SerializableFont("RobotoMed");


    public UserInterface(Widget currentContainer) {
        parentContainer = currentContainer;
        topContainer = new Frame {
            Anchors = Anchors.LeftRightTopBottom,
            Size = new Vector2(50, 50),
            Visible = false,
            Shader = ShaderId.Silhouette,
            Color = Color4.DarkGray,
            Presenter = new WidgetFlatFillPresenter(Theme.Colors.TabActive),
        };
        parentContainer.AddNode(topContainer);
        topContainer.CenterOnParent();
        // var listContainer = (Frame)topContainer.Clone();
        topContainer.Layer = Layers.Interface;
        var w = new Widget() {
            Layout = new VBoxLayout(), Size = topContainer.Size,
            Anchors = Anchors.LeftRightTopBottom
        };
        // w.AddNode(listContainer);
        w.AddNode(CreateToolPanel());
        topContainer.AddNode(w);
        // topContainer.AddNode(back);
        // back.CenterOnParent();
        topContainer.SetFocus();
    }

    private Widget CreateToolPanel() {
        const float Height = 50.0f;
        var w = new Widget {
            Height = Height,
            MinHeight = Height,
            MaxHeight = Height,
            Layout = new HBoxLayout() { Spacing = 8.0f }
        };
        var orderInfantry = CreateItemButton();
        var orderArtillery = CreateItemButton();
        var orderAir = CreateItemButton();
        orderInfantry.Id = "cheat_menu_close";
        orderArtillery.Id = "cheat_menu_fold_all";
        orderAir.Id = "cheat_menu_unfold_all";
        orderInfantry.MaxHeight = Height;
        orderArtillery.MaxHeight = Height;
        orderAir.MaxHeight = Height;
        orderInfantry.Text = "Заказать пехоту";
        orderArtillery.Text = "Заказать артиллерию";
        orderAir.Text = "Заказать воздух";
        w.AddNode(orderArtillery);
        w.AddNode(orderAir);
        w.AddNode(orderInfantry);
        orderArtillery.Clicked = () => {
            The.NetworkClient.commands.Enqueue(new OrderUnitCommand(0, 1));
        };
        orderInfantry.Clicked = () => {
            The.NetworkClient.commands.Enqueue(new OrderUnitCommand(0, 0));
        };
        orderAir.Clicked = () => {
            The.NetworkClient.commands.Enqueue(new OrderUnitCommand(0, 2));
        };
        return w;
    }

    private Button CreateItemButton() {
        const int FontSize = 36;
        const int SpaceAfter = -16;
        var b = new Button {
            Layout = new StackLayout(),
        };
        var textPresenter = new RichText {
            Id = "TextPresenter",
            WordSplitAllowed = false,
            Nodes = {
                new TextStyle {
                    Font = Font,
                    Tag = "1",
                    Size = FontSize,
                    SpaceAfter = SpaceAfter,
                    TextColor = Color4.Black,
                    Components = { new SignedDistanceFieldComponent() },
                }
            },
            HAlignment = HAlignment.Center,
            VAlignment = VAlignment.Center
        };
        var bg = new Image {
            Id = "bg",
            Shader = ShaderId.Silhouette,
            Color = Color4.White
        };
        b.AddNode(textPresenter);
        b.AddNode(bg);
        return b;
    }
}