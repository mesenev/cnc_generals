// This file is automatically generated by Kumquat.
// Do not modify this file - YOUR CHANGES WILL BE ERASED!

using Lime;

namespace Game.Scenes.Data;
public class GameScreen : GameScreen<Lime.Frame>
{
	public const string AssetPath = "Shell/GameScreen";

	public static readonly Lime.Frame FrameCache = (Lime.Frame)Node.Load(AssetPath);

	public GameScreen() : base(FrameCache.Clone<Node>()) { }

	public GameScreen(Node node) : base(node) { }

	public new GameScreen Clone()
	{
		return new GameScreen(It.Clone<Node>());
	}
}

public class GameScreen<T> : ParsedNode where T : Node
{
	public T It => (T)Node;


	public class WaitingText : ParsedNode
	{
		public Lime.SimpleText It => (Lime.SimpleText)Node;

		public WaitingText(Node node)
		{
			Node = node;
		}

		public WaitingText Clone()
		{
			return new WaitingText(It.Clone<Node>());
		}
	}

	public readonly WaitingText @_WaitingText;

	public GameScreen() : this(GameScreen.FrameCache.Clone<Node>()) { }
	public GameScreen(Node node)
	{
		Node = node;
		@_WaitingText = new WaitingText(Node.Find<Node>("@WaitingText"));
	}

	public GameScreen<T> Clone()
	{
		return new GameScreen<T>(It.Clone<Node>());
	}

	public T RunAnimationInit()
	{
		Node.RunAnimation("Init");
		return (T)Node;
	}
	public T RunAnimationJumpToInit()
	{
		Node.RunAnimation("JumpToInit");
		return (T)Node;
	}
}
