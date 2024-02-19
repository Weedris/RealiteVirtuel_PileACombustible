using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class ContinualDialog : MonoBehaviour, ILangUpdatable
{
	public event System.Action OnDialogEnd;
	[SerializeField] private TMP_Text message;
	[SerializeField] private Button nextButton;
	[SerializeField] private TMP_Text nextButtonText;

	private string[] messages;
	private int index = 0;

	private void Start()
	{
		nextButton.onClick.AddListener(NextMessage);
#if UNITY_ANDROID
		// TODO put object in front of player
#endif
	}

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
		{
			OnDialogEnd?.Invoke();
			Destroy(gameObject);
		}
	}

	private void OnDestroy()
	{
		nextButton.onClick.RemoveListener(NextMessage);
	}

	public void UpdateLang(LanguageRef lang)
	{
		ContextIntroductionDialogs introductionContext = lang.introductionDialogsContext;
		messages = introductionContext.GetMessages();
		nextButtonText.text = introductionContext.nextButtonText;
		ShowCurrentMessage();
	}
}
