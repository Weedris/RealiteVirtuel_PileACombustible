using UnityEngine;

public class LanguageManager : MonoBehaviour
{
	public static LanguageManager Instance;

	[SerializeField] private Settings settings;
	[SerializeField] private Translation translations;
	[SerializeField] private GameObject[] toUpdateOnLangChanged;

	private void Awake()
	{
		// singleton
		if (Instance == null) Instance = this;
		else Destroy(gameObject);
	}

	private void Start()
	{
		Invoke(nameof(UpdateUI), 0.2f);
	}

	public void SwitchLanguage(Language lang)
	{
		settings.curentLanguage = lang;
		UpdateUI();
	}

	private void UpdateUI()
	{
		LanguageRef translation = translations.refs[(int)settings.curentLanguage];
		foreach (GameObject element in toUpdateOnLangChanged)
		{
			element
				.GetComponent<ILangUpdatable>()
				.UpdateLang(translation);
		}
	}
}
