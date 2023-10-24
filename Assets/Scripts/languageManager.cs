using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
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
    public Languages language;
    public void SelecttLanguage(Languages l) 
    {
        language = l;
    }

    // maybe use a scriptableobject instead
    public Language GiveCorectlanguage()
    {
        return languages[(int)language];
    }
}
