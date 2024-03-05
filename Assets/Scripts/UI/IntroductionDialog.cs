using System;

namespace Assets.Scripts.UI
{
	public class IntroductionDialog : ContinualDialog
	{
		public override void OnLangUpdated(Translation lang)
		{
			if (index >= messages.Length)  // fool proof
				return;

			ContextIntroductionDialogs introductionContext = lang.IntroductionDialogsContext;
			messages = introductionContext.GetMessages();
			nextButtonText.text = introductionContext.nextButtonText;
			ShowCurrentMessage();
		}
	}
}