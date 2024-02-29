using System.Text;
using UnityEngine;
using UnityEngine.UI;

public class LanguageLayout : MonoBehaviour
{
	[SerializeField] private Settings settings;
	[SerializeField] private Transform flagContainer;
	[SerializeField] private GameObject flagPrefab;
	[SerializeField] private TranslationRefs translationRefs;
	[SerializeField] private Button previousButton;
	[SerializeField] private Button nextButton;

	private Translation[] translations;
	private int currentIndex;
	private int lastIndex;

	private void Awake()
	{
		translations = translationRefs.Translations;

		lastIndex = translations.Length - 1;
		bool hasCurrentLanguageBeenFound = false;
		foreach (Translation translation in translations)
		{
			Instantiate(flagPrefab, flagContainer).GetComponent<Image>().sprite = translation.FlagSprite;
			if (translation == settings.currentLanguage)
				hasCurrentLanguageBeenFound = true;
			if (!hasCurrentLanguageBeenFound)
				currentIndex++;
		}

		// initialise position depending on currently selected language

		if (currentIndex == 0)
			flagContainer.GetChild(lastIndex).SetAsFirstSibling();
		else for (int i = 0; i < currentIndex - 1; i++)
			flagContainer.GetChild(0).SetAsLastSibling();
	}

	private void SelectLanguage()
	{
		LanguageManager.Instance.SwitchLanguage(translations[currentIndex]);
	}

	private void OnPreviousButtonPressed()
	{
		// basically just moves the flag from last to first position in hierarchie
		// this automatically recenters the correct flag
		// yes i'm lazy af
		flagContainer.GetChild(lastIndex).SetAsFirstSibling();

		// actualise data
		currentIndex--;
		if (currentIndex < 0)
			currentIndex = lastIndex;
		SelectLanguage();
	}

	private void OnNextButtonPressed()
	{
		// just does the opposite of OnPreviousButtonPressed()
		flagContainer.GetChild(0).SetAsLastSibling();

		currentIndex++;
		if (currentIndex > lastIndex)
			currentIndex = 0;
		SelectLanguage();
	}

	private void OnEnable()
	{
		nextButton.onClick.AddListener(OnNextButtonPressed);
		previousButton.onClick.AddListener(OnPreviousButtonPressed);
	}

	private void OnDisable()
	{
		nextButton.onClick.RemoveAllListeners();
		previousButton.onClick.RemoveAllListeners();
	}
}