using System.Collections;
using UnityEngine;
using UnityEngine.UI;


public class PolarisationChart : MonoBehaviour
{
	[SerializeField] private Graph polarisationChart;
	[SerializeField] private Button polarisationPlotButton;

	private void Start()
	{
		if (polarisationPlotButton != null)
			polarisationPlotButton.onClick.AddListener(OnPolarisationPlotButtonPressed);
	}

	public void OnPolarisationPlotButtonPressed()
	{
		GaugeManager gm = GaugeManager.Instance;
		polarisationChart.AddPoint(gm.Intensity, gm.StackVoltage, "Polarisation");
	}

	private void OnDestroy()
	{
		if (polarisationPlotButton != null)
			polarisationPlotButton.onClick.RemoveListener(OnPolarisationPlotButtonPressed);
	}
}
