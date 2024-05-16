using System;
using System.Diagnostics;
using System.Linq;
using Game.Dialogs;
using Game.Debug;
using Lime;
using RemoteScriptingClient;

namespace QualityAssurance
{
	public static partial class Command
	{
		public static async Coroutine<T> WaitDialog<T>(WaitDialogTaskParameters parameters = null) where T : Dialog
		{
			var dialog = await WaitDialogs(parameters, typeof(T));
			return dialog as T;
		}

		public static Coroutine<Dialog> WaitDialogs<T1, T2>(WaitDialogTaskParameters parameters = null)
			where T1 : Dialog where T2 : Dialog
		{
			return WaitDialogs(parameters, typeof(T1), typeof(T2));
		}

		public static Coroutine<Dialog> WaitDialogs<T1, T2, T3>(WaitDialogTaskParameters parameters = null)
			where T1 : Dialog where T2 : Dialog where T3 : Dialog
		{
			return WaitDialogs(parameters, typeof(T1), typeof(T2), typeof(T3));
		}

		public static Coroutine<Dialog> WaitDialogs<T1, T2, T3, T4>(WaitDialogTaskParameters parameters = null)
			where T1 : Dialog where T2 : Dialog where T3 : Dialog where T4 : Dialog
		{
			return WaitDialogs(parameters, typeof(T1), typeof(T2), typeof(T3), typeof(T4));
		}

		public static Coroutine<Dialog> WaitDialogs<T1, T2, T3, T4, T5>(WaitDialogTaskParameters parameters = null)
			where T1 : Dialog where T2 : Dialog where T3 : Dialog where T4 : Dialog where T5 : Dialog
		{
			return WaitDialogs(parameters, typeof(T1), typeof(T2), typeof(T3), typeof(T4), typeof(T5));
		}

		public static Coroutine<Dialog> WaitDialogs<T1, T2, T3, T4, T5, T6>(WaitDialogTaskParameters parameters = null)
			where T1 : Dialog where T2 : Dialog where T3 : Dialog where T4 : Dialog where T5 : Dialog where T6 : Dialog
		{
			return WaitDialogs(parameters, typeof(T1), typeof(T2), typeof(T3), typeof(T4), typeof(T5), typeof(T6));
		}

		public static async Coroutine<Dialog> WaitDialogs(
			WaitDialogTaskParameters parameters,
			params Type[] dialogsTypes
		)
		{
			var searchingDialogsInfo = dialogsTypes.Aggregate(
				string.Empty,
				(s, dialogType) => (s.Length == 0 ? string.Empty : $"{s}, ") + $"{dialogType.Name}"
			);
			parameters ??= WaitDialogTaskParameters.Default;

			RemoteScriptingManager.Instance.Environment.Log(
				$@"{nameof(WaitDialogs)} {searchingDialogsInfo} processing..."
			);
			var time = 0f;
			Stopwatch stopwatch = null;
			if (parameters.UseUnscaledTime) {
				stopwatch = new Stopwatch();
				stopwatch.Start();
			}
			while (true) {
				var dialog = DialogManager.Instance.ActiveDialogs.LastOrDefault(d => {
					var dialogType = d.GetType();
					return dialogsTypes.Any(i => i.IsAssignableFrom(dialogType));
				});
				if (dialog != null && parameters.IsConditionMet(dialog)) {
					RemoteScriptingManager.Instance.Environment.Log(
						$@"{nameof(WaitDialogs)} {dialog.GetType().Name} succeed. {GetVisibleDialogsInfo()}"
					);
					return dialog;
				}
				if (time >= parameters.Duration) {
					break;
				}
				await Wait(parameters.Period);
				time += stopwatch?.ElapsedMilliseconds * 0.001f ?? Mathf.Max(Task.Current.Delta, parameters.Period);
				stopwatch?.Restart();
			}
			stopwatch?.Stop();
			RemoteScriptingManager.Instance.Environment.Log(
				$@"{nameof(WaitDialogs)} {searchingDialogsInfo} failed. {GetVisibleDialogsInfo()}"
			);

			if (!parameters.IsStrictly) {
				return null;
			}
			var userAnswer = await MessageBox.Show($"Failed to found dialog {searchingDialogsInfo}.");
			return userAnswer == MessageBoxResult.Retry ? await WaitDialogs(parameters, dialogsTypes) : null;
		}

		private static string GetVisibleDialogsInfo()
		{
			var visibleDialogsInfo = DialogManager.Instance.ActiveDialogs.Aggregate(
				string.Empty,
				(s, dialog) => (s.Length == 0 ? string.Empty : $"{s}, ") + $"{dialog.GetType().Name} (State: {dialog.State})"
			);
			return visibleDialogsInfo.Length > 0 ? $"Now on screen: {visibleDialogsInfo}." : string.Empty;
		}

		public static Coroutine<bool> WaitWhileDialogOnScreen(Dialog dialog, TaskParameters parameters = null)
		{
			return WaitWhile($"{dialog.GetType().Name} is on screen", () => dialog.Root.Parent != null, parameters);
		}

		public static async Coroutine<bool> Cheat(string cheatPath, TaskParameters parameters = null)
		{
			parameters ??= TaskParameters.Default;

			RemoteScriptingManager.Instance.Environment.Log($@"{nameof(Cheat)} ""{cheatPath}"" processing...");
			var menu = Cheats.ShowMenu();
			var existCheat = DevPanel.Menu.Cheat(cheatPath);
			if (menu.IsShown) {
				menu.Hide();
			}

			if (!existCheat && parameters.IsStrictly) {
				RemoteScriptingManager.Instance.Environment.LogWarning($@"{nameof(Cheat)} ""{cheatPath}"" failed.");
				var userAnswer = await MessageBox.Show($"Failed to found cheat: \"{cheatPath}\".");
				if (userAnswer != MessageBoxResult.Retry) {
					return false;
				}
				await WaitNextFrame();
				return await Cheat(cheatPath, parameters);
			}
			new RemoteScriptingWidgets.NotificationWidget(cheatPath, animationTime: 1);
			await RenderThisFrame();
			RemoteScriptingManager.Instance.Environment.Log($@"{nameof(Cheat)} ""{cheatPath}"" succeed.");
			return true;
		}
	}
}
