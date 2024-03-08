using UnityEngine;
using UnityEngine.UI;

namespace Assets.Scripts.UI.Graph
{
	[System.Serializable]
	public class GraphReferences
	{
		public RectTransform Graph;
		public RectTransform Legend;

		[Space(10f)]

		public Image XAxis;
		public Image YAxis;
		public RectTransform XLabelContainer;
		public RectTransform YLabelContainer;

		[Space(10f)]

		public RectTransform DrawArea;
		public RectTransform CurveContainer;
		public RectTransform GridLineContainer;
	}
}
