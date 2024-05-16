using System;
using Lime;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using Game.Map;

namespace Game
{
	public delegate void UpdateDelegate();

	public class Game
	{
		public Widget Canvas { get; set; }

		public readonly List<Component> Components = new();
		public readonly List<IProcessor> Processors = new();

		private Client _client;

		private PlayerComponent mainPlayer;

		private HexGrid hexGrid;

		public Game(Widget canvas, Client client)
		{
			// initHexGrid(canvas);
			
			_client = client;
			_client.Connect("Player");

			Canvas = canvas;
		}

		private void initHexGrid(Widget canvas)
		{
			hexGrid = new HexGrid(canvas);

			foreach (var cell in hexGrid.cells) {
				Processors.Add(new HexInteractionProcessor(cell));
			}
		}

		private void CallIfPlayerConnected(UpdateDelegate func)
		{
			if (_client.isPLayerJoined) {
				func();
			}
		}

		public void SetMainPlayer(int pid)
		{
			mainPlayer = new PlayerComponent(Canvas, Canvas.Size / 2, pid);
			var playerInputProcessor = new PlayerInputProcessor(mainPlayer);

			Components.Add(mainPlayer);
			Processors.Add(playerInputProcessor);
		}

		public void Update(float delta)
		{
			foreach (var processor in Processors) {
				processor.Update(delta, this);
			}

			_client.Update();
			if (_client.isPLayerJoined && mainPlayer != null) {
				_client.UpdateClientPlayer(mainPlayer.Position);
			}

			if (_client.isPLayerJoined && mainPlayer == null) {
				SetMainPlayer((int)_client.GetClientPlayer().state.pid);
			}
		}

		public void UpdatePlayers(float delta)
		{
			CallIfPlayerConnected(RemovePlayersFromCanvas);

			CallIfPlayerConnected(GetPlayersFromServer);
		}

		private void RemovePlayersFromCanvas()
		{
			Canvas.Nodes.RemoveAll(node => node != mainPlayer.image);

			Components.RemoveAll(el => el.EntityId != mainPlayer.EntityId);
		}

		private void GetPlayersFromServer()
		{
			foreach (var remotePlayer in _client.GetServerPlayers()
				         .FindAll(el => (int)el.state.pid != mainPlayer.EntityId)) {
				Components.Add(
					new PlayerComponent(Canvas, remotePlayer.state.position, spritePath: "Sprites/Unit") {
						EntityId = (int)remotePlayer.state.pid
					});
			}
		}
	}
}
