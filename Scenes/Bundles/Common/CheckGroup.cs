using Lime;

namespace Game.Scenes.Common;

public class CheckGroup : ParsedNode
{
	public Lime.Frame It => (Lime.Frame)Node;


	public class Check : ParsedNode
	{
		public Lime.Frame It => (Lime.Frame)Node;

		public Check(Node node)
		{
			Node = node;
		}

		public Check Clone()
		{
			return new Check(It.Clone<Node>());
		}

		public Lime.Frame RunAnimationCheck()
		{
			Node.RunAnimation("Check");
			return (Lime.Frame)Node;
		}
		public Lime.Frame RunAnimationChecked()
		{
			Node.RunAnimation("Checked");
			return (Lime.Frame)Node;
		}
		public Lime.Frame RunAnimationUncheck()
		{
			Node.RunAnimation("Uncheck");
			return (Lime.Frame)Node;
		}
		public Lime.Frame RunAnimationUnchecked()
		{
			Node.RunAnimation("Unchecked");
			return (Lime.Frame)Node;
		}
	}

	public class BtnCheck : ParsedNode
	{
		public Lime.Button It => (Lime.Button)Node;

		public BtnCheck(Node node)
		{
			Node = node;
		}

		public BtnCheck Clone()
		{
			return new BtnCheck(It.Clone<Node>());
		}

		public Lime.Button RunAnimationInit()
		{
			Node.RunAnimation("Init");
			return (Lime.Button)Node;
		}
		public Lime.Button RunAnimationNormal()
		{
			Node.RunAnimation("Normal");
			return (Lime.Button)Node;
		}
		public Lime.Button RunAnimationFocus()
		{
			Node.RunAnimation("Focus");
			return (Lime.Button)Node;
		}
		public Lime.Button RunAnimationPress()
		{
			Node.RunAnimation("Press");
			return (Lime.Button)Node;
		}
		public Lime.Button RunAnimationRelease()
		{
			Node.RunAnimation("Release");
			return (Lime.Button)Node;
		}
		public Lime.Button RunAnimationDisable()
		{
			Node.RunAnimation("Disable");
			return (Lime.Button)Node;
		}
	}

	public readonly Check @_Check;
	public readonly BtnCheck @_BtnCheck;

	public CheckGroup(Node node)
	{
		Node = node;
		@_Check = new Check(Node.Find<Node>("@Check"));
		@_BtnCheck = new BtnCheck(Node.Find<Node>("@BtnCheck"));
	}

	public CheckGroup Clone()
	{
		return new CheckGroup(It.Clone<Node>());
	}
}
