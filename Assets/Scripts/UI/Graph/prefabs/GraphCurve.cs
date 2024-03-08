using System.Linq;
using UnityEngine;

namespace Assets.Scripts.UI.Graph
{
	[RequireComponent(typeof(LineRenderer))]
	public class GraphCurve : MonoBehaviour
	{
		private LineRenderer lineRenderer;

		private void RetrieveLineRenderer() { if (lineRenderer == null) lineRenderer = GetComponent<LineRenderer>(); }

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
			RetrieveLineRenderer();
			lineRenderer.colorGradient = gradient;
		}

		public void SetWidth(float width)
		{
			RetrieveLineRenderer();
			lineRenderer.widthCurve = new(new Keyframe[] { new(width, 0f) });
		}

		public void SetMaterial(Material mat)
		{
			RetrieveLineRenderer();
			lineRenderer.material = mat;
		}

		public void SetPoints(Vector2[] vectors)
		{
			Vector3[] tfVectors = vectors.Select(v => new Vector3(v.x, v.y)).ToArray();
			SetPoints(tfVectors);
		}

		public void SetPoints(Vector3[] vectors)
		{
			if (vectors.Length < 2)
			{
				lineRenderer.positionCount = 2;
				lineRenderer.SetPositions(new Vector3[] { Vector3.zero, Vector3.zero });
			}
			else
			{
				lineRenderer.positionCount = vectors.Length;
				lineRenderer.SetPositions(vectors);
			}
		}
	}
}