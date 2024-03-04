using System.Collections.Generic;
using UnityEngine;

public class LanguageManager : MonoBehaviour
{
	private static LanguageManager _instance;
	public static LanguageManager Instance
	{
		get
		{
			if (_instance == null)
			{
				_instance = FindObjectOfType<LanguageManager>();
				if (_instance == null)
				{
					_instance = new LanguageManager();
					_instance.gameObject.name = "LanguageManager";
				}
			}
			return _instance;
		}
	}

	[SerializeField] private Settings settings;
	private HashSet<LangUpdatable> toUpdateOnLangChanged = new();

	private void Awake()
	{
		// singleton
		if (_instance != null && _instance != this)
		{
			Destroy(gameObject);
			return;
		}
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
		lu.UpdateLang(settings.currentLanguage);
	}
	public void ForgetUpdatable(LangUpdatable lu)
	{
		toUpdateOnLangChanged.Remove(lu);
	}
}
