using System;
using Lime;
using System.Collections.Generic;
using Game.Map;
using Game.Widgets;

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
		
		// нужно выносить это все в level
		private Camera2D camera;
		private Viewport2D viewport;
		
		public Game(Client client, Widget Scene)
		{
			_client = client;
			_client.Connect("Player");
			
			viewport = new Viewport2D();
			
			camera = new Camera2D();
			camera.Pivot = Vector2.Zero;
			
			
			viewport.Camera = camera;
			viewport.Size = Scene.Size;
			viewport.Position = Scene.Size * 0;
			viewport.Pivot = Vector2.Zero;
			
			
			Scene.AddNode(viewport);
			viewport.Anchors = Anchors.LeftRightTopBottom;

			camera.X = viewport.Width;
			camera.OrthographicSize = viewport.Height;
			camera.Y = viewport.Height * 0.5f;
			
			viewport.AddNode(camera);
			
			CanvasManager.Instance.InitLayers(viewport);
			
			initHexGrid(CanvasManager.Instance.GetCanvas(Layers.HexMap));
			setSpriteToBackground(CanvasManager.Instance.GetCanvas(Layers.Background));
			Canvas = CanvasManager.Instance.GetCanvas(Layers.Entities);
			
			//Scene.PushNode(camera);
		}

		private void setSpriteToBackground(Widget canvas)
		{
			canvas.AddNode(new Image {
				Sprite = new SerializableSprite("Sprites/Grass"),
				Size = new Vector2(The.World.Width, The.World.Height)
			});
		}

		private void initHexGrid(Widget canvas)
		{
			hexGrid = new HexGrid(canvas);
			
			foreach (var cell in hexGrid.cells) {
				Processors.Add(new HexInteractionProcessor(cell, viewport));
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
			 mainPlayer = new PlayerComponent(Canvas, hexGrid.getRandomCellPosition(), pid);
			 var playerInputProcessor = new PlayerInputProcessor(mainPlayer, viewport);
			 var receiveCommandsFromRecognitionModule = new ReceiveCommandsFromCommandRecognitionModule(mainPlayer);
			
			 viewport.PushNode(mainPlayer);
			
			 Components.Add(mainPlayer);
			 Processors.Add(playerInputProcessor);
			//Processors.Add(receiveCommandsFromRecognitionModule;
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
			
			camera.X += 10 * delta;
			//camera.OrthographicSize = viewport.Height;
			//camera.Y = viewport.Height * 0.5f;

			// if (img == null) {
			// 	img = new Image();
			// 	img.Sprite = new SerializableSprite();
			// 	img.Size = new Vector2(50, 50);
			// 	img.Pivot = Vector2.Half;
			// 	viewport.PushNode(img);
			// }
			//
			// img.Position = viewport.ViewportToWorldPoint(new Vector2(0, 0));
			
		}

		private Image img;
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
					new PlayerComponent(Canvas,
						new Vector2(remotePlayer.state.position.X, remotePlayer.state.position.Y),
						spritePath: "Sprites/Unit") { EntityId = (int)remotePlayer.state.pid });
			}
		}
	}
}
