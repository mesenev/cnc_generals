using System.Collections.Generic;
using Game.Commands;
using Game.Network;
using Game.Widgets;
using Lime;
using HexCell = Game.Map.HexCell;
using HexGrid = Game.Map.HexGrid;

namespace Game.Stuff;

public delegate void UpdateDelegate();

public class Game {
    public Widget Canvas { get; set; }

    private readonly List<Component> components = [];
    private readonly List<IProcessor> processors = [];

    private readonly Client client;

    private HexGrid hexGrid;

    public HexCell SelectedCell;
    public HexCell DestinationCell;

    // нужно выносить это все в level
    private Camera2D camera;
    private Viewport2D viewport;

    public Game(Client client, Widget scene) {
        this.client = client;
        this.client.Connect("Player");


        InitializeViewportAndCameraAndAddToWidget(scene);
        CanvasManager.Instance.InitLayers(viewport);

        SetSpriteToBackground(CanvasManager.Instance.GetCanvas(Layers.Background));
        Canvas = CanvasManager.Instance.GetCanvas(Layers.Entities);

        // ToDO скорее всего процесс перемещения камеры должен создаваться в другом месте
        var moveCameraProcessor = new MoveCameraProcessor(camera);
        var voiceStreamingProcessor = new VoiceStreamingProcessor();
        
        processors.Add(moveCameraProcessor);
        processors.Add(voiceStreamingProcessor);
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

        foreach (var cell in hexGrid.cells) {
            processors.Add(new HexInteractionProcessor(cell, viewport));
        }
    }

    private static void SetSpriteToBackground(Widget canvas) {
        canvas.AddNode(new Image {
            Sprite = new SerializableSprite("Sprites/Grass"), 
            Size = new Vector2(The.World.Width, The.World.Height)
        });
    }

    public void Update(float delta) {
        // Console.WriteLine("Update");
        foreach (var processor in processors) {
            processor.Update(delta, this);
        }

        if (SelectedCell != null && DestinationCell != null) {
            client.commands.Enqueue(new MoveCommand2 {
                unitId = SelectedCell.CellUnitId, x = DestinationCell.XCoord, y = DestinationCell.YCoord
            });
            SelectedCell = null;
            DestinationCell = null;
        }

        client.Update();
    }

    public void UpdatePlayers(float delta) {
        if (client.gameState != null && hexGrid == null) {
            InitHexGrid(CanvasManager.Instance.GetCanvas(Layers.HexMap), client.gameState.Grid.width,
                client.gameState.Grid.height);
        }

        RemovePlayersFromCanvas();
        GetPlayersFromServer();
        UpdateHexCells();
    }

    private void RemovePlayersFromCanvas() {
        Canvas.Nodes.RemoveAll(_ => true);
        components.RemoveAll(_ => true);
        processors.RemoveAll(el => el.GetType() == typeof(PlayerInputProcessor));
    }

    private void GetPlayersFromServer() {
        if (client.gameState == null)
            return;
        foreach (var unit in client.gameState.Units) {
            var newUnit = new UnitComponent(
                Canvas,
                hexGrid.cells[unit.x, unit.y].GetPosition(unit.x, unit.y),
                unit.UnitId,
                spritePath: "Sprites/Unit"
            );
            components.Add(newUnit);
            processors.Add(new PlayerInputProcessor(newUnit));
        }
    }

    private void UpdateHexCells() {
        if (client.gameState == null)
            return;
        foreach (GameObjects.HexCell cell in client.gameState.Grid.cells) {
            hexGrid.cells[cell.YCoord, cell.XCoord].XCoord = cell.XCoord;
            hexGrid.cells[cell.YCoord, cell.XCoord].YCoord = cell.YCoord;
            hexGrid.cells[cell.YCoord, cell.XCoord].Occupied = cell.Occupied;
            hexGrid.cells[cell.YCoord, cell.XCoord].CellUnitId = cell.CellUnitId;
        }
    }
}
