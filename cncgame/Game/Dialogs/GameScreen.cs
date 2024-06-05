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
		// canvas.X = 0;
		// canvas.Y = 0;
		// canvas.Height = canvas.Height * 2;
		game = new Game(Client, canvas);
		canvas.Updated += game.Update;
		canvas.Updated += game.UpdatePlayers;
	}

	protected override void Closing()
	{
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
