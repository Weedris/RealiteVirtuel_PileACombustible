using System;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class GaugeManager : MonoBehaviour
{
	public static GaugeManager Instance;

	[SerializeField] private Slider _intensitySlider;
    [SerializeField] private Slider _resistanceSlider;
    [SerializeField] private Slider _nbCellsSlider;
    [SerializeField] private Slider _waterLevelSlider;
    [SerializeField] private Slider _hydrogenLevelSlider;
    [SerializeField] private TextMeshProUGUI _powerText;

    #region values
    public const float LAMBDA_H2 = 1.07f; // surstoichiometry coef of H2 (L/min)
	public const float LAMBDA_O2 = 2f; // stoichiometry coef of O2 (L/min)
	public const float XH2 = LAMBDA_H2 / (LAMBDA_H2 + 1); // % reactive gaz injected inside the anode
	public const float N2PerStop = 11; // L (<~> 200 stop with 2.1 m^3)
	
	public int nbCells { get; private set; } // nb cells in stack
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
        _hydrogenLevelSlider.GetComponentsInChildren<TextMeshProUGUI>()[0].text = _hydrogenLevelSlider.value.ToString("F2");
        _waterLevelSlider.value += WaterProduction / 60;
        _waterLevelSlider.GetComponentsInChildren<TextMeshProUGUI>()[0].text = _waterLevelSlider.value.ToString("F2");
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
        nbCells = Mathf.RoundToInt(_nbCellsSlider.value);
        Intensity = _intensitySlider.value * (nbCells/_nbCellsSlider.maxValue);
		Resistance = _resistanceSlider.value; // 0.2412 to 0.8499 // (float)(26.641 * Math.Pow(Intensity, -1.15));  // approximation from given data

        // U = RI
		StackVoltage = Resistance * Intensity;

		// W
		Power = StackVoltage * Intensity;
		ThermalLoss = Intensity * (nbCells * 1.48f - StackVoltage);
		Efficiency = Power / (Power + ThermalLoss);

		// L/time
		DihydrogenFlow = LAMBDA_H2 * nbCells * Intensity / (XH2 * 1.4358f);
		AirFlow = LAMBDA_O2 * nbCells * Intensity / 60.3f;
		WaterProduction = Intensity * nbCells / 2978;

        //Texts Update
        _intensitySlider.GetComponentsInChildren<TextMeshProUGUI>()[0].text = _intensitySlider.value.ToString("F2");
        _resistanceSlider.GetComponentsInChildren<TextMeshProUGUI>()[0].text = _resistanceSlider.value.ToString("F2");
        _nbCellsSlider.GetComponentsInChildren<TextMeshProUGUI>()[0].text = nbCells.ToString();
        _powerText.text = Power.ToString();
    }
}
