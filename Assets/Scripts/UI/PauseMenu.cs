/*
 * Make sure the buttons in the UI are in the same order than the enum "Language".
 */

using UnityEngine;
using UnityEngine.UI;


public class PauseMenu : MonoBehaviour
{
	[Header("Icons")]
	[SerializeField] private Sprite iconBgmOn;
	[SerializeField] private Sprite iconBgmOff;
	[SerializeField] private Sprite iconSfxOn;
	[SerializeField] private Sprite iconSfxOff;

	[Header("Sound controllers")]
	[SerializeField] private Slider bgmSlider;
	[SerializeField] private Slider sfxSlider;

	[Header("Language")]
	[SerializeField] private Transform languageButtonsContainer;

	private void OnEnable()
	{
		AddListeners();
	}

	private void OnDisable()
	{
		RemoveListeners();
	}

	private void SelectLanguage(Language lang)
	{
		LanguageManager.Instance.SwitchLanguage(lang);
	}

	private void SetBgmLevel(float level)
	{
		SoundManager.Instance.ChangeBgmVolume(level);
	}

	private void SetSfxLevel(float level)
	{
		SoundManager.Instance.ChangeSfxVolume(level);
	}

	private void AddListeners()
	{
		bgmSlider.onValueChanged.AddListener(SetBgmLevel);
		sfxSlider.onValueChanged.AddListener(SetSfxLevel);
		for (int i = 0; i < languageButtonsContainer.childCount; i++)
		{
			languageButtonsContainer
				.GetChild(i)
				.GetComponent<Button>()
				.onClick
				.AddListener(() => SelectLanguage((Language)i));
		}
	}

	private void RemoveListeners()
	{
		bgmSlider.onValueChanged.RemoveListener(SetBgmLevel);
		sfxSlider.onValueChanged.RemoveListener(SetSfxLevel);
		for (int i = 0; i < languageButtonsContainer.childCount; i++)
		{
			languageButtonsContainer
				.GetChild(i)
				.GetComponent<Button>()
				.onClick
				.RemoveAllListeners();
		}
	}
}
