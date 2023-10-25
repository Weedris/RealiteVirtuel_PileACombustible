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
using System.Collections.Generic;
using TMPro;
using Unity.XR.CoreUtils;
using UnityEngine;

public class GameManager : MonoBehaviour
{

    public static GameManager instance;

    private void Awake()
    {
        if (instance == null)
        {
            instance = this;
        }
        else
        {
            Destroy(gameObject);
        }
    }



    public GameObject particle1;
	public GameObject particle2;

	public TextMeshProUGUI BoardTMPGUI;

    public LanguageManager language;

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

	public traceParser traceParser;

	public GameObject endButton;

	#region intro
	public GameObject Bvn;
	public GameObject Instruction;
	public GameObject Warning;

    private Dictionary<State, Action> stateMethodMap = new Dictionary<State, Action>();

    public GameManager()
    {
        stateMethodMap[State.Stack] = Stack;
        stateMethodMap[State.BombonneH2] = BombonneH2;
        stateMethodMap[State.Compresseur] = Compresseur;
        stateMethodMap[State.Humidificateur] = Humidificateur;
        stateMethodMap[State.BombonneN2] = BombonneN2;
        stateMethodMap[State.Ventilateur] = Ventilateur;
        stateMethodMap[State.CollecteurEau] = CollecteurEau;
        stateMethodMap[State.Radiateur] = Radiateur;
        stateMethodMap[State.Pilotage] = Pilotage;
        stateMethodMap[State.End] = End;
    }

    public void ExecuteState(State state)
    {
        if (stateMethodMap.TryGetValue(state, out var method))
        {
            method();
        }
    }

    #endregion

    public void Start()
	{
		state = State.Initilazing;
	}


    public void NextState()
    {
        state += 1;
        ExecuteState(state);
        traceParser.traceMainStep(state);
    }

    #region intro
    public TextMeshProUGUI GetHeader(GameObject textGameObject)
    {
        return textGameObject.GetNamedChild("Header Text").GetComponent<TextMeshProUGUI>();
    }

    public TextMeshProUGUI GetModalText(GameObject textGameObject)
    {
        return textGameObject.GetNamedChild("Modal Text").GetComponent<TextMeshProUGUI>();
    }

    public TextMeshProUGUI GetTextTMP(GameObject textGameObject)
    {
        return textGameObject.GetNamedChild("Text (TMP)").GetComponent<TextMeshProUGUI>();
    }

    public void LoadLangIntoDisplay(GameObject go, string header, string body, string nextButtonMessage)
    {
		var textGameObject = go.GetNamedChild("Texte");
		var next = go.GetNamedChild("Suivant");

		GetHeader(textGameObject).text = header;
		GetModalText(textGameObject).text = body;
		GetTextTMP(next).text = nextButtonMessage;
	}

    public void bvn()
    {
        var lang = language.GiveCorrectlanguage();
		BoardTMPGUI.text = lang.welcome;
        LoadLangIntoDisplay(Bvn, lang.welcome, lang.objectif, lang.suivant);
    }

    public void instru()
    {
        var lang = language.GiveCorrectlanguage();
		LoadLangIntoDisplay(Instruction, lang.welcome, lang.instruction, lang.suivant);
    }

    public void warning()
    {
        var lang = language.GiveCorrectlanguage();
        LoadLangIntoDisplay(Warning, lang.welcome, lang.warning, lang.montage);
    }

    #endregion

    #region montage
    public void Stack()
    {
        Vector3 coord = new Vector3(-0.0900000036f, 1f, 1.13f);
        Quaternion quat = new Quaternion(0.109381668f, 0.875426114f, -0.408217877f, 0.234569758f);
        Pac = Instantiate(PAC_prefab, coord, quat);
        Pac.GetComponent<ShowElement>().TuyauMetal.gameObject.SetActive(true);
        BoardTMPGUI.text = language.GiveCorrectlanguage().stack;
    }

    public void BombonneH2()
    {
        Pac.GetComponent<ShowElement>().H2_In.gameObject.SetActive(true);
        BoardTMPGUI.text = language.GiveCorrectlanguage().H2;
    }

    public void Compresseur()
    {
        Pac.GetComponent<ShowElement>().O2_In.SetActive(true);
        BoardTMPGUI.text = language.GiveCorrectlanguage().compresseur;
    }

    public void Humidificateur()
    {
        BoardTMPGUI.text = language.GiveCorrectlanguage().humidificateur;
    }

    public void BombonneN2()
    {
        Pac.GetComponent<ShowElement>().N2_In.SetActive(true);
        BoardTMPGUI.text = language.GiveCorrectlanguage().N2;
    }

    public void Ventilateur()
    {
        Pac.GetComponent<ShowElement>().ventilloCapteur.SetActive(true);
        BoardTMPGUI.text = language.GiveCorrectlanguage().ventilateur;
    }

    public void CollecteurEau()
    {
        Pac.GetComponent<ShowElement>().H2O_Out.SetActive(true);
        BoardTMPGUI.text = language.GiveCorrectlanguage().H2O;
    }

    public void Radiateur()
    {
        Pac.GetComponent<ShowElement>().Refroidissement.SetActive(true);
        BoardTMPGUI.text = language.GiveCorrectlanguage().radiateur;
    }
    #endregion

    public void Pilotage()
    {
        var lang = language.GiveCorrectlanguage();
        var textGameObject = endButton.GetNamedChild("Texte");
        var next = endButton.GetNamedChild("Suivant");

        Pac.GetComponent<ShowElement>().Vitre.SetActive(true);
        sliderIntensite.SetActive(true);
        sliderIntensite.GetComponentsInChildren<TextMeshProUGUI>()[1].text = lang.intensityBarName;
        BoardTMPGUI.text = lang.pilotage;
        endButton.SetActive(true);

        GetHeader(textGameObject).text = lang.endtitle;
        GetModalText(textGameObject).text = lang.endtext;
        GetTextTMP(next).text = lang.endbutton;
    }

    public void End()
    {
        var lang = language.GiveCorrectlanguage();
        BoardTMPGUI.text = lang.endtext + " !";
        particle1.SetActive(true);
        particle2.SetActive(true);
        traceParser.save();
    }

    #region utilities
    public void getMainComponentToTheirPlace()
    {
        GameObject[] gameObjects = GameObject.FindGameObjectsWithTag("mainComponent");

        foreach (GameObject go in gameObjects)
        {
            go.GetComponent<comeback>().returnToPosInit();
        }
    }
    #endregion


}
