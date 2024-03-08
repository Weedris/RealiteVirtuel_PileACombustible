using UnityEngine;

namespace Assets.Scripts.UI.Graph
{
	[System.Serializable]
	public class GraphSettings
	{
		[Min(0)] public int NbGridHorizontalLines = 4;
		[Min(0)] public int NbGridVerticalLines = 3;

		[Space(10f)]

		public Vector2 DefaultOrigin = new(0, 0);
		public float DefaultMaxX = 100;
		public float DefaultMaxY = 50;

		[Tooltip("If you want the origin to stay at (DefaultMinX, DefaultMinY)")]
		public bool ForceOrigin;
		[Tooltip("If you want the end of the x axis to stay at DefaultMaxX")]
		public bool ForceMaxX = false;
		[Tooltip("If you want the end of the y axis to stay at DefaultMaxY")]
		public bool ForceMaxY = false;
	}
}