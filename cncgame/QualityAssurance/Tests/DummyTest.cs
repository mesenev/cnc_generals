using RemoteScriptingClient;

namespace QualityAssurance.Tests
{
	/// <summary>
	/// This test should be used for quick testing only. Use it if you want to check something small.
	/// If you have changes in this file that you want to commit - you are probably doing something wrong.
	/// </summary>
	public class DummyTest : TestCoroutine
	{
		protected override async Coroutine RunTest()
		{
			using var formLocker = new WindowsFormLocker();
			using var timeFlow = new FramesSkippingTimeFlow();
			// TODO: Write your code here
			await Command.WaitNextFrame();
		}
	}
}
