using Game.Dialogs;

namespace QualityAssurance
{
	public class WaitDialogTaskParameters : TaskParameters
	{
		public static readonly ConditionDelegate DefaultCondition = dialog => dialog.State == DialogState.Shown && dialog.IsTopDialog;
		public static readonly ConditionDelegate TopOrBackgroundDialogCondition = dialog => dialog.State == DialogState.Shown;

		public new static readonly WaitDialogTaskParameters Default = new WaitDialogTaskParameters();
		public static readonly WaitDialogTaskParameters TopOrBackground = new WaitDialogTaskParameters { Condition = TopOrBackgroundDialogCondition };
		public static readonly WaitDialogTaskParameters WithoutConditions = new WaitDialogTaskParameters { Condition = null };
		public new static readonly WaitDialogTaskParameters Optional = new WaitDialogTaskParameters { IsStrictly = false };
		public new static readonly WaitDialogTaskParameters Immediately = new WaitDialogTaskParameters { Duration = 0 };
		public new static readonly WaitDialogTaskParameters ImmediatelyAndOptional = new WaitDialogTaskParameters {
			IsStrictly = false,
			Duration = 0
		};

		public delegate bool ConditionDelegate(Dialog dialog);
		public ConditionDelegate Condition = DefaultCondition;

		public bool IsConditionMet(Dialog dialog) => Condition?.Invoke(dialog) ?? true;
	}
}
