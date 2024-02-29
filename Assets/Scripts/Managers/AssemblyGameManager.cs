using Assets.Scripts.PEMFC;
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
	#region singleton
	private static AssemblyGameManager _instance;
	public static AssemblyGameManager Instance
	{
		get
		{
			if (_instance == null)
			{
				_instance = FindAnyObjectByType<AssemblyGameManager>();
				if (_instance == null)
				{
					GameObject go = new("GameManager");
					_instance = go.AddComponent<AssemblyGameManager>();
				}
			}
			return _instance;
		}
	}
	#endregion singleton

	#region fields
	[SerializeField] private ScreenInstructionsBuilding screenInstruction;
	[SerializeField] private ContinualDialog introductionDialog;
	[SerializeField] private EndSceneDialog endSceneDialog;

	[Header("Fuel Cell")]
	[SerializeField] private FuelCellMainComponent[] mainComponents;
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
		if(_instance == null)
		{
			_instance = this;
		}
		else if (_instance != null && _instance != this)
		{
			Destroy(gameObject);
			return;
		}
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
		introductionDialog.gameObject.SetActive(false);

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
		GameMenuManager.Instance.AddMenu(endSceneDialog.gameObject);
	}

	public void ResetComponentsPositionAndRotation()
	{
		foreach (FuelCellMainComponent fcmc in mainComponents)
			fcmc.ResetPositionAndRotation();
	}
}
