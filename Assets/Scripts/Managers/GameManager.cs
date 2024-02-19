using System;
using System.Collections.Generic;
using UnityEngine;

public class GameManager : MonoBehaviour
{
	public static GameManager Instance;
	#region fields
	[SerializeField] private ScreenInstructionsBuilding screenInstruction;
	[SerializeField] private ContinualDialog introductionDialog;

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

	// TODO Remove this shit
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


	private Dictionary<State, Action> stateMethodMap = new();

	public GameManager()
	{
		stateMethodMap[State.Stack] = Stack;
		stateMethodMap[State.BombonneH2] = BombonneH2;
		stateMethodMap[State.Compresseur] = Compresseur;
		stateMethodMap[State.BombonneN2] = BombonneN2;
		stateMethodMap[State.Ventilateur] = Ventilateur;
		stateMethodMap[State.CollecteurEau] = CollecteurEau;
		stateMethodMap[State.Radiateur] = Radiateur;
		stateMethodMap[State.End] = End;
	}

	private void Awake()
	{
		// Singleton
		if (Instance == null) Instance = this;
		else Destroy(gameObject);
	}

	public void Start()
	{
		state = State.Initilazing;

		// Choose a random BGM
		SoundManager.Instance.PlayBGM((BgmType)UnityEngine.Random.Range(0, 2));

		// change state after instructions (start game)
		introductionDialog.OnDialogEnd += NextState;
	}

	public void ExecuteState(State state)
	{
		if (stateMethodMap.TryGetValue(state, out var method))
			method();
	}

	public void NextState()
	{
		state += 1;
		traceParser.traceMainStep(state);
		screenInstruction.NextInstruction();
		ExecuteState(state);
	}

	#region montage
	public void Stack()
	{
		Pac.SetActive(true);
		Pac.GetComponent<ShowElement>().TuyauMetal.SetActive(true);
		debutConstruction = Time.realtimeSinceStartup;
	}

	public void BombonneH2()
	{
		Pac.GetComponent<ShowElement>().H2_In.gameObject.SetActive(true);
	}

	public void Compresseur()
	{
		Pac.GetComponent<ShowElement>().O2_In.SetActive(true);
	}

	public void BombonneN2()
	{
		Pac.GetComponent<ShowElement>().N2_In.SetActive(true);
	}

	public void Ventilateur()
	{
		Pac.GetComponent<ShowElement>().ventilloCapteur.SetActive(true);
	}

	public void CollecteurEau()
	{
		Pac.GetComponent<ShowElement>().H2O_Out.SetActive(true);
	}

	public void Radiateur()
	{
		Pac.GetComponent<ShowElement>().Refroidissement.SetActive(true);
	}
	#endregion montage

	public void End()
	{
		TempsConstruction = Time.realtimeSinceStartup - debutConstruction;

		// enclose PEMFC with glass
		Pac.GetComponent<ShowElement>().Vitre.SetActive(true);

		// victory effects (SFX + VFX)
		SoundManager.Instance.PlaySFX(SfxType.endAssembly);

		traceParser.save();
	}

	#region utilities
	public void getMainComponentToTheirPlace()
	{
		GameObject[] gameObjects = GameObject.FindGameObjectsWithTag("mainComponent");
		foreach (GameObject go in gameObjects)
			go.GetComponent<comeback>().returnToPosInit();
	}
	#endregion
}
