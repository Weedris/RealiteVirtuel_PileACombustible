using System;
using System.Collections.Generic;
using System.Text;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class PolarisationChart : MonoBehaviour
{
	[Header("Child Elements")]
	[SerializeField] private Image _gridImage;
	[SerializeField] private Transform _pointContainer;
	[SerializeField] private GameObject _pointPrefab;
	[SerializeField] private GameObject _pointContextMenu;

	[Header("GUI Elements")]
	[SerializeField] private Slider _zoomSlider; // recommanded - min: 0.5, max: 3
	[SerializeField] private Slider _intensitySlider;
	[SerializeField] private Button _addPointButton;

	private Vector2 selectedPoint;
	public Dictionary<Vector2, GameObject> plots = new();

	private const float LAMBDA_H2 = 1.7f; // L/min surstoichiometry coef of dihydrogen
	private const float LAMBDA_O2 = 2f; // L/min stoichiometry coef of dioxygen
	private float _resistance;      // R
	private float _intensity;       // A
	private float _stackVoltage;    // V
	private float _power;           // W
	private float _thermalLoss;     // W
	private float _waterProduction; // Kg/h <=> L/h
	private float _dihydrogenFlow;  // L/min
	private float _airFlow;         // L/min
	private float _XH2;             // % reactive gaz injected inside the anode
	private float _efficiency;      // % = _power / (_power + _thermalLoss)

	public int   nbCells        = 24; // nb cells in the stack
	public float consoN2PerStop = 11; // L (<~> 200 stop with 2.1 m^3)

	private void Start()
	{
		_zoomSlider.onValueChanged.AddListener(delegate { OnZoomValueChanged(); });
		_intensitySlider.onValueChanged.AddListener(delegate { UpdateValues(); });
		_addPointButton.onClick.AddListener(delegate { OnAddPointButtonPressed(); });
	}

	#region add_point
	private void OnAddPointButtonPressed()
	{
		AddPoint(_intensity, _stackVoltage);
	}

	public void AddPoint(float x, float y)
	{
		Vector2 position = new(x, y);

		GameObject plot = Instantiate(_pointPrefab, _pointContainer);
		plot.transform.localPosition = position;
		plot.GetComponent<Button>().onClick.AddListener(delegate
		{
			selectedPoint = position;
			ShowContextMenu();
		});

		plots[position] = plot;
	}
	#endregion add_point

	public void ShowContextMenu()
	{
		_pointContextMenu.transform.localPosition = selectedPoint;
		_pointContextMenu.SetActive(true);
	}

	#region remove_point
	public void OnContextMenuDeletePresssed()
	{
		_pointContextMenu.SetActive(false);
		RemovePoint(selectedPoint);
	}

	public void RemovePoint(Vector2 pointPosition)
	{
		Destroy(plots[pointPosition]);
		plots.Remove(pointPosition);
	}
	#endregion remove_point

	private void OnZoomValueChanged()
	{
		float zoom = _zoomSlider.value;
		float scale = 1 / zoom;
		_zoomSlider.transform.Find("ValueLabel").GetComponent<TMP_Text>().text = $"x{zoom}";
		_gridImage.pixelsPerUnitMultiplier = zoom;
		_pointContainer.localScale = new Vector2(scale, scale);
	}

	private void UpdateValues()
	{
		_intensity = _intensitySlider.value;
		_resistance = (float) (26.641 * Math.Pow(_intensity, -1.15));  // approximation from given data
		_stackVoltage = _resistance * _intensity;

		_power = _stackVoltage * _intensity;
		_thermalLoss = _intensity * (nbCells * 1.48f - _stackVoltage);
		_efficiency = _power / (_power + _thermalLoss);

		_XH2 = LAMBDA_H2 / (LAMBDA_H2 + 1);
		_dihydrogenFlow = LAMBDA_H2 * nbCells * _intensity / (_XH2 * 1.4358f);
		_airFlow = LAMBDA_O2 * nbCells * _intensity / 60.3f;

		_waterProduction = _intensity * nbCells / 2978;
	}

	public Tuple<int, Dictionary<string, float>> GetData()
	{
		return new(nbCells, new()
		{
			["intensity"] = _intensity,
			["voltage"] = _stackVoltage,
			["airFlow"] = _airFlow,
			["resistance"] = _resistance,
			["dihydrogenFlow"] = _dihydrogenFlow,
			["waterProduction"] = _waterProduction,
			["thermalLoss"] = _thermalLoss,
			["power"] = _power,
			["efficiency"] = _efficiency
		});
	}

	public string GetCSVStringFormat()
	{
		StringBuilder csvString = new();

		csvString.AppendLine("X,Y");
		foreach (Vector2 position in plots.Keys)
			csvString.AppendLine($"{position.x},{position.y}");

		return csvString.ToString();
	}
}
