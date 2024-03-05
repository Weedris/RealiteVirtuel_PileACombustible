using UnityEngine;

namespace Assets.Scripts.UI
{
	[RequireComponent(typeof(ContinualDialog))]
	public class IntroductionDialog : LangUpdatable
	{
		[SerializeField] private ContinualDialog continualDialog;
		public override void OnLangUpdated(Translation lang)
		{
			ContextIntroductionDialogs introductionContext = lang.IntroductionDialogsContext;
			continualDialog.SetMessages(introductionContext.GetMessages());
			continualDialog.SetNewtButtonText(introductionContext.nextButtonText);
			continualDialog.ReloadCurrentMessage();
		}
	}
}