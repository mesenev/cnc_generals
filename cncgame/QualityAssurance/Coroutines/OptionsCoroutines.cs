using Game.Dialogs;
using RemoteScriptingClient;

namespace QualityAssurance.Coroutines
{
	public class OptionsCoroutines
	{
		public static async Coroutine Close(Options dialog = null, WaitDialogTaskParameters parameters = null) =>
			await CommonCoroutines.CloseDialog(dialog, parameters, "@BtnOk");
	}
}
