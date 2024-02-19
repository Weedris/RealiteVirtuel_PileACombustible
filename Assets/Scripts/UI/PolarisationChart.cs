using UnityEngine;
using UnityEngine.UI;


public class PolarisationChart : MonoBehaviour
{
	[SerializeField] private Graph graph;
	[SerializeField] private Button polarisationPlotButton;

	private void Start()
	{
		if (polarisationPlotButton != null)
			polarisationPlotButton.onClick.AddListener(OnPolarisationPlotButtonPressed);
	}

	public void OnPolarisationPlotButtonPressed()
	{
		GaugeManager gm = GaugeManager.Instance;
		graph.AddPoint(gm.Intensity, gm.StackVoltage, "Polarisation");
	}

	private void OnDestroy()
	{
		if (polarisationPlotButton != null)
			polarisationPlotButton.onClick.RemoveListener(OnPolarisationPlotButtonPressed);
	}
}
