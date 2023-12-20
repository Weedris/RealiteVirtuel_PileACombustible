using System;
using UnityEngine;
using UnityEngine.UI;

public class GaugeManager : MonoBehaviour
{
	public static GaugeManager Instance;

	[SerializeField] private Slider _intensitySlider;
	[SerializeField] private Slider _hydrogenLevelSlider;
	// todo [SerializeField] private Slider _waterLevel;
	// todo [SerializeField] private Slider nbCells; replace const

	#region values
	public const float LAMBDA_H2 = 1.07f; // surstoichiometry coef of H2 (L/min)
	public const float LAMBDA_O2 = 2f; // stoichiometry coef of O2 (L/min)
	public const float XH2 = LAMBDA_H2 / (LAMBDA_H2 + 1); // % reactive gaz injected inside the anode
	public const float N2PerStop = 11; // L (<~> 200 stop with 2.1 m^3)
	public const int nbCells = 24; // nb cells in stack

	public float Intensity { get; private set; } // A
	public float Resistance { get; private set; } // R
	public float StackVoltage { get; private set; } // V

	public float Power { get; private set; } // W
	public float ThermalLoss { get; private set; } // W
	public float Efficiency { get; private set; } // %

	public float DihydrogenFlow { get; private set; } // L/min
	public float AirFlow { get; private set; } // L/min
	public float WaterProduction { get; private set; } // Kg/h <=> L/h
	#endregion

	private void Awake()
	{
		if (Instance == null) Instance = this;
		else Destroy(gameObject);
	}

	private void Start()
	{
		_intensitySlider.onValueChanged.AddListener(delegate { UpdateValues(); });
	}

	private void UpdateValues()
	{
		// U = RI
		Intensity = _intensitySlider.value;
		Resistance = (float)(26.641 * Math.Pow(Intensity, -1.15));  // approximation from given data
		StackVoltage = Resistance * Intensity;

		// W
		Power = StackVoltage * Intensity;
		ThermalLoss = Intensity * (nbCells * 1.48f - StackVoltage);
		Efficiency = Power / (Power + ThermalLoss);

		// L/min
		DihydrogenFlow = LAMBDA_H2 * nbCells * Intensity / (XH2 * 1.4358f);
		AirFlow = LAMBDA_O2 * nbCells * Intensity / 60.3f;
		WaterProduction = Intensity * nbCells / 2978;
	}
}
