/* I made the mistakes to put everything here, i shouldn't have so there is to much thing here 
 * 
 * In this script there is the language gestion
 * the game state
 * how to pass to the next state
 * the call to replace all the component to their initial place
 * each step to place an element
 * 
 * Annnd that's it but to luch i know but it is whta it is
 * 
 */

using System;
using System.Collections;
using System.Collections.Generic;
using System.Resources;
using System.Text;
using TMPro;
using Unity.XR.CoreUtils;
using UnityEngine;

public class GameManager : MonoBehaviour
{

    public GameObject particle1;
    public GameObject particle2;

    public TextMeshProUGUI text;

    public Language french;
    public Language portuguese;
    public Language english;

    public enum Languages
    {
        french,
        portuguese,
        english
    }

    public Languages language;

    public enum State
    {
        Initilazing,
        Stack,
        BombonneH2,
        Compresseur,
        Humidificateur,
        BombonneN2,
        Ventilateur,
        CollecteurEau,
        Radiateur,
        Pilotage,
        End
    }


    public State state;

    public GameObject PAC_prefab;
    private GameObject Pac;

    public GameObject sliderIntensite;

    public SaveInCSV saveInCSV;
    public traceParser traceParser;

    public GameObject endButton;

    #region intro
    public GameObject Bvn;
    public GameObject Instruction;
    public GameObject Warning;


    #endregion

    public void Start()
    {
        language = Languages.french;
        state = State.Initilazing;
    }

    public void NextState()
    {
        state += 1;
        switch(state)
        {
            case State.Stack: Stack();break;
            case State.BombonneH2: BombonneH2();break;
            case State.Compresseur: Compresseur();break;
            case State.Humidificateur: Humidificateur();break;
            case State.BombonneN2: BombonneN2();break;
            case State.Ventilateur: Ventilateur();break;
            case State.CollecteurEau: CollecteurEau();break;
            case State.Pilotage: Pilotage();break;
            case State.End: End();break;
            default: Debug.Log("Shouldn't happen");break;
        }
        traceParser.traceMainStep(state);
    }

    #region language

    public void language_french()
    {
        language = Languages.french;
    }

    public void language_english()
    {
        language = Languages.english;
    }

    public void language_portuguese()
    {
        language = Languages.portuguese;
    }

    public Language giveCorectlanguage()
    {
        if (language == Languages.french)
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
        }
    }

    #endregion

    #region intro

    public void bvn()
    {
        Bvn.gameObject.GetNamedChild("Texte").gameObject.GetNamedChild("Header Text").gameObject.GetComponent<TextMeshProUGUI>().text 
            = giveCorectlanguage().welcome;
        Bvn.gameObject.GetNamedChild("Texte").gameObject.GetNamedChild("Modal Text").gameObject.GetComponent<TextMeshProUGUI>().text
            = giveCorectlanguage().objectif;
        Bvn.gameObject.GetNamedChild("Suivant").gameObject.GetNamedChild("Text (TMP)").gameObject.GetComponent<TextMeshProUGUI>().text
            = giveCorectlanguage().suivant;

    }

    public void instru()
    {
        Instruction.gameObject.GetNamedChild("Texte").gameObject.GetNamedChild("Header Text").gameObject.GetComponent<TextMeshProUGUI>().text
            = giveCorectlanguage().welcome;
        Instruction.gameObject.GetNamedChild("Texte").gameObject.GetNamedChild("Modal Text").gameObject.GetComponent<TextMeshProUGUI>().text
            = giveCorectlanguage().instruction;
        Instruction.gameObject.GetNamedChild("Suivant").gameObject.GetNamedChild("Text (TMP)").gameObject.GetComponent<TextMeshProUGUI>().text
            = giveCorectlanguage().suivant;
    }

    public void warning()
    {
        Warning.gameObject.GetNamedChild("Texte").gameObject.GetNamedChild("Header Text").gameObject.GetComponent<TextMeshProUGUI>().text
            = giveCorectlanguage().welcome;
        Warning.gameObject.GetNamedChild("Texte").gameObject.GetNamedChild("Modal Text").gameObject.GetComponent<TextMeshProUGUI>().text
            = giveCorectlanguage().warning;
        Warning.gameObject.GetNamedChild("Suivant").gameObject.GetNamedChild("Text (TMP)").gameObject.GetComponent<TextMeshProUGUI>().text
            = giveCorectlanguage().montage;
    }

    #endregion

    #region montage
    public void Stack()
    {
        Vector3 coord = new Vector3(-0.0900000036f, 1f, 1.13f);
        Quaternion quat = new Quaternion(0.109381668f, 0.875426114f, -0.408217877f, 0.234569758f);
        Pac = Instantiate(PAC_prefab, coord, quat);

        Pac.GetComponent<ShowElement>().TuyauMetal.gameObject.SetActive(true);

        text.text = giveCorectlanguage().stack;


    }

    public void BombonneH2()
    {
        Pac.GetComponent<ShowElement>().H2_In.gameObject.SetActive(true);

        text.text = giveCorectlanguage().H2;

    }

    public void Compresseur()
    {
        Pac.GetComponent<ShowElement>().O2_In.SetActive(true);

        text.text = giveCorectlanguage().compresseur;
    }

    public void Humidificateur()
    {

        text.text = giveCorectlanguage().humidificateur;

    }

    public void BombonneN2()
    {
        Pac.GetComponent<ShowElement>().N2_In.SetActive(true);

        text.text = giveCorectlanguage().N2;
    }

    public void Ventilateur()
    {
        Pac.GetComponent<ShowElement>().ventilloCapteur.SetActive(true);
        text.text = giveCorectlanguage().ventilateur;
    }

    public void CollecteurEau()
    {
        Pac.GetComponent<ShowElement>().H2O_Out.SetActive(true);

        text.text = giveCorectlanguage().H2O;
    }

    public void Radiateur()
    {
        Pac.GetComponent<ShowElement>().Refroidissement.SetActive(true);

        text.text = giveCorectlanguage().radiateur;
    }

    #endregion

    public void Pilotage()
    {

        Pac.GetComponent<ShowElement>().Vitre.SetActive(true);

        sliderIntensite.SetActive(true);

        text.text = giveCorectlanguage().pilotage;

        endButton.SetActive(true);

        endButton.gameObject.GetNamedChild("Texte").gameObject.GetNamedChild("Header Text").gameObject.GetComponent<TextMeshProUGUI>().text
            = giveCorectlanguage().endtitle;
        endButton.gameObject.GetNamedChild("Texte").gameObject.GetNamedChild("Modal Text").gameObject.GetComponent<TextMeshProUGUI>().text
            = giveCorectlanguage().endtext;
        endButton.gameObject.GetNamedChild("Suivant").gameObject.GetNamedChild("Text (TMP)").gameObject.GetComponent<TextMeshProUGUI>().text
            = giveCorectlanguage().endbutton;

    }

    public void End()
    {
        text.text = giveCorectlanguage().endtext + " !";

        particle1.SetActive(true);
        particle2.SetActive(true);

        saveInCSV.sauvegarde();

    }


    #region utilities

    public void getMainComponentToTherePlace()
    {
        GameObject[] gameObjects = GameObject.FindGameObjectsWithTag("mainComponent");

        foreach (GameObject go in gameObjects)
        {
            go.GetComponent<comeback>().returnToPosInit();
        }
    }

    #endregion

}
