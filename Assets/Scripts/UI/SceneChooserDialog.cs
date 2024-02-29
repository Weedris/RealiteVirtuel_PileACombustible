using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class SceneChooserDialog : LangUpdatable
{
	[Header("Buttons")]
	[SerializeField] private Button nextButton;
	[SerializeField] private Button previousButton;
	[SerializeField] private Button validateButton;

	[Header("Labels")]
	[SerializeField] private TMP_Text header;
	[SerializeField] private TMP_Text currentSessionDescription;
	[SerializeField] private TMP_Text startSessionButtonLabel;

	private Session[] sessions;
	private int index;

	private void OnValidationClicked()
	{
		ScenesManager.Instance.LoadScene(sessions[index].Scene);
	}

	private void NextSession()
	{
		index++;
		if (index >= sessions.Length)
			index = 0;
		UpdateCurrentSessionDescription();
	}

	private void PreviousSession()
	{
		index--;
		if (index < 0)
			index = sessions.Length - 1;
		UpdateCurrentSessionDescription();
	}

	private void OnEnable()
	{
		validateButton.onClick.AddListener(OnValidationClicked);
		nextButton.onClick.AddListener(NextSession);
		previousButton.onClick.AddListener(PreviousSession);
	}

	private void OnDisable()
	{
		validateButton.onClick.RemoveListener(OnValidationClicked);
	}

	private void UpdateCurrentSessionDescription()
	{
		currentSessionDescription.text = sessions[index].Description;
	}

	public override void UpdateLang(Translation lang)
	{
		ContextMainMenu mainMenuContext = lang.MainMenuContext;
		sessions = mainMenuContext.Sessions;
		header.text = mainMenuContext.SessionChooserHeader;
		startSessionButtonLabel.text = mainMenuContext.StartSession;
		UpdateCurrentSessionDescription();
	}
}