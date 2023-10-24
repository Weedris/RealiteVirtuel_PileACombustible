using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class languageManager : MonoBehaviour
{
   public List<Language> languages;

    public enum Languages
    {
        french,
        portuguese,
        english
    }
    public Languages language;
    public void selecttLanguage(Languages l) 
    {
        language = l;
    }

    // maybe use a scriptableobject instead
    public Language giveCorectlanguage()
    {
        /* if (language == Languages.french)
         {
             return french;
         }
         else if (language == Languages.english)
         {
             return english;
         }
         else
         {
             return portuguese;
         }*/
        return languages[(int)language];
    }
}
