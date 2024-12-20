// This file is automatically generated by Kumquat.
// Do not modify this file - YOUR CHANGES WILL BE ERASED!

using Lime;

namespace Game.Scenes.Data;
public class BtnOk : BtnOk<Lime.Frame>
{
	public const string AssetPath = "Shell/Externals/BtnOk";

	public static readonly Lime.Frame FrameCache = (Lime.Frame)Node.Load(AssetPath);

	public BtnOk() : base(FrameCache.Clone<Node>()) { }

	public BtnOk(Node node) : base(node) { }

	public new BtnOk Clone()
	{
		return new BtnOk(It.Clone<Node>());
	}
}

public class BtnOk<T> : ParsedNode where T : Node
{
	public T It => (T)Node;



	public BtnOk() : this(BtnOk.FrameCache.Clone<Node>()) { }

	public BtnOk(Node node)
	{
		Node = node;
	}

	public BtnOk<T> Clone()
	{
		return new BtnOk<T>(It.Clone<Node>());
	}

	public T RunAnimationInit()
	{
		Node.RunAnimation("Init");
		return (T)Node;
	}
	public T RunAnimationNormal()
	{
		Node.RunAnimation("Normal");
		return (T)Node;
	}
	public T RunAnimationFocus()
	{
		Node.RunAnimation("Focus");
		return (T)Node;
	}
	public T RunAnimationPress()
	{
		Node.RunAnimation("Press");
		return (T)Node;
	}
	public T RunAnimationRelease()
	{
		Node.RunAnimation("Release");
		return (T)Node;
	}
	public T RunAnimationDisable()
	{
		Node.RunAnimation("Disable");
		return (T)Node;
	}
}
