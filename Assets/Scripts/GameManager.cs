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

using TMPro;
using Unity.XR.CoreUtils;
using UnityEngine;

public class GameManager : MonoBehaviour
{
	public GameObject particle1;
	public GameObject particle2;


	public TextMeshProUGUI text;


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
            case State.Radiateur: Radiateur(); break;
            case State.Pilotage: Pilotage();break;
            case State.End: End();break;
            default: Debug.Log("Shouldn't happen");break;
        }
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
        text.text = lang.welcome;
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
		LoadLangIntoDisplay(Instruction, lang.welcome, lang.warning, lang.montage);
    }

    #endregion

    #region montage
    public void Stack()
    {
        Vector3 coord = new Vector3(-0.0900000036f, 1f, 1.13f);
        Quaternion quat = new Quaternion(0.109381668f, 0.875426114f, -0.408217877f, 0.234569758f);
        Pac = Instantiate(PAC_prefab, coord, quat);
        Pac.GetComponent<ShowElement>().TuyauMetal.gameObject.SetActive(true);
        text.text = language.GiveCorrectlanguage().stack;
    }

    public void BombonneH2()
    {
        Pac.GetComponent<ShowElement>().H2_In.gameObject.SetActive(true);
        text.text = language.GiveCorrectlanguage().H2;
    }

    public void Compresseur()
    {
        Pac.GetComponent<ShowElement>().O2_In.SetActive(true);
        text.text = language.GiveCorrectlanguage().compresseur;
    }

    public void Humidificateur()
    {
        text.text = language.GiveCorrectlanguage().humidificateur;
    }

    public void BombonneN2()
    {
        Pac.GetComponent<ShowElement>().N2_In.SetActive(true);
        text.text = language.GiveCorrectlanguage().N2;
    }

    public void Ventilateur()
    {
        Pac.GetComponent<ShowElement>().ventilloCapteur.SetActive(true);
        text.text = language.GiveCorrectlanguage().ventilateur;
    }

    public void CollecteurEau()
    {
        Pac.GetComponent<ShowElement>().H2O_Out.SetActive(true);
        text.text = language.GiveCorrectlanguage().H2O;
    }

    public void Radiateur()
    {
        Pac.GetComponent<ShowElement>().Refroidissement.SetActive(true);
        text.text = language.GiveCorrectlanguage().radiateur;
    }
    #endregion

    public void Pilotage()
    {
        var lang = language.GiveCorrectlanguage();
        var textGameObject = endButton.GetNamedChild("Texte");
        var next = endButton.GetNamedChild("Suivant");

        Pac.GetComponent<ShowElement>().Vitre.SetActive(true);
        sliderIntensite.SetActive(true);
        text.text = lang.pilotage;
        endButton.SetActive(true);

        GetHeader(textGameObject).text = lang.endtitle;
        GetModalText(textGameObject).text = lang.endtext;
        GetTextTMP(next).text = lang.endbutton;
    }

    public void End()
    {
        var lang = language.GiveCorrectlanguage();
        text.text = lang.endtext + " !";
        particle1.SetActive(true);
        particle2.SetActive(true);
        saveInCSV.sauvegarde();
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
