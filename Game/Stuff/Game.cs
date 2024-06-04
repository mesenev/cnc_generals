using System;
using System.Collections.Generic;
using Game.Commands;
using Game.GameObjects;
using Game.Network;
using Lime;
using HexCell = Game.Map.HexCell;
using HexGrid = Game.Map.HexGrid;

namespace Game.Stuff {
    public delegate void UpdateDelegate();

    public class Game {
        public Widget Canvas { get; set; }

        public readonly List<Component> Components = [];
        public readonly List<IProcessor> Processors = [];

        private Client _client;

        private HexGrid hexGrid;

        public HexCell SelectedCell = null;
        public HexCell DestinationCell = null;

        public Game(Client client) {
            _client = client;
            _client.Connect("Player");

            Canvas = CanvasManager.Instance.GetCanvas(Layers.Entities);
        }

        private void InitHexGrid(Widget canvas, int width, int height) {
            hexGrid = new HexGrid(canvas,width,height);

            foreach (var cell in hexGrid.cells) {
                Processors.Add(new HexInteractionProcessor(cell));
            }
        }

        public void Update(float delta) {
            // Console.WriteLine("Update");
            foreach (var processor in Processors) {
                processor.Update(delta, this);
            }

            if (SelectedCell != null && DestinationCell != null) {
                _client.commands.Enqueue(new MoveCommand2 {
                    unitId = SelectedCell.CellUnitId, x = DestinationCell.XCoord, y = DestinationCell.YCoord
                });
                SelectedCell = null;
                DestinationCell = null;
            }

            _client.Update();
        }

        public void UpdatePlayers(float delta) {
            if (_client.gameState != null && hexGrid == null) {
                InitHexGrid(CanvasManager.Instance.GetCanvas(Layers.HexMap), _client.gameState.Grid.width,
                    _client.gameState.Grid.height);
            }

            RemovePlayersFromCanvas();
            GetPlayersFromServer();
            UpdateHexCells();
        }

        private void RemovePlayersFromCanvas() {
            Canvas.Nodes.RemoveAll(node => true);
            Components.RemoveAll(el => true);
            Processors.RemoveAll(el => el.GetType() == typeof(PlayerInputProcessor));
        }

        private void GetPlayersFromServer() {
            if (_client.gameState == null)
                return;
            foreach (var unit in _client.gameState.Units) {
                var newUnit = new UnitComponent(
                    Canvas,
                    hexGrid.cells[unit.x, unit.y].GetPosition((float)unit.x, (float)unit.y),
                    unit.UnitId,
                    spritePath: "Sprites/Unit"
                );
                Components.Add(newUnit);
                Processors.Add(new PlayerInputProcessor(newUnit));
            }
        }

        private void UpdateHexCells() {
            if (_client.gameState == null)
                return;
            foreach (GameObjects.HexCell cell in _client.gameState.Grid.cells) {
                hexGrid.cells[cell.YCoord, cell.XCoord].XCoord = cell.XCoord;
                hexGrid.cells[cell.YCoord, cell.XCoord].YCoord = cell.YCoord;
                hexGrid.cells[cell.YCoord, cell.XCoord].Occupied = cell.Occupied;
                hexGrid.cells[cell.YCoord, cell.XCoord].CellUnitId = cell.CellUnitId;
            }
        }
    }
}
