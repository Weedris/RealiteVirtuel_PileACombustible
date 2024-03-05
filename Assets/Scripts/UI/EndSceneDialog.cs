using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class EndSceneDialog : LangUpdatable
{
	[Tooltip("If set to MainMenu, it considers that it's the last scene and hides the next button")]
	[SerializeField] private ScenesManager.Scene nextScene;
	[SerializeField] private Button nextSceneButton;
	[SerializeField] private Button mainMenuButton;
	[SerializeField] private Button quitButton;

	[Header("Labels")]
	[SerializeField] private TMP_Text messageLabel;
	[SerializeField] private TMP_Text nextLabel;
	[SerializeField] private TMP_Text mainMenuLabel;
	[SerializeField] private TMP_Text quitLabel;

	private bool isLastScene;

	private new void Start()
	{
		base.Start();
		isLastScene = nextScene == ScenesManager.Scene.MAIN_MENU;
		if (isLastScene)
			nextSceneButton.gameObject.SetActive(false);
	}

	private void OnEnable()
	{
		nextSceneButton.onClick.AddListener(() => ScenesManager.Instance.LoadScene(nextScene));
		mainMenuButton.onClick.AddListener(() => ScenesManager.Instance.LoadScene(ScenesManager.Scene.MAIN_MENU));
		quitButton.onClick.AddListener(() => ScenesManager.QuitApplication());
	}

	private void OnDisable()
	{
		nextSceneButton.onClick.RemoveAllListeners();
		mainMenuButton.onClick.RemoveAllListeners();
		quitButton.onClick.RemoveAllListeners();
	}

	public override void OnLangUpdated(Translation lang)
	{
		ContextEndDialog edc = lang.EndDialogContext;
		messageLabel.text = edc.Message;
		nextLabel.text = edc.Next;
		mainMenuLabel.text = edc.MainMenu;
		quitLabel.text = edc.Quit;
	}
}