// This file is automatically generated by Kumquat.
// Do not modify this file - YOUR CHANGES WILL BE ERASED!

using Lime;

namespace Game.Scenes.Data;
public class MainMenu : MainMenu<Lime.Frame>
{
	public const string AssetPath = "Shell/MainMenu";

	public static readonly Lime.Frame FrameCache = (Lime.Frame)Node.Load(AssetPath);

	public MainMenu() : base(FrameCache.Clone<Node>()) { }

	public MainMenu(Node node) : base(node) { }

	public new MainMenu Clone()
	{
		return new MainMenu(It.Clone<Node>());
	}
}

public class MainMenu<T> : ParsedNode where T : Node
{
	public T It => (T)Node;


	public readonly BtnPlay<Lime.Button> @_BtnPlay;
	public readonly BtnOptions<Lime.Button> @_BtnOptions;

	public MainMenu() : this(MainMenu.FrameCache.Clone<Node>()) { }
	public MainMenu(Node node)
	{
		Node = node;
		@_BtnPlay = new BtnPlay<Lime.Button>(Node.Find<Node>("@BtnPlay"));
		@_BtnOptions = new BtnOptions<Lime.Button>(Node.Find<Node>("@BtnOptions"));
	}

	public MainMenu<T> Clone()
	{
		return new MainMenu<T>(It.Clone<Node>());
	}

	public T RunAnimationHide()
	{
		Node.RunAnimation("Hide");
		return (T)Node;
	}
	public T RunAnimationShow()
	{
		Node.RunAnimation("Show");
		return (T)Node;
	}
}