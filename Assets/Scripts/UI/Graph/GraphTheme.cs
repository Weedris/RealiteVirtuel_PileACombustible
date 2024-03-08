using TMPro;
using UnityEngine;

namespace Assets.Scripts.UI.Graph
{
	[System.Serializable]
	public class GraphTheme
	{
		public Material AxisMaterial;
		public float AxisWidth = 2f;
		public Color AxisColor = Color.white;

		[Space(10f)]

		public Material GridLineMaterial;
		public float GridLineWidth = 1f;
		public Color GridLineColor = Color.grey;
		public TMP_FontAsset GridLineFont;

		[Space(10f)]

		public Material CurveMaterial;
		public float CurveWidth = 1f;
		public Color[] CurveColors = new Color[] {Color.yellow, Color.green, Color.blue, Color.magenta};

		[Space(10f)]

		public TMP_FontAsset LegendFont;
	}
}
