using System;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class ContinualDialog : MonoBehaviour, ILangUpdatable, IEndable
{
	public event Action OnDialogEnd;
	[SerializeField] private TMP_Text message;
	[SerializeField] private Button nextButton;
	[SerializeField] private TMP_Text nextButtonText;

	private string[] messages;
	private int index = 0;

	private void ShowCurrentMessage()
	{
		message.text = messages[index];
	}

	private void NextMessage()
	{
		index++;
		if (index < messages.Length)
			ShowCurrentMessage();
		else
			OnDialogEnd?.Invoke();
	}

	private void OnEnable()
	{
		nextButton.onClick.AddListener(NextMessage);
	}

	private void OnDisable()
	{
		nextButton.onClick.RemoveListener(NextMessage);
		foreach (Delegate d in OnDialogEnd.GetInvocationList())
			OnDialogEnd -= (Action)d;
	}

	public void UpdateLang(LanguageRef lang)
	{
		ContextIntroductionDialogs introductionContext = lang.IntroductionDialogsContext;
		messages = introductionContext.GetMessages();
		nextButtonText.text = introductionContext.nextButtonText;
		ShowCurrentMessage();
	}
}
