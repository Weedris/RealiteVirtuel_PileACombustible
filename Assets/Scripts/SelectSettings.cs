using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using static Unity.VisualScripting.Icons;

public class SelectSettings : MonoBehaviour
{
    public List<Language> languages;
    public Languages language;
    public Settings settings;
    public enum Languages
    {
        french,
        portuguese,
        english
    }

    public void SelectLanguage(Languages l)
    {
        language = l;
        settings.curentLanguage = languages[(int)language];
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
