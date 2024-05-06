using QualityAssurance.Coroutines;
using RemoteScriptingClient;

namespace QualityAssurance.Tests
{
	public class ExampleTest : TestCoroutine
	{
		protected override async Coroutine RunTest()
		{
			using var formLocker = new WindowsFormLocker();
			using var timeFlow = new FramesSkippingTimeFlow();
			var gameScreen = await MainMenuCoroutines.OpenGameScreen();
			await GameScreenCoroutines.Close(gameScreen);
			using (new NormalTimeFlow()) {
				var options = await MainMenuCoroutines.OpenOptions();
				await OptionsCoroutines.Close(options);
			}
		}
	}
}
