using System;
using Lime;

namespace Game.Dialogs;

public class MainMenu : Dialog<Scenes.Data.MainMenu>
{
	public MainMenu()
	{
		SoundManager.PlayMusic("Theme");
		Scene._BtnPlay.It.Clicked = CrossfadeInto<GameScreen>;
		// Scene._BtnStartServer.It.Clicked = HostThenCrossfade;
		Scene._BtnConnect.It.Clicked = ConnectThenCossfade;
		Scene._BtnOptions.It.Clicked = Open<Options>;
	}

	// private void HostThenCrossfade()
	// {
	// 	Server.Start();
	// 	Client.Connect("Player");
	// 	CrossfadeInto<GameScreen>();
	// }

	private void ConnectThenCossfade()
	{
		// Client.Connect("Player");
		CrossfadeInto<GameScreen>();
	}

	protected override bool HandleAndroidBackButton()
	{
		return false;
	}

	protected override void Update(float delta)
	{
		if (Input.WasKeyPressed(Key.Escape)) {
			Lime.Application.Exit();
		}
	}
}
