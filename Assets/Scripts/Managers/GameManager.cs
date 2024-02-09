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
	#endregion fields

	#region intro
	[Header("Intro")]
	public GameObject Bvn;
	public GameObject Instruction;
	public GameObject Warning;

	private Dictionary<State, Action> stateMethodMap = new();

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
		language.UpdateLanguage();
		bvn();

		var OuiGameObject = exit.GetNamedChild("Oui");
		var NonGameObject = exit.GetNamedChild("Non");

		var lang = language.GiveCorrectlanguage();
		GetHeader(exit).text = lang.Exit;
		GetModalText(exit).text = lang.ExitMessage;
		GetTextTMP(OuiGameObject).text = lang.oui;
		GetTextTMP(NonGameObject).text = lang.non;

		// Choose a random BGM
		SoundManager.Instance.PlayBGM((BgmType)UnityEngine.Random.Range(0, 2));


		if (Settings.Instance.isPlayerPastAssembly)
		{
			GameObject socketMainComponent = Pac.GetNamedChild("SocketMainComponent");
			GameObject mainComponents = Pac.GetNamedChild("MainComponents");

			List<GameObject> mainComponentChildren = new();

			for (int i = 0; i < mainComponents.transform.childCount; i++)
			{
				// Ajoutez chaque enfant à la liste
				Transform childTransform = mainComponents.transform.GetChild(i);
				mainComponentChildren.Add(childTransform.gameObject);
			}

			foreach (GameObject child in mainComponentChildren)
			{
				GameObject poss = socketMainComponent.GetNamedChild(child.name);
				if (poss != null)
				{
					child.transform.SetPositionAndRotation(poss.transform.position, poss.transform.rotation);
					Rigidbody r = child.GetComponent<Rigidbody>();
					r.useGravity = false;
					r.isKinematic = true;
				}
				else
					Debug.LogWarning("GameObject not found in MainComponents: " + child.name);
			}

			Pac.SetActive(true);
			ShowElement PACElements = Pac.GetComponent<ShowElement>();
			PACElements.TuyauMetal.SetActive(true);
			PACElements.H2_In.SetActive(true);
			PACElements.O2_In.SetActive(true);
			PACElements.N2_In.SetActive(true);
			PACElements.ventilloCapteur.SetActive(true);
			PACElements.H2O_Out.SetActive(true);
			PACElements.Refroidissement.SetActive(true);
			PACElements.Vitre.SetActive(true);

			Bvn.SetActive(false);
			TempsConstruction = -1;

			lang = language.GiveCorrectlanguage();
			var textGameObject = endButton.GetNamedChild("Texte");
			var next = endButton.GetNamedChild("Suivant");
			endButton.SetActive(true);

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
        traceParser.traceMainStep(state);
        ExecuteState(state);
		
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
	#endregion intro

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
	#endregion montage

	public void Pilotage()
	{
		// enclose PEMFC with glass
		Pac.GetComponent<ShowElement>().Vitre.SetActive(true);

		// victory effects (SFX + VFX)
		SoundManager.Instance.PlaySFX(SfxType.endAssembly);
		particle1.SetActive(true);
		Destroy(particle1, 5);

		// display end popup
		// might be removed in future update
		var lang = language.GiveCorrectlanguage();
		var textGameObject = endButton.GetNamedChild("Texte");
		var next = endButton.GetNamedChild("Suivant");
		endButton.SetActive(true);

		// change screen display
		BoardTMPGUI.gameObject.SetActive(false);
		screenPart2.SetActive(true);

		// set language for ui elements
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
