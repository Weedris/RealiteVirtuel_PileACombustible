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
	#region fields
	public static GameManager Instance;

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

    public GameObject Pac;

	public traceParser traceParser;

	double debutConstruction;
	double TempsConstruction;
    public GameObject exit;

	[Header("Part2 (Pilotage)")]
	[SerializeField] private GameObject endButton;
	[SerializeField] private GameObject screenPart2;
	[SerializeField] private GameObject sliderIntensite;
	[SerializeField] private GameObject zoomSlider;
	#endregion fields

	#region intro
	[Header("Intro")]
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
	#endregion intro

	private void Awake()
	{
		// Singleton
		if (Instance == null) Instance = this;
		else Destroy(gameObject);
	}

	public void Start()
	{
		state = State.Initilazing;
		language.updateLanguage();
		bvn();

        var OuiGameObject = exit.GetNamedChild("Oui");
        var NonGameObject = exit.GetNamedChild("Non");

        var lang = language.GiveCorrectlanguage();
        GetHeader(exit).text = lang.Exit;
        GetModalText(exit).text = lang.ExitMessage;
		GetTextTMP(OuiGameObject).text = lang.oui;
        GetTextTMP(NonGameObject).text = lang.non;

		
		if(UnityEngine.Random.Range(0, 2)==0)
		{ SoundManager.Instance.PlayBGM(BgmType.BGM_Elcto);  }
		else
		{ SoundManager.Instance.PlayBGM(BgmType.BGM_Jazz); }

        Debug.Log(Settings.Instance.get_pass_assembly());
		if(Settings.Instance.get_pass_assembly())
		{
			GameObject socketMainComponent = Pac.GetNamedChild("SocketMainComponent");
            GameObject mainComponents = Pac.GetNamedChild("MainComponents");

            List<GameObject> childListeMainComponent = new List<GameObject>();

            for (int i = 0; i < mainComponents.transform.childCount; i++)
            {
                // Ajoutez chaque enfant à la liste
                Transform enfantTransform = mainComponents.transform.GetChild(i);
                childListeMainComponent.Add(enfantTransform.gameObject);
            }

            foreach (GameObject childTransform in childListeMainComponent)
			{
                GameObject poss = socketMainComponent.GetNamedChild(childTransform.name);
				if (poss != null)
				{
                    childTransform.transform.position = poss.transform.position;
                    childTransform.transform.rotation = poss.transform.rotation;
                    Rigidbody r = childTransform.GetComponent<Rigidbody>();
					r.useGravity = false;
                    r.isKinematic = true;
                }
				else 
				{
                    Debug.LogWarning("GameObject not found in MainComponents: " + childTransform.gameObject.name);
                }
            }

            Pac.SetActive(true);
            Pac.GetComponent<ShowElement>().TuyauMetal.SetActive(true);
            Pac.GetComponent<ShowElement>().H2_In.gameObject.SetActive(true);
            Pac.GetComponent<ShowElement>().O2_In.SetActive(true);
            Pac.GetComponent<ShowElement>().N2_In.SetActive(true);
            Pac.GetComponent<ShowElement>().ventilloCapteur.SetActive(true);
            Pac.GetComponent<ShowElement>().H2O_Out.SetActive(true);
            Pac.GetComponent<ShowElement>().Refroidissement.SetActive(true);
            Pac.GetComponent<ShowElement>().Vitre.SetActive(true);

            Bvn.SetActive(false);
			TempsConstruction = -1;



            lang = language.GiveCorrectlanguage();
            var textGameObject = endButton.GetNamedChild("Texte");
            var next = endButton.GetNamedChild("Suivant");
            endButton.SetActive(true);

            sliderIntensite.SetActive(true);
            sliderIntensite.GetComponentsInChildren<TextMeshProUGUI>()[1].text = lang.intensityBarName;
            BoardTMPGUI.text = lang.pilotage;

            GetHeader(textGameObject).text = lang.endtitle;
            GetModalText(textGameObject).text = lang.endtext;
            GetTextTMP(next).text = lang.endbutton;

			state = State.Pilotage;
        }

    }


	public void NextState()
	{
		state += 1;
		ExecuteState(state);
		traceParser.traceMainStep(state);
	}

	#region intro
	public TMP_Text GetHeader(GameObject textGameObject)
	{
		return textGameObject.GetNamedChild("Header Text").GetComponent<TMP_Text>();
	}

	public TMP_Text GetModalText(GameObject textGameObject)
	{
		return textGameObject.GetNamedChild("Modal Text").GetComponent<TMP_Text>();
	}

	public TMP_Text GetTextTMP(GameObject textGameObject)
	{
		return textGameObject.GetNamedChild("Text (TMP)").GetComponent<TMP_Text>();
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
		Pac.SetActive(true);
        Pac.GetComponent<ShowElement>().TuyauMetal.SetActive(true);
		BoardTMPGUI.text = language.GiveCorrectlanguage().stack;
		debutConstruction = Time.realtimeSinceStartup;
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
		// close PEMFC with glass
        Pac.GetComponent<ShowElement>().Vitre.SetActive(true);
		
		// victory effects (SFX + VFX)
		SoundManager.Instance.PlaySFX(SfxType.endAssembly);
		particle1.SetActive(true);
		Destroy(particle1, 5);

		// display end popup
		// might be removed in fture update
		var lang = language.GiveCorrectlanguage();
		var textGameObject = endButton.GetNamedChild("Texte");
		var next = endButton.GetNamedChild("Suivant");
        endButton.SetActive(true);

		// change screen display
		BoardTMPGUI.gameObject.SetActive(false);
		screenPart2.SetActive(true);

		// show sliders for part2
		sliderIntensite.SetActive(true);
		zoomSlider.SetActive(true);

		// set language for ui elements
		sliderIntensite.GetComponentsInChildren<TMP_Text>()[1].text = lang.intensityBarName;
		BoardTMPGUI.text = lang.pilotage;
		

		GetHeader(textGameObject).text = lang.endtitle;
		GetModalText(textGameObject).text = lang.endtext;
		GetTextTMP(next).text = lang.endbutton;

		TempsConstruction = Time.realtimeSinceStartup - debutConstruction;
	}

	public void End()
	{
		var lang = language.GiveCorrectlanguage();
		BoardTMPGUI.text = lang.endtext + " !";
		particle2.SetActive(true);
		Destroy(particle2, 5);
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
