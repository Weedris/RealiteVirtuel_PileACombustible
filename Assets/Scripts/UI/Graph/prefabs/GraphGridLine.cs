using System;
using UnityEngine;

namespace Assets.Scripts.UI.Graph
{
	[RequireComponent(typeof(LineRenderer))]
	public class GraphGridLine : MonoBehaviour
	{
		private LineRenderer lineRenderer;

		private void RetrieveLR()
		{
			if (lineRenderer == null)
				lineRenderer = GetComponent<LineRenderer>();
		}

		public void SetColor(Color color)
		{
			Gradient gradient = new()
			{
				colorKeys = new GradientColorKey[] { new(color, 0f), new(color, 1f) },
				alphaKeys = new GradientAlphaKey[] { new(1f, 0f), new(1f, 1f) }
			};
			SetColor(gradient);
		}


		public void SetColor(Gradient gradient)
		{
			RetrieveLR();
			lineRenderer.colorGradient = gradient;
		}

		public void SetWidth(float width)
		{
			RetrieveLR();
			lineRenderer.startWidth = width;
		}

		public void UpdateTheme(GraphTheme theme)
		{
			SetWidth(theme.GridLineWidth);
			SetColor(theme.GridLineColor);
			lineRenderer.material = theme.GridLineMaterial;
		}
	}
}