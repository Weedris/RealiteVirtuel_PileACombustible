using Assets.Scripts.PEMFC;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public enum FuelCellComponent
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
	private FuelCellComponent[] steps = new FuelCellComponent[]
	{
		FuelCellComponent.Stack,
		FuelCellComponent.HydrogenBottle,
		FuelCellComponent.Compressor,
		FuelCellComponent.Humidifier,
		FuelCellComponent.NitrogenBottle,
		FuelCellComponent.Ventilator,
		FuelCellComponent.WaterContainer,
		FuelCellComponent.Radiator
	};
	#endregion fields
	private List<FuelCellMainComponent> componentsToPlace;
	private int currentStep = 0;

	private void Awake()
	{
		// singleton
		if (Instance == null) Instance = this;  // don't use DontDestroyOnLoad as it shouldn't be used in other scenes
		else Destroy(gameObject);
	}

	private void Start()
	{
		// Choose a random BGM
		SoundManager.Instance.PlayBGM(BgmType.BGM_Jazz);

		// retrieve components to place
		componentsToPlace = GameObject
			.FindGameObjectsWithTag(mainComponentsTag)
			.Select(el => el.GetComponent<FuelCellMainComponent>())
			.Where(el => el != null)
			.ToList();

		// We don't want the player to do anything funny before (s)he finished reading
		foreach (FuelCellMainComponent component in componentsToPlace)
			component.gameObject.SetActive(false);

		// events
		// change state after instructions (start game)
		introductionDialog.OnDialogEnd += StartPlaying;


		DataSaver.Instance.Log("[INFO] Started Session Assembly");  // marks the session start
	}

	private void StartPlaying()
	{
		// players now can start interacting with objects
		foreach (FuelCellMainComponent component in componentsToPlace)
			component.gameObject.SetActive(true);

		// don't need the introduction anymore
		Destroy(introductionDialog.gameObject);

		screenInstruction.NextInstruction();
		DataSaver.Instance.Log("[INFO] Finished Assembly Introduction");
	}

	public void AttemptPlaceObject(FuelCellMainComponent component, FuelCellSocket socket)
	{
		DataSaver.Instance.Log($"[INFO] Placing [{component.WhoAmI}] to [{socket.Target}] on step [{steps[currentStep]}]");

		bool isAwaitedComponent = component.WhoAmI == steps[currentStep];
		bool doesSocketCorrepond = component.WhoAmI == socket.Target;
		bool isCorrectAnswer = isAwaitedComponent && doesSocketCorrepond;
		if (isCorrectAnswer)
		{
			SoundManager.Instance.PlaySFX(SfxType.GoodAnswer);
			component.Place(socket);

			// destroys every unused components
			// (makes it easier to control events)
			componentsToPlace.Remove(component);
			component.Deactivate();
			socket.Deactivate();
			NextStep();
		}
		else
		{
			SoundManager.Instance.PlaySFX(SfxType.BadAnswer);
			component.ResetPositionAndRotation();
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
		DataSaver.Instance.Log($"[INFO] Session End");
		// victory feedback
		SoundManager.Instance.PlaySFX(SfxType.endAssembly);
		endSceneDialog.gameObject.SetActive(true);
	}

	public void ResetComponentsPositionAndRotation()
	{
		foreach (FuelCellMainComponent fcmc in componentsToPlace)
			fcmc.ResetPositionAndRotation();
	}
}
