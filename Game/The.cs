using System.Threading;
using Game.Application;
using Game.Dialogs;
using Game.Network;
using Lime;

namespace Game;

public static class The
{
	public static CanvasManager CanvasManager => CanvasManager.Instance;
	public static Client Client => Client.Instance;
	public static Application.Application App => Application.Application.Instance;
	public static WindowWidget World => App.World;
	public static IWindow Window => World.Window;
	public static SoundManager SoundManager => SoundManager.Instance;
	public static AppData AppData => AppData.Instance;
	public static Profile Profile => Profile.Instance;
	public static DialogManager DialogManager => DialogManager.Instance;
	public static Logger Log => Logger.Instance;
	public static Persistence Persistence => persistence.Value;
	private static readonly ThreadLocal<Persistence> persistence = new ThreadLocal<Persistence>(() => new Persistence());
}
