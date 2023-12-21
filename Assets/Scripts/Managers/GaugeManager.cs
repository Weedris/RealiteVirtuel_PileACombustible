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
	public const float XH2 = LAMBDA_H2 / (LAMBDA_H2 + 1); // % reactive gaz injected inside the anode
	public const float N2PerStop = 11; // L (<~> 200 stop with 2.1 m^3)

	public int NbCells { get; private set; } // nb cells in stack
	public float Intensity { get; private set; } // A
	public float Resistance { get; private set; } // R
	public float StackVoltage { get; private set; } // V

	public float Power { get; private set; } // W
	public float ThermalLoss { get; private set; } // W
	public float Efficiency { get; private set; } // %

	public float DihydrogenFlow { get; private set; } // L/min
	public float AirFlow { get; private set; } // L/min
	public float WaterProduction { get; private set; } // Kg/h <=> L/h
	internal float Hydrogen { get => _hydrogenLevelSlider.value; } // A
	#endregion

	private void Awake()
	{
		// make sure it's a singleton
		if (Instance == null) Instance = this;
		else Destroy(gameObject);
	}

	private void Start()
	{
		_intensitySlider.onValueChanged.AddListener(delegate { UpdateValues(); });
		_resistanceSlider.onValueChanged.AddListener(delegate { UpdateValues(); });
		_nbCellsSlider.onValueChanged.AddListener(delegate { UpdateValues(); });
	}

	public void UpdateOutSliders()
	{
		_hydrogenLevelSlider.value = Mathf.Max(0, _hydrogenLevelSlider.value - Efficiency / 100);
		
		_waterLevelSlider.value += WaterProduction / 60;
	}

	internal void ResetValues(float hydrogen)
	{
		_hydrogenLevelSlider.value = hydrogen;
		_intensitySlider.value = _intensitySlider.minValue;
		UpdateValues();
	}

	private void UpdateValues()
	{
		//Variable values
		NbCells = Mathf.RoundToInt(_nbCellsSlider.value);
		Intensity = _intensitySlider.value * (NbCells / _nbCellsSlider.maxValue);
		Resistance = _resistanceSlider.value; // 0.2412 to 0.8499 // (float)(26.641 * Math.Pow(Intensity, -1.15));  // approximation from given data

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
	}
}
