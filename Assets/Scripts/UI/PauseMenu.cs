/*
 * Make sure the buttons in the UI are in the same order than the enum "Language".
 */

using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;


public class PauseMenu : MonoBehaviour
{
	[SerializeField] private Settings m_Settings;

	[Header("Buttons")]
	[SerializeField] private Button resumeButton;
	[SerializeField] private Button quitButton;

	[Header("Images")]
	[SerializeField] private Image bgmImage;
	[SerializeField] private Image sfxImage;

	[Header("Icons")]
	[SerializeField] private Sprite iconBgmOn;
	[SerializeField] private Sprite iconBgmOff;
	[SerializeField] private Sprite iconSfxOn;
	[SerializeField] private Sprite iconSfxOff;

	[Header("Sound controllers")]
	[SerializeField] private Slider bgmSlider;
	[SerializeField] private Slider sfxSlider;

	private void Start()
	{
		bgmSlider.value = m_Settings.BgmLevel;
		sfxSlider.value = m_Settings.SfxLevel;
	}

	private void OnEnable()
	{
		// add listeners
		bgmSlider.onValueChanged.AddListener(SetBgmLevel);
		sfxSlider.onValueChanged.AddListener(SetSfxLevel);
		resumeButton.onClick.AddListener(() => GameMenuManager.Instance.CloseSpecificMenu(gameObject));
		quitButton.onClick.AddListener(ScenesManager.QuitApplication);
	}

	private void OnDisable()
	{
		// remove listeners
		bgmSlider.onValueChanged.RemoveListener(SetBgmLevel);
		sfxSlider.onValueChanged.RemoveListener(SetSfxLevel);
		resumeButton.onClick.RemoveAllListeners();
		quitButton.onClick.RemoveAllListeners();
	}

	private void SetBgmLevel(float level)
	{
		if (level == 0) bgmImage.sprite = iconBgmOff;
		else bgmImage.sprite = iconBgmOn;
		SoundManager.Instance.ChangeBgmVolume(level);
	}

	private void SetSfxLevel(float level)
	{
		if (level == 0) sfxImage.sprite = iconSfxOff;
		else sfxImage.sprite = iconSfxOn;
		SoundManager.Instance.ChangeSfxVolume(level);
	}

}
