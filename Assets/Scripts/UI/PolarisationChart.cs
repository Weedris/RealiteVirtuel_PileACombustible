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
	[SerializeField] private Button _addPointButton;

	private Vector2 selectedPoint;
	private GaugeManager _gaugeManager;
	public Dictionary<Vector2, GameObject> plots = new();


	private void Start()
	{
		_gaugeManager = GaugeManager.Instance;
		_zoomSlider.onValueChanged.AddListener(delegate { OnZoomValueChanged(); });
		//_addPointButton.onClick.AddListener(delegate { OnAddPointButtonPressed(); });
	}

	#region add_point
	private void OnAddPointButtonPressed()
	{
		AddPoint(_gaugeManager.Intensity, _gaugeManager.StackVoltage);
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

	#region point_settings
	public void ShowContextMenu()
	{
		_pointContextMenu.transform.localPosition = selectedPoint;
		_pointContextMenu.SetActive(true);
	}
	#endregion point_settings

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

	public string GetCSVStringFormat()
	{
		StringBuilder csvString = new();

		csvString.AppendLine("X,Y");
		foreach (Vector2 position in plots.Keys)
			csvString.AppendLine($"{position.x},{position.y}");

		return csvString.ToString();
	}
}