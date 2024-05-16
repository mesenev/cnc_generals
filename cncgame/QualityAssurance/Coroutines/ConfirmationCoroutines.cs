using Game.Dialogs;
using RemoteScriptingClient;

namespace QualityAssurance.Coroutines
{
	public class ConfirmationCoroutines
	{
		public static async Coroutine Confirm(Confirmation dialog = null, WaitDialogTaskParameters parameters = null) =>
			await CommonCoroutines.CloseDialog(dialog, parameters, "@BtnOk");

		public static async Coroutine Decline(Confirmation dialog = null, WaitDialogTaskParameters parameters = null) =>
			await CommonCoroutines.CloseDialog(dialog, parameters, "@BtnCancel");
	}
}
