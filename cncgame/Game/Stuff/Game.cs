using System;
using Lime;
using System.Collections.Generic;
using System.Linq;
using System.Threading;

namespace Game
{
	public class Game
	{
		private int entityCounter;
		public Widget Canvas { get; set; }

		public readonly List<Component> Components = new();
		public readonly List<IProcessor> Processors = new();
		
		private Server server;
		private Client client;

		private PlayerComponent mainPlayer;

		public Game(Widget canvas, Server server, Client client)
		{
			this.server = server;
			this.client = client;

			mainPlayer = new PlayerComponent(canvas, canvas.Size / 2, (int)server.GetNewPID());

			var playerInputProcessor = new PlayerInputProcessor(mainPlayer);

			Canvas = canvas;

			Components.Add(mainPlayer);
			Processors.Add(playerInputProcessor);
		}

		public void Update(float delta)
		{
			foreach (var processor in Processors) {
				processor.Update(delta, this);
			}

			client.UpdateClientPlayer(mainPlayer.Position, (uint)mainPlayer.EntityId);
			client.Update();
		}

		public void UpdateServer(float delta)
		{
			if (server.IsHost()) {
				server.Update();
				Thread.Sleep(15);
			}
		}

		public void UpdatePlayers(float delta)
		{
			RemovePlayersFromCanvas();

			GetPlayersFromServer();
		}

		private void RemovePlayersFromCanvas()
		{
			foreach (var player in Components.FindAll(el => el.EntityId != mainPlayer.EntityId)) {
				Canvas.Nodes.RemoveAt(player.EntityId);
			}

			Components.RemoveAll(el => el.EntityId != mainPlayer.EntityId);
		}

		private void GetPlayersFromServer()
		{
			foreach (var remotePlayer in client.GetServerPlayers()) {
				Components.Add(
					new PlayerComponent(Canvas, remotePlayer.state.position, spritePath: "Sprites/Unit") {
						EntityId = (int)remotePlayer.state.pid
					});
			}
		}
	}
}
