using System.Collections.Generic;
using UnityEngine;

public class LanguageManager : MonoBehaviour
{
   public List<Language> languages;

	public enum Languages
	{
		french,
		portuguese,
		english
	}
	private Languages language = Languages.english;
	public void SelectLanguage(Languages l) 
	{
		language = l;
	}

	// maybe use a scriptableobject instead
	public Language GiveCorrectlanguage()
	{
		return languages[(int)language];
	}

    public void language_french()
    {
        SelectLanguage(Languages.french);
    }

    public void language_english()
    {
        SelectLanguage(Languages.english);

    }

    public void language_portuguese()
    {
        SelectLanguage(Languages.portuguese);
    }
}
