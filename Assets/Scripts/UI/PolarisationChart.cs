using Assets.Scripts.UI.Graph;
using UnityEngine;
using UnityEngine.UI;


[RequireComponent(typeof(Graph))]
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
		PerformanceLabGameManager gm = PerformanceLabGameManager.Instance;
		graph.AddPoint(gm.Intensity, gm.StackVoltage, "Polarisation");
	}

	private void OnDestroy()
	{
		if (polarisationPlotButton != null)
			polarisationPlotButton.onClick.RemoveListener(OnPolarisationPlotButtonPressed);
	}
}
