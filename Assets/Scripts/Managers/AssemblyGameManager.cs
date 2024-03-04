using Assets.Scripts.PEMFC;
using System.Collections.Generic;
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
	[SerializeField] private List<FuelCellMainComponent> mainComponents;
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

	private int currentStep = 0;

	private void Awake()
	{
		// singleton
		if (Instance == null)
			Instance = this;  // no DontDestroyOnLoad as it shouldn't be used in other scenes
		else
			Destroy(gameObject);
	}

	private void Start()
	{
		// Choose a random BGM
		SoundManager.Instance.PlayBGM(BgmType.BGM_Jazz);

		// We don't want the player to do anything funny before (s)he finished reading
		foreach (FuelCellMainComponent component in mainComponents)
			component.gameObject.SetActive(false);

		// events
		// change state after instructions (start game)
		introductionDialog.OnDialogEnd += StartPlaying;
		DataSaver.Instance.Log("[Session] Assembly");
	}

	private void StartPlaying()
	{
		// players now can start interacting with objects
		foreach (FuelCellMainComponent component in mainComponents)
			component.gameObject.SetActive(true);

		// don't need the introduction anymore
		Destroy(introductionDialog.gameObject);

		screenInstruction.NextInstruction();
		DataSaver.Instance.Log("[Start] Finished Introduction");
	}

	public void AttemptPlaceObject(FuelCellMainComponent component, FuelCellSocket socket)
	{
		string message = $"Placing [{component.WhoAmI}] to [{socket.Target}] on step [{steps[currentStep]}]";

		bool isAwaitedComponent = component.WhoAmI == steps[currentStep];
		bool doesSocketCorrepond = component.WhoAmI == socket.Target;
		bool isCorrectAnswer = isAwaitedComponent && doesSocketCorrepond;
		if (isCorrectAnswer)
		{
			DataSaver.Instance.Log("[Correct] " + message);
			SoundManager.Instance.PlaySFX(SfxType.GoodAnswer);
			component.Place(socket);

			// destroys every unused components
			// (makes it easier to control events)
			mainComponents.Remove(component);  // so it won't try to reset unused component
			component.Deactivate();
			socket.Deactivate();

			NextStep();
		}
		else
		{
			DataSaver.Instance.Log("[Incorrect] " + message);
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
		DataSaver.Instance.Log($"[End]");
		// victory feedback
		SoundManager.Instance.PlaySFX(SfxType.endAssembly);
		endSceneDialog.gameObject.SetActive(true);
	}

	public void ResetComponentsPositionAndRotation()
	{
		foreach (FuelCellMainComponent fcmc in mainComponents)
			fcmc.ResetPositionAndRotation();
	}
}
