#if !WEB && !MAC

using System;
using System.Collections.Generic;
using System.Net;
using System.Text.RegularExpressions;
using Lime;
using RemoteScriptingClient;

namespace Game.Extensions;

public class RemoteScriptingClientExtension
{
	private static readonly Dictionary<string, IPAddress> serversAddresses = new Dictionary<string, IPAddress> {
		{ "Localhost", IPAddress.Parse("127.0.0.1") },
	};
	private static string lastEnteredIpAddress;

	public static void Initialize()
	{
		if (RemoteScriptingManager.Instance != null) {
			throw new InvalidOperationException($"{nameof(RemoteScriptingManager)} already initialized");
		}
		var environment = new Environment();
		RemoteScriptingManager.Initialize(environment, string.Empty);
	}

	public static void FillDebugMenuItems(DevPanel.Menu menu)
	{
		var section = menu.Section("Remote Scripting");
		if (!RemoteScriptingManager.Instance.Environment.Client.IsConnected()) {
			foreach (var (serverTitle, ipAddress) in serversAddresses) {
				section.Item(
					serverTitle,
					() => RemoteScriptingManager.Instance.Environment.Client.Connect(ipAddress)
				);
			}
			section.Item(
				"Connect to...",
				() => {
					var ipRegex = new Regex(
						@"^(([0-9]|[1-9][0-9]|1[0-9]{2}|2[0-4][0-9]|25[0-5])\.){3}([0-9]|[1-9][0-9]|1[0-9]{2}|2[0-4][0-9]|25[0-5])$"
					);
					RemoteScriptingWidgets.TextInputWidget scene = null;
					scene = new RemoteScriptingWidgets.TextInputWidget(
						"Enter IP Address",
						lastEnteredIpAddress ?? "192.168.0.1",
						OnDone,
						ipRegex
					);
					The.World.PushNode(scene);
					scene.ExpandToContainerWithAnchors();
					scene.Input.RestrictScope();

					void OnDone(bool success)
					{
						scene.UnlinkAndDispose();
						if (!success) {
							return;
						}
						lastEnteredIpAddress = scene.Value;
						RemoteScriptingManager.Instance.Environment.Client.Connect(IPAddress.Parse(scene.Value));
					}
				}
			);
		} else {
			section.Item("Disconnect", RemoteScriptingManager.Instance.Environment.Client.Disconnect);
		}
	}

	private class Environment : RemoteScriptingManager.IEnvironment
	{
		private readonly System.Reflection.FieldInfo requiredToWaitForWindowRenderingField =
			typeof(Application.Application)
			.GetField(
				"requiredToWaitForWindowRendering",
				System.Reflection.BindingFlags.Instance | System.Reflection.BindingFlags.NonPublic
			);
		private readonly System.Reflection.MethodInfo runScheduledActions =
			typeof(Lime.Application)
			.GetMethod(
				"RunScheduledActions",
				System.Reflection.BindingFlags.Static | System.Reflection.BindingFlags.NonPublic
			);
		private readonly object[] scheduledActionsParameters = new object[1] { 0f };
		private readonly RenderChain worldRenderChain;
		private float? lockedApplicationDelta;

		public IWindow Window => The.Window;
		public WindowInput WindowInput => The.Window.Input;
		public WindowWidget WindowWidget => The.World;
		public RemoteScriptingBehavior Client { get; }
		public TaskList Tasks { get; }
		public object TasksTag { get; } = new object();
		public bool ExistBlockingBackgroundWorker => false;

		public Environment()
		{
			Client = WindowWidget.Components.GetOrAdd<RemoteScriptingBehavior>();
			Tasks = WindowWidget.Components.GetOrAdd<PostLateStageBehavior>().Tasks;
			var worldRenderChainField =
				typeof(WindowWidget)
				.GetField(
					"renderChain",
					System.Reflection.BindingFlags.Instance | System.Reflection.BindingFlags.NonPublic
				);
			worldRenderChain = (RenderChain)worldRenderChainField.GetValue(WindowWidget);
		}

		public void TriggerRenderRequired() => The.App.WaitForRenderingOnNextFrame();

		public void LockApplicationDelta(float delta)
		{
			if (lockedApplicationDelta.HasValue) {
				throw new System.Exception("Can not LockApplicationDelta, call UnlockApplicationDelta before");
			}

			lockedApplicationDelta = delta;
			The.App.CalculatingWorldUpdatingParameters += CalculatingWorldUpdatingParameters;
			The.App.CustomWorldUpdating += CustomWorldUpdating;
			WindowWidget.RenderChainBuilder = null;
			TriggerRenderRequired();
		}

		public void UnlockApplicationDelta()
		{
			if (!lockedApplicationDelta.HasValue) {
				return;
			}

			lockedApplicationDelta = null;
			The.App.CalculatingWorldUpdatingParameters -= CalculatingWorldUpdatingParameters;
			The.App.CustomWorldUpdating -= CustomWorldUpdating;
			WindowWidget.RenderChainBuilder = WindowWidget;
			TriggerRenderRequired();
		}

		public void Log(string message) => Logger.Instance.Info($"[RemoteScripting] {message}");

		public void LogWarning(string message) => Logger.Instance.Warn($"[RemoteScripting] {message}");

		public void LogError(string message) => Logger.Instance.Error($"[RemoteScripting] {message}");

		private bool GetRequiredToWaitForWindowRendering() => (bool)requiredToWaitForWindowRenderingField.GetValue(The.App);

		private void CalculatingWorldUpdatingParameters(ref float delta, ref int iterationsCount, ref bool isTimeQuantized)
		{
			if (!lockedApplicationDelta.HasValue) {
				throw new InvalidOperationException();
			}

			delta = lockedApplicationDelta.Value;
			iterationsCount = int.MaxValue;
			isTimeQuantized = true;
		}

		private void CustomWorldUpdating(
			float delta,
			int iterationsCount,
			bool isTimeQuantized,
			Action<float, bool> updateFrameAction
		) {
			void UpdateFrame(float d, bool requiredInputSimulation)
			{
				runScheduledActions.Invoke(null, scheduledActionsParameters);
				updateFrameAction(d, false);
				if (!GetRequiredToWaitForWindowRendering() && requiredInputSimulation) {
					Lime.Application.Input.Simulator.OnBetweenFrames(delta);
				}
			}

			if (iterationsCount == 1) {
				var validDelta = Mathf.Clamp(delta, 0, Lime.Application.MaxDelta);
				iterationsCount = validDelta >= Mathf.ZeroTolerance ? (int)(delta / validDelta) : 1;
				var remainDelta = delta - validDelta * iterationsCount;
				for (var i = 0; i < iterationsCount; i++) {
					UpdateFrame(validDelta, i + 1 < iterationsCount || remainDelta > 0);
					if (GetRequiredToWaitForWindowRendering()) {
						break;
					}
				}
				if (remainDelta > 0 && !isTimeQuantized && !GetRequiredToWaitForWindowRendering()) {
					UpdateFrame(remainDelta, false);
				}
			} else {
				for (var i = 0; i < iterationsCount; i++) {
					UpdateFrame(delta, i + 1 < iterationsCount);
					if (GetRequiredToWaitForWindowRendering()) {
						break;
					}
				}
			}

			if (WindowWidget.RenderChainBuilder == null) {
				WindowWidget.AddToRenderChain(worldRenderChain);
			}
		}
	}
}

#endif
