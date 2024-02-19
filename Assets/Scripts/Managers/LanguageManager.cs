using UnityEngine;

public class LanguageManager : MonoBehaviour
{
	public static LanguageManager Instance;

	[SerializeField] private Translation translations;
	[SerializeField] private Language currentLanguage;
	[SerializeField] private GameObject[] toUpdateOnLangChanged;

	private void Awake()
	{
		if (Instance == null) Instance = this;
		else Destroy(gameObject);
	}

	private void Start()
	{
		UpdateUI();
	}

	public void SwitchLanguage(Language lang)
	{
		if (lang == currentLanguage) return;
		currentLanguage = lang;
		UpdateUI();
	}

	private void UpdateUI()
	{
		foreach (GameObject element in toUpdateOnLangChanged)
		{
			element
				.GetComponent<ILangUpdatable>()
				.UpdateLang(translations.refs[(int)currentLanguage]);
		}
	}
}
