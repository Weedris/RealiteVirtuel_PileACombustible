using System;
using System.Collections.Generic;
using TMPro;
using Unity.XR.CoreUtils;
using UnityEngine;


public class SelectSettings : MonoBehaviour
{
	public List<Language> languages;
	public Languages language;
	public Settings settings;

	public GameObject menu;
	public enum Languages
	{
		french,
		portuguese,
		english
	}

    public TextMeshProUGUI GetHeader(GameObject textGameObject)
    {
        return textGameObject.GetNamedChild("Header Text").GetComponent<TextMeshProUGUI>();
    }

    public TextMeshProUGUI GetModalText(GameObject textGameObject)
    {
        return textGameObject.GetNamedChild("Modal Text").GetComponent<TextMeshProUGUI>();
    }
    public void SelectLanguage(Languages l)
	{
		language = l;
		settings.curentLanguage = languages[(int)language];


		GetHeader(menu).text=settings.curentLanguage.welcome;

		String text="";
		foreach( string s in settings.curentLanguage.MenuPrincipale)
		{
			

            text += s; text += '\n'; }
       GetModalText(menu).text= text;

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
