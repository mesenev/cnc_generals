using System;
using Lime;

namespace Game.Dialogs;

public class GameScreen : Dialog<Scenes.Data.GameScreen>
{
	public GameScreen()
	{
		SoundManager.PlayMusic("Ingame");
		Scene._BtnExit.It.Clicked = ReturnToMenu;
	}

	private Game game;

	protected override void Shown()
	{
		var canvas = Scene.It["Scene1"];
		game = new Game(canvas, Server, Client);
		canvas.Updated += game.Update;
		canvas.Updated += game.UpdateServer;
		canvas.Updated += game.UpdatePlayers;
	}

	protected override void Closing()
	{
		if (Server.IsHost()) {
			Server.Stop();
		}
		Client.Disconnect();
	}

	protected override void Update(float delta)
	{
		if (Input.WasKeyPressed(Key.Escape)) {
			ReturnToMenu();
		}
	}

	protected override bool HandleAndroidBackButton()
	{
		ReturnToMenu();
		return true;
	}

	private void ReturnToMenu()
	{
		var confirmation = new Confirmation("Are you sure?");
		confirmation.OkClicked += CrossfadeInto<MainMenu>;
		Open(confirmation);
	}
}
