using System.Collections.Generic;
using System.Linq;
using Game.GameObjects.Units;
using Game.Map;
using Game.Stuff;
using Game.Widgets;
using Lime;
using SharedObjects.Commands;
using SharedObjects.Network;
using HexCell = Game.Map.HexCell;
using HexGrid = Game.Map.HexGrid;

namespace Game.GameObjects;

public class Game {
    public Widget Canvas { get; set; }

    private readonly List<Component> components =  [];
    private readonly List<IProcessor> processors =  [];

    private HexGrid hexGrid;
    private FowGrid fowGrid;
    private OccupationGrid occupationGrid;
    private ClientGameState gameState;

    public HexCell SelectedCell;
    public HexCell DestinationCell;

    // нужно выносить это все в level
    private Camera2D camera;
    private Viewport2D viewport;

    public Game(ClientGameState gameState, Widget scene) {
        this.gameState = gameState;

        The.NetworkClient.OnGameStateUpdateEvent += UpdateGameState;

        InitializeViewportAndCameraAndAddToWidget(scene);
        CanvasManager.Instance.InitLayers(viewport);

        SetSpriteToBackground(CanvasManager.Instance.GetCanvas(Layers.Background));
        Canvas = CanvasManager.Instance.GetCanvas(Layers.Entities);

        // ToDO скорее всего процесс перемещения камеры должен создаваться в другом месте
        var moveCameraProcessor = new MoveCameraProcessor(viewport);
        processors.Add(moveCameraProcessor);
    }

    private void UpdateGameState(PlayerReceiveUpdatePacket packet) {
        this.gameState = new ClientGameState(packet.state);
    }


    private void InitializeViewportAndCameraAndAddToWidget(Widget parent) {
        viewport = new Viewport2D();
        viewport.Size = parent.Size;
        viewport.Position = parent.Size * 0;
        viewport.Pivot = Vector2.Zero;
        viewport.Anchors = Anchors.LeftRightTopBottom;
        parent.AddNode(viewport);

        camera = new Camera2D();
        camera.X = viewport.Width * 0.5f;
        camera.OrthographicSize = viewport.Height;
        camera.Y = viewport.Height * 0.5f;
        camera.Pivot = Vector2.Zero;

        viewport.Camera = camera;
        viewport.AddNode(camera);
    }

    private void InitHexGrid(Widget canvas, int width, int height) {
        hexGrid = new HexGrid(canvas, width, height);
        fowGrid = new FowGrid(gameState.Units.ToList());
        occupationGrid = new OccupationGrid(width, height);
        fowGrid.InitFow(width, height);

        foreach (var cell in hexGrid.cells) {
            processors.Add(new HexInteractionProcessor(cell, viewport));
        }
    }

    private static void SetSpriteToBackground(Widget canvas) {
        canvas.AddNode(
            new Image {
                Sprite = new SerializableSprite("Sprites/Grass"),
                Size = new Vector2(The.World.Width, The.World.Height)
            }
        );
    }

    public void Update(float delta) {
        // Console.WriteLine("Update");
        foreach (var processor in processors) {
            processor.Update(delta, this);
        }

        if (SelectedCell != null && DestinationCell != null) {
            if (SelectedCell.CellUnitId != -1)
                The.NetworkClient.commands.Enqueue(
                    new MoveCommand {
                        unitId = SelectedCell.CellUnitId, x = DestinationCell.XCoord,
                        y = DestinationCell.YCoord
                    }
                );
            SelectedCell = null;
            DestinationCell = null;
        }

        The.NetworkClient.Update();
    }

    public void UpdatePlayers(float delta) {
        if (hexGrid == null) {
            InitHexGrid(
                CanvasManager.Instance.GetCanvas(Layers.HexMap), gameState.Grid.Width,
                gameState.Grid.Height
            );
        }

        RemovePlayersFromCanvas();
        GetPlayersFromServer();
        UpdateHexCells();
    }

    private void RemovePlayersFromCanvas() {
        Canvas.Nodes.RemoveAll(el => el.GetType() == typeof(Image));
        components.RemoveAll(_ => true);
        processors.RemoveAll(el => el.GetType() == typeof(PlayerInputProcessor));
    }

    private void GetPlayersFromServer() {
        foreach (var unit in gameState.Units) {
            var newUnit = new UnitComponent(
                Canvas,
                hexGrid.cells[unit.Y, unit.X].GetPosition(unit.X, unit.Y),
                unit
            );
            components.Add(newUnit);
            processors.Add(new PlayerInputProcessor(newUnit));
        }

        fowGrid.UpdateUnits(
            gameState.Units
                .Where(unit => unit.OwnerId == The.NetworkClient.GetClientPlayer().playerId).ToList()
        );
        occupationGrid.Occupy(
            gameState.Units.ToList(), The.NetworkClient.GetClientPlayer().playerId
        );
        fowGrid.RecalculateFow();
    }

    private void UpdateHexCells() {
        foreach (SharedObjects.GameObjects.HexCell cell in gameState.Grid.cells) {
            hexGrid.cells[cell.YCoord, cell.XCoord].XCoord = cell.XCoord;
            hexGrid.cells[cell.YCoord, cell.XCoord].YCoord = cell.YCoord;
            hexGrid.cells[cell.YCoord, cell.XCoord].CellUnitId = cell.CellUnitId;
        }
    }
}
