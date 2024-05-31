using System;
using Lime;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using Game.GameObjects;
using Game.Network;
using HexGrid = Game.Map.HexGrid;

namespace Game {
    public delegate void UpdateDelegate();

    public class Game {
        public Widget Canvas { get; set; }

        public readonly List<Component> Components = [];
        public readonly List<IProcessor> Processors = [];

        private Client _client;

        private HexGrid hexGrid;

        public Game(Client client) {
            initHexGrid(CanvasManager.Instance.GetCanvas(Layers.HexMap));

            _client = client;
            _client.Connect("Player");

            Canvas = CanvasManager.Instance.GetCanvas(Layers.Entities);
        }

        private void initHexGrid(Widget canvas) {
            hexGrid = new HexGrid(canvas);

            foreach (var cell in hexGrid.cells) {
                Processors.Add(new HexInteractionProcessor(cell));
            }
        }

        public void Update(float delta) {
            Console.WriteLine("Update");
            foreach (var processor in Processors) {
                processor.Update(delta, this);
            }

            _client.Update();
        }

        public void UpdatePlayers(float delta) {
            RemovePlayersFromCanvas();
            GetPlayersFromServer();
        }

        private void RemovePlayersFromCanvas() {
            Canvas.Nodes.RemoveAll(node => true);
            Components.RemoveAll(el => true);
            Processors.RemoveAll(el => true);
        }

        private void GetPlayersFromServer() {
            if (_client.gameState == null)
                return;
            foreach (var unit in _client.gameState.Units) {
                var newUnit = new UnitComponent(
                    Canvas,
                    hexGrid.cells[0, 0].GetPosition((float)unit.x, (float)unit.y),
                    unit.UnitId,
                    spritePath: "Sprites/Unit"
                );
                Components.Add(newUnit);
                Processors.Add(new PlayerInputProcessor(newUnit));
            }
        }
    }
}
