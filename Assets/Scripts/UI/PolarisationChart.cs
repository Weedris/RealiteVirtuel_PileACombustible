using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class PolarisationChart : MonoBehaviour
{
    [Header("Child Elements")]
    [SerializeField] private Image _gridImage;
    [SerializeField] private Transform _pointContainer;
    [SerializeField] private GameObject _pointPrefab;

    [Header("GUI Elements")]
    [SerializeField] private Slider _zoomSlider;
    [SerializeField] private Slider _intensitySlider;
    [SerializeField] private Slider _tensionSlider;
    [SerializeField] private Button _addPointButton;

    public Dictionary<Vector2, GameObject> plots = new();

	private void Start()
	{
        _zoomSlider.onValueChanged.AddListener(delegate { OnSliderValueChanged(); }) ;
        _addPointButton.onClick.AddListener(delegate { OnAddPointButtonPressed(); });
	}


    private void OnAddPointButtonPressed()
    {
        AddPoint(_intensitySlider.value, _tensionSlider.value);
    }

    public void AddPoint(float x, float y)
    {
        Vector2 position = new(x, y);

        GameObject plot = Instantiate(_pointPrefab, _pointContainer);
        plot.transform.localPosition = position;

        plots[position] = plot;
    }

    private void OnSliderValueChanged()
    {
        UpdateZoom(_zoomSlider.value);
    }

	public void UpdateZoom(float zoom)
    {
        float scale = 1 / zoom;
        _gridImage.pixelsPerUnitMultiplier = zoom;
		_pointContainer.localScale = new Vector2(scale, scale);
    }
}
