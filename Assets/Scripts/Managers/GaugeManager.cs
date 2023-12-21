using System;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class GaugeManager : MonoBehaviour
{
	public static GaugeManager Instance;

	[Header("Sliders")]
	[SerializeField] private Slider _intensitySlider;
	[SerializeField] private Slider _resistanceSlider;
	[SerializeField] private Slider _nbCellsSlider;
	[SerializeField] private Slider _waterLevelSlider;
	[SerializeField] private Slider _hydrogenLevelSlider;
	[SerializeField] private Slider _nitrogenLevelSlider;
	[SerializeField] private Slider _voltageLevelSlider;

	[Header("TMP_Text")]
	[SerializeField] private TMP_Text _powerText;

	#region values
	public const float LAMBDA_H2 = 1.07f; // surstoichiometry coef of H2 (L/min)
	public const float LAMBDA_O2 = 2f; // stoichiometry coef of O2 (L/min)
	public const float N2PerStop = 11; // L (<~> 200 stop with 2.1 m^3)

	public float XH2 { get; private set; } // % reactive gaz injected inside the anode
	public int NbCells { get; private set; } // nb cells in stack
	public float Intensity { get; private set; } // A
	public float Resistance { get; private set; } // Ω
	public float StackVoltage { get; private set; } // V

	public float Power { get; private set; } // W
	public float ThermalLoss { get; private set; } // W
	public float Efficiency { get; private set; } // %

	public float DihydrogenFlow { get; private set; } // L/min
	public float AirFlow { get; private set; } // L/min
	public float WaterProduction { get; private set; } // Kg/h <=> L/h
	public float Hydrogen { get; private set; }
	#endregion values

	#region control_variables
	[NonSerialized] public bool isHydrogenConsumed;
	private float lastTimeHydrogenWasConsumed;
	# endregion control_variables

	private void Awake()
	{
		// make sure it's a singleton
		if (Instance == null) Instance = this;
		else Destroy(gameObject);
	}

	private void Start()
	{
		// initialise gauges start value
		_hydrogenLevelSlider.value = _hydrogenLevelSlider.maxValue;
		_waterLevelSlider.value = _waterLevelSlider.minValue;
		_nitrogenLevelSlider.value = _nitrogenLevelSlider.maxValue;

		// add listeners
		_intensitySlider.onValueChanged.AddListener(delegate { UpdateValues(); });
		_resistanceSlider.onValueChanged.AddListener(delegate { UpdateValues(); });
		_nbCellsSlider.onValueChanged.AddListener(delegate { UpdateValues(); });
	}

	public void ConsumeHydrogen()
	{
		// calculate elapsed time
		if (!isHydrogenConsumed)
		{ 
			isHydrogenConsumed = true;
			lastTimeHydrogenWasConsumed = Time.time;
		}
		float now = Time.time;
		float elapsedTime = now - lastTimeHydrogenWasConsumed;  // seconds

		// calculate consumption
		float consumedHydrogen = DihydrogenFlow * elapsedTime / 60f;
		_hydrogenLevelSlider.value -= consumedHydrogen;
		_waterLevelSlider.value += WaterProduction * elapsedTime / 3.6f;
		Hydrogen = _hydrogenLevelSlider.value;

		// actualise time
		lastTimeHydrogenWasConsumed = now;
	}

	internal void ResetValues(float hydrogen)
	{
		_hydrogenLevelSlider.value = hydrogen;
		_intensitySlider.value = _intensitySlider.minValue;
		UpdateValues();
	}

	private void UpdateValues()
	{
		XH2 = _intensitySlider.maxValue / _intensitySlider.value * 100;

		//Variable values
		NbCells = (int) _nbCellsSlider.value;
		Intensity = _intensitySlider.value * (NbCells / _nbCellsSlider.maxValue);
		Resistance = (float) (26.641 * Math.Pow(Intensity, -1.15)); // approximation from given data

		// U = RI
		StackVoltage = Resistance * Intensity;

		// W
		Power = StackVoltage * Intensity;
		ThermalLoss = Intensity * (NbCells * 1.48f - StackVoltage);
		Efficiency = Power / (Power + ThermalLoss);

		// L/time
		DihydrogenFlow = LAMBDA_H2 * NbCells * Intensity / (XH2 * 1.4358f);
		AirFlow = LAMBDA_O2 * NbCells * Intensity / 60.3f;
		WaterProduction = Intensity * NbCells / 2978;

		// update gauges
		_voltageLevelSlider.value = StackVoltage;
		_powerText.text = Power.ToString();

		if (Resistance < _resistanceSlider.minValue)
			_resistanceSlider.minValue = Resistance;
		if (Resistance > _resistanceSlider.maxValue)
			_resistanceSlider.maxValue = Resistance;
		_resistanceSlider.value = Resistance;
	}
}
