using Assets.Scripts.PEMFC;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public enum FuelCellComponentType
{
	Stack,
	HydrogenBottle,
	Compressor,
	Humidifier,
	NitrogenBottle,
	Ventilator,
	WaterContainer,
	Radiator
}

/// <summary>
/// The manager responsible of the fuel cell assembly.
/// </summary>
public class AssemblyGameManager : MonoBehaviour
{
	public static AssemblyGameManager Instance { get; private set; }

	#region fields
	[SerializeField] private ScreenInstructionsBuilding screenInstruction;
	[SerializeField] private ContinualDialog introductionDialog;
	[SerializeField] private EndSceneDialog endSceneDialog;

	[Header("Fuel Cell")]
	[SerializeField] private string mainComponentsTag;
	[Tooltip("The array of components ordered by their order of placement (gameloop)")]
	[SerializeField]
	private FuelCellComponentType[] steps = new FuelCellComponentType[]
	{
		FuelCellComponentType.Stack,
		FuelCellComponentType.HydrogenBottle,
		FuelCellComponentType.Compressor,
		FuelCellComponentType.Humidifier,
		FuelCellComponentType.NitrogenBottle,
		FuelCellComponentType.Ventilator,
		FuelCellComponentType.WaterContainer,
		FuelCellComponentType.Radiator
	};
	#endregion fields

	private List<FuelCellComponent> componentsToPlace;
	private int currentStep = 0;
	private int numberErrors;

	private void Awake()
	{
		// singleton
		if (Instance == null) Instance = this;
		else Destroy(gameObject);
	}

	private void Start()
	{
		// choose a random BGM
		SoundManager.Instance.PlayBGM(BgmType.BGM_Jazz);

		// retrieve components to place
		componentsToPlace = GameObject
			.FindGameObjectsWithTag(mainComponentsTag)
			.Select(el => el.GetComponent<FuelCellComponent>())
			.Where(el => el != null)
			.ToList();

		// we don't want the player to do anything funny before (s)he finished reading
		foreach (FuelCellComponent component in componentsToPlace)
			component.gameObject.SetActive(false);

		introductionDialog.OnDialogEnd += StartPlaying;

		DataSaver.Instance.Log("Started Session Assembly");  // marks the session start
	}

	/// <summary>
	/// Called when the Player finished the introduction
	/// </summary>
	private void StartPlaying()
	{
		// players now can interact with objects
		foreach (FuelCellComponent component in componentsToPlace)
			component.gameObject.SetActive(true);

		// don't need the introduction anymore
		Destroy(introductionDialog.gameObject);

		// displays instructions on the screen
		screenInstruction.NextInstruction();

		// log
		DataSaver.Instance.Log("Finished Assembly Introduction");
	}

	public void AttemptPlaceObject(FuelCellComponent component, FuelCellSocket socket)
	{
		// log
		DataSaver.Instance.Log($"Placing [{component.ComponentType}] to [{socket.Target}] on step [{steps[currentStep]}]");

		bool isAwaitedComponent = component.ComponentType == steps[currentStep];
		bool doesSocketCorrepond = component.ComponentType == socket.Target;
		bool isCorrectAnswer = isAwaitedComponent && doesSocketCorrepond;
		if (isCorrectAnswer)
		{
			// correct answer feedback
			SoundManager.Instance.PlaySFX(SfxType.GoodAnswer);
			
			// snap component to it's corresponding socket
			component.Place(socket);

			// Deactivate objects that should be interacted with anymore
			componentsToPlace.Remove(component);
			component.Deactivate();
			socket.Deactivate();

			NextStep();
		}
		else
		{
			// bad answer feedback
			SoundManager.Instance.PlaySFX(SfxType.BadAnswer);

			component.ResetPositionAndRotation();
			numberErrors++;
		}
	}

	public void NextStep()
	{
		currentStep++;
		if (currentStep < steps.Length)
			screenInstruction.NextInstruction();
		else
			OnEnd();
	}

	public void OnEnd()
	{
		DataSaver.Instance.Log($"Session End");

		// victory feedback
		SoundManager.Instance.PlaySFX(SfxType.endAssembly);
		endSceneDialog.gameObject.SetActive(true);
	}

	public void ResetComponentsPositionAndRotation()
	{
		foreach (FuelCellComponent fcmc in componentsToPlace)
			fcmc.ResetPositionAndRotation();
	}
}
