using System.Collections.Generic;
using System.Linq;
using System.Threading;
using Game.GameObjects.Units;
using Game.Map;
using Game.Stuff;
using Game.Widgets;
using Lime;
using NAudio.Wave;
using SharedObjects.Commands;
using SharedObjects.Network;
using SharedObjects.TextToSpeech;
using HexCell = Game.Map.HexCell;
using HexGrid = Game.Map.HexGrid;

namespace Game.GameObjects;

public class Game {
    public Widget Canvas { get; set; }

    private readonly List<Component> components =  [];
    private readonly List<IProcessor> processors =  [];

    private static readonly WaveFormat custom = WaveFormat.CreateCustomFormat(
        WaveFormatEncoding.Pcm, 22050,
        1, 44100, 2,
        16
    );

    private readonly BufferedWaveProvider bufferedWaveProvider = new(custom) {
        BufferLength = custom.AverageBytesPerSecond * 60, DiscardOnBufferOverflow = true
    };

    private readonly VoiceReceiverModule voiceReceiver;
    private Thread voiceReceiverThread;
    private WaveOutEvent waveOutEvent = new();


    private HexGrid hexGrid;
    private FowGrid fowGrid;
    private StatusGrid statusGrid;
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

        waveOutEvent.Init(bufferedWaveProvider);
        voiceReceiver = new VoiceReceiverModule(bufferedWaveProvider);
        waveOutEvent.Play();
        voiceReceiverThread = new Thread(voiceReceiver.ReadBytesFromServer);
        voiceReceiverThread.Start();
        InitializeViewportAndCameraAndAddToWidget(scene);
        CanvasManager.Instance.InitLayers(viewport);

        SetSpriteToBackground(CanvasManager.Instance.GetCanvas(Layers.Background));
        Canvas = CanvasManager.Instance.GetCanvas(Layers.Entities);

        // ToDO скорее всего процесс перемещения камеры должен создаваться в другом месте
        var moveCameraProcessor = new MoveCameraProcessor(viewport);
        processors.Add(moveCameraProcessor);

        processors.Add(new VoiceStreamingProcessor());
    }

    private void UpdateGameState(PlayerReceiveUpdatePacket packet) {
        gameState = new ClientGameState(packet.state);
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
        hexGrid = new HexGrid(canvas, width, height, "Sprites/TransparentCell");
        fowGrid = new FowGrid(width, height, GetAllies());
        statusGrid = new StatusGrid(width, height);
        occupationGrid = new OccupationGrid(width, height);

        foreach (var cell in hexGrid.cells) {
            processors.Add(new HexInteractionProcessor(cell, viewport));
        }
    }

    private static void SetSpriteToBackground(Widget canvas) {
        canvas.AddNode(
            new Image {
                Sprite = new SerializableSprite("Sprites/bgmain"),
                Size = new Vector2(The.World.Width, The.World.Height)
            }
        );
    }

    public void Update(float delta) {
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
        UpdateGrids();
        UpdateHexCells();
    }

    private void RemovePlayersFromCanvas() {
        // Canvas.Nodes.RemoveAll(el => el.GetType() == typeof(Image));
        EraseUnitsFromCanvas();
        components.RemoveAll(_ => true);
        processors.RemoveAll(el => el.GetType() == typeof(PlayerInputProcessor));
    }

    private void UpdateGrids() {
        GetUnitsFromGameState();

        fowGrid.UpdateAllies(GetAllies());
        fowGrid.RecalculateFow();
        var visibleArea = fowGrid.GetVisibleCoords();
        foreach (var cell in hexGrid.cells) {
            if (visibleArea.Contains(cell.HexPosition)) cell.DrawUnit(Canvas);
            else cell.EraseUnit(Canvas);
        }

        foreach (var cell in fowGrid.GetVisibleArea()) {
            hexGrid.cells[cell.YCoord, cell.XCoord].DrawUnit(Canvas);
        }

        occupationGrid.Occupy(
            gameState.Units.ToList(), fowGrid.GetVisibleCoords(),
            The.NetworkClient.GetClientPlayer().playerId
        );
    }

    private void EraseUnitsFromCanvas() {
        foreach (var cell in hexGrid.cells) {
            cell.EraseUnit(Canvas);
        }
    }

    private void GetUnitsFromGameState() {
        foreach (var unit in gameState.Units) {
            var newUnit = new UnitComponent(
                hexGrid.cells[unit.Y, unit.X].GetPosition(unit.X, unit.Y),
                unit
            );
            hexGrid.cells[unit.Y, unit.X].SetUnit(newUnit);
            components.Add(newUnit);
            processors.Add(new PlayerInputProcessor(newUnit));
        }
    }

    private List<IDrawableUnit> GetAllies() {
        return gameState.Units
            .Where(unit => unit.OwnerId == The.NetworkClient.GetClientPlayer().playerId).ToList();
    }

    private List<IDrawableUnit> GetEnemies() {
        return gameState.Units
            .Where(unit => unit.OwnerId != The.NetworkClient.GetClientPlayer().playerId).ToList();
    }

    private void UpdateHexCells() {
        foreach (SharedObjects.GameObjects.HexCell cell in gameState.Grid.cells) {
            hexGrid.cells[cell.YCoord, cell.XCoord].XCoord = cell.XCoord;
            hexGrid.cells[cell.YCoord, cell.XCoord].YCoord = cell.YCoord;
            hexGrid.cells[cell.YCoord, cell.XCoord].CellUnitId = cell.CellUnitId;
        }
    }
}
