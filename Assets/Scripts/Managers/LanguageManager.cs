using System.Collections.Generic;
using UnityEngine;

public class LanguageManager : MonoBehaviour
{
	public static LanguageManager Instance;

	//[SerializedField] 
	public Settings settings;
	public Language language;
	private void Awake()
	{
		if (Instance == null) Instance = this;
		else Destroy(gameObject);
	}


	private void Start()
	{
		UpdateLanguage();
	}


	public void UpdateLanguage()
	{
		language = settings.curentLanguage;
	}


	// maybe use a scriptableobject instead
	public Language GiveCorrectlanguage()
	{
		return language;
	}

}
