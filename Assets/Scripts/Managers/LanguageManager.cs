using System;
using System.Collections.Generic;
using UnityEngine;

public class LanguageManager : MonoBehaviour
{
	public static LanguageManager Instance;

	[SerializeField] private Settings settings;
	private HashSet<LangUpdatable> toUpdateOnLangChanged = new();

	private void Awake()
	{
		// singleton
		if (Instance == null) Instance = this;
		else Destroy(gameObject);
	}

	private void Start()
	{
		Invoke(nameof(UpdateLang), 0.2f);
	}

	private void UpdateLang()
	{
		foreach (LangUpdatable element in toUpdateOnLangChanged)
			element.UpdateLang(settings.currentLanguage);
	}

	public void SwitchLanguage(Translation lang)
	{
		settings.currentLanguage = lang;
		UpdateLang();
	}

	public void RegisterUpdatable(LangUpdatable lu)
	{
		toUpdateOnLangChanged.Add(lu);
	}
	public void ForgetUpdatable(LangUpdatable lu)
	{
		toUpdateOnLangChanged.Remove(lu);
	}
}
