using System;
using System.Collections.Generic;
using System.Linq;
using TMPro;
using UnityEditor;
using UnityEngine;


namespace Assets.Scripts.UI.Graph
{
	[ExecuteInEditMode]
	public class Graph : MonoBehaviour
	{
		[SerializeField] private GraphPrefabs prefabs;
		[SerializeField] private GraphReferences references;
		[SerializeField] private GraphTheme theme;
		[SerializeField] private GraphSettings settings;

		private List<string> curveNames = new();
		private List<List<Vector2>> plots = new();

		private List<GraphCurve> curves = new();
		private List<LineRenderer> hGridLines = new();
		private List<LineRenderer> vGridLines = new();
		private List<TMP_Text> xLabels = new();
		private List<TMP_Text> yLabels = new();

		private List<GameObject> legendItems = new();

		private Vector2 originValue;
		private float maxXValue;
		private float maxYValue;

		#region init

		private void EmptyTransform(Transform transform)
		{
			while (transform.childCount > 0) DestroyImmediate(transform.GetChild(0).gameObject);
		}

		private void Awake()
		{
			// Destroy already existing component to prevent weird behaviours
			EmptyTransform(references.CurveContainer);
			EmptyTransform(references.XLabelContainer);
			EmptyTransform(references.YLabelContainer);
			EmptyTransform(references.GridLineContainer);
			EmptyTransform(references.Legend);

			// init base values
			originValue = settings.DefaultOrigin;
			maxXValue = settings.DefaultMaxX;
			maxYValue = settings.DefaultMaxY;

			// draw
			RedrawGridLines();
			RedrawGridLineLabels();
			RedrawCurves();
			UpdateTheme();
		}
		#endregion init

		#region utility
		/// <summary>
		/// Retrieve the curve Index from it's name.
		/// </summary>
		/// <param name="curveName">The name of the curve.</param>
		/// <returns>The index of the curve in every list or -1 if not found.</returns>
		private int GetCurveIndexFromName(string curveName)
		{
			return curveNames.IndexOf(curveName);
		}

		/// <summary>
		/// Insert a Vector to a List<Vector> while maintaining the order by X values.
		/// Sorting algo: binary tree.
		/// </summary>
		/// <param name="list">The list you want modified</param>
		/// <param name="newVector">The Vector to Add to the list</param>
		public void InsertVectorToList(List<Vector2> list, Vector2 newVector)
		{
			int left = 0;
			int right = list.Count - 1;

			while (left <= right)
			{
				int mid = left + (right - left) / 2;
				float midValue = list[mid].x;

				if (newVector.x < midValue)
					right = mid - 1;
				else if (newVector.x > midValue)
					left = mid + 1;
				else
				{
					// Handle duplicates (optional: insert before or after)
					list.Insert(mid, newVector);
					return;
				}
			}

			// Insert at the correct position based on search result
			list.Insert(left, newVector);
		}


		public bool DoPointChangeBounds(Vector2 point)
		{
			return originValue.x > point.x || originValue.y > point.y || maxXValue < point.x || maxYValue < point.y;
		}

		public void UpdateBounds(Vector2 point)
		{
			originValue.x = Math.Min(originValue.x, point.x);
			originValue.y = Math.Min(originValue.y, point.y);
			maxXValue = Math.Max(maxXValue, point.x);
			maxYValue = Math.Max(maxYValue, point.y);
		}

		private void RecalculateBoundValues()
		{
			// originX, originY, maxX, maxY
			Vector4 bounds = new(Mathf.Infinity, Mathf.Infinity, -Mathf.Infinity, -Mathf.Infinity);

			int nbMaxCurvePoints = 0;

			// calc bounds
			foreach (List<Vector2> category in plots)
			{
				nbMaxCurvePoints = Math.Max(nbMaxCurvePoints, category.Count);
				foreach (Vector2 point in category)
				{
					if (point.x < bounds.x) bounds.x = point.x;
					if (point.y < bounds.y) bounds.y = point.y;
					if (point.x > bounds.z) bounds.z = point.x;
					if (point.y > bounds.w) bounds.w = point.y;
				}
			}

			bool hasMinimumPoints = nbMaxCurvePoints > 1;

			bool isOriginValid = bounds.x != Mathf.Infinity && bounds.y != Mathf.Infinity;
			bool isMaxXValid = bounds.z != -Mathf.Infinity;
			bool isMaxYValid = bounds.w != -Mathf.Infinity;

			bool resetOrigin = hasMinimumPoints || !isOriginValid || settings.ForceOrigin;
			bool resetMaxX = hasMinimumPoints || !isMaxXValid || settings.ForceMaxX;
			bool resetMaxY = hasMinimumPoints || !isMaxYValid || settings.ForceMaxY;

			originValue = resetOrigin ? settings.DefaultOrigin : new Vector2(bounds.x, bounds.y);
			maxXValue = resetMaxX ? settings.DefaultMaxX : bounds.z;
			maxYValue = resetMaxY ? settings.DefaultMaxY : bounds.w;
		}
		#endregion utility

		#region data_gestion
		/// <summary>
		/// Add a point to a curve. If the curve does not exist, creates it.
		/// </summary>
		/// <param name="x"></param>
		/// <param name="y"></param>
		/// <param name="curveName"></param>
		public void AddPoint(float x, float y, string curveName)
		{
			// retrieve curve
			int curveIndex = GetCurveIndexFromName(curveName);

			// create it if necessary
			if (curveIndex == -1)
			{
				curveIndex = plots.Count;
				AddCurve(curveName);
			}

			//init
			Vector2 point = new(x, y);
			List<Vector2> curvePoints = plots[curveIndex];

			InsertVectorToList(curvePoints, point);

			if (DoPointChangeBounds(point))
			{
				UpdateBounds(point);
				RedrawCurves();
				RedrawGridLineLabels();
			}
			else RedrawCurve(curveIndex);
		}

		/// <summary>
		/// Same as AddPoint but for multiple points instead of one.
		/// </summary>
		/// <param name="points">The list of points you want to add.</param>
		/// <param name="curveName">The curve name.</param>
		public void AddPoints(List<Vector2> points, string curveName)
		{
			// retrieve curve index
			int curveIndex = GetCurveIndexFromName(curveName);
			// create curve if it doesn't exist yet
			if (curveIndex == -1)
			{
				curveIndex = plots.Count;
				AddCurve(curveName);
			}

			List<Vector2> curvePoints = plots[curveIndex];

			// insert all points
			bool redrawAllCurves = false;
			foreach (Vector2 point in points)
			{
				InsertVectorToList(curvePoints, point);
				if (DoPointChangeBounds(point))
				{
					UpdateBounds(point);
					redrawAllCurves = true;
				}
			}

			if (redrawAllCurves)
			{
				RedrawCurves();
				RedrawGridLineLabels();
			}
			else RedrawCurve(curveIndex);
		}

		public void RemovePoint(Vector2 point, string curveName)
		{
			int curveIndex = GetCurveIndexFromName(curveName);
			// ignore non sense
			if (curveIndex == -1) return;

			plots[curveIndex].Remove(point);

			bool pointIsBound = point.x == originValue.x || point.y == originValue.y || point.x == maxXValue || point.y == maxYValue;
			if (pointIsBound)
			{
				RecalculateBoundValues();
				RedrawCurves();
				RedrawGridLineLabels();
			}
			else RedrawCurve(curveIndex);
		}

		public void AddCurve(string name)
		{
			GameObject curveGo = Instantiate(prefabs.Curve, references.CurveContainer);
			GameObject legendGo = Instantiate(prefabs.LegendItem, references.Legend);

			curveGo.name = name;
			legendGo.name = name;

			GraphCurve curve = curveGo.GetComponent<GraphCurve>();

			// set curve theme
			curve.SetMaterial(theme.CurveMaterial);
			curve.SetColor(theme.CurveColors[curveNames.Count]);
			curve.SetWidth(theme.CurveWidth);

			// save ref
			legendItems.Add(legendGo);
			curves.Add(curve);
			curveNames.Add(name);
			plots.Add(new());
		}

		public void RemoveCurve(string curveName)
		{
			// retrieve categoryID
			int curveIndex = GetCurveIndexFromName(curveName);

			// ignore when nothing needs to be deleted
			if (curveIndex == -1) return;

			// destroy UI elements
			DestroyImmediate(legendItems[curveIndex]);
			DestroyImmediate(curves[curveIndex].gameObject);

			// destroy related data
			curveNames.RemoveAt(curveIndex);
			curves.RemoveAt(curveIndex);
			legendItems.RemoveAt(curveIndex);
			plots.RemoveAt(curveIndex);
		}
		#endregion data_gestion

		#region redrawing

		/// <summary>
		/// Regulates labels population.
		/// </summary>
		/// <param name="labelList">The list that remembers the labels.</param>
		/// <param name="container">The container of the labels.</param>
		/// <param name="nbNeeded">The number of labels wanted/needed.</param>
		private void RegulateLabels(List<TMP_Text> labelList, RectTransform container, int nbNeeded)
		{
			// destroy excess
			while (labelList.Count > nbNeeded)
			{
				DestroyImmediate(labelList[0].gameObject);
				labelList.RemoveAt(0);
			}

			// create if needed
			while (container.childCount < nbNeeded)
			{
				GameObject go = Instantiate(prefabs.GridLineLabel, container);
				labelList.Add(go.GetComponent<TMP_Text>());
			}
		}

		private void RedrawGridLineLabels()
		{
			int nbVLines = settings.NbGridVerticalLines;
			int nbHLines = settings.NbGridHorizontalLines;

			float xUnit = (maxXValue - originValue.x) / (nbVLines + 1);
			float yUnit = (maxYValue - originValue.y) / (nbHLines + 1);

			float xAnchorSpace = 1f / (nbVLines + 1);
			float yAnchorSpace = 1f / (nbHLines + 1);

			// regulate label population
			RegulateLabels(xLabels, references.XLabelContainer, nbVLines + 1);
			RegulateLabels(yLabels, references.YLabelContainer, nbHLines + 1);

			// set x labels
			for (int i = 0; i < xLabels.Count; i++)
			{
				// set text
				TMP_Text label = xLabels[i];
				label.text = (originValue.x + xUnit * (i + 1)).ToString("F2");

				// set anchor
				RectTransform labelRect = label.rectTransform;
				labelRect.anchorMin = new(xAnchorSpace * i + xAnchorSpace / 10f, 0f);
				labelRect.anchorMax = new(xAnchorSpace * (i + 1), 1f);
			}

			// set y labels
			for (int i = 0; i < yLabels.Count; i++)
			{
				// set text
				TMP_Text label = yLabels[i];
				label.text = (originValue.y + yUnit * (i + 1)).ToString("F2");

				// set anchor
				RectTransform labelRect = label.rectTransform;
				labelRect.anchorMin = new(0f, yAnchorSpace * i + yAnchorSpace / 10f);
				labelRect.anchorMax = new(1f, yAnchorSpace * (i + 1));
			}
		}

		/// <summary>
		/// Destroy and create grid lines when needed.
		/// </summary>
		/// <param name="direction">'H' or 'V'</param>
		private void RegulateGridLines(List<LineRenderer> lineList, int nbLines, char direction)
		{
			// delete excess
			while (lineList.Count > nbLines)
			{
				DestroyImmediate(lineList[^1].gameObject);
				lineList.RemoveAt(lineList.Count - 1);
			}

			// create if needed
			while (lineList.Count < nbLines)
			{
				GameObject go = Instantiate(prefabs.GridLine, references.GridLineContainer);
				lineList.Add(go.GetComponent<LineRenderer>());

				// rename GameObject
				go.name = "Scale" + direction + lineList.Count;
			}
		}

		public void RedrawGridLines()
		{
			Rect rect = references.GridLineContainer.rect;

			int nbHLines = settings.NbGridHorizontalLines;
			int nbVLines = settings.NbGridVerticalLines;

			float hSpacing = rect.height / (nbHLines + 1);
			float vSpacing = rect.width / (nbVLines + 1);

			RegulateGridLines(hGridLines, nbHLines, 'H');
			RegulateGridLines(vGridLines, nbVLines, 'V');

			// redraw horizontal scales
			for (int i = 0; i < hGridLines.Count; i++)
			{
				Vector3 startPosition = new(0, hSpacing + hSpacing * i);
				Vector3 endPosition = new(rect.width, startPosition.y);
				hGridLines[i].SetPositions(new[] { startPosition, endPosition });
			}

			// redraw vertical grid lines
			for (int i = 0; i < vGridLines.Count; i++)
			{
				Vector3 startPosition = new(vSpacing + vSpacing * i, 0);
				Vector3 endPosition = new(startPosition.x, rect.height);
				vGridLines[i].SetPositions(new[] { startPosition, endPosition });
			}
		}

		public void RedrawCurves()
		{
			for (int i = 0; i < plots.Count; i++)
				RedrawCurve(i);
		}

		/// <summary>
		/// Redraw (points/lines/curves) of the specified category.
		/// </summary>
		/// <param name="curveIndex">The index of the category within the _categoryNames List</param>
		private void RedrawCurve(int curveIndex)
		{
			Rect rect = references.CurveContainer.rect;

			GraphCurve curve = curves[curveIndex];
			List<Vector2> curvePoints = plots[curveIndex];

			// calculate the point world position based on their virtual value
			Vector2[] finalPositions = curvePoints.Select(point =>
			{
				Vector2 normalizedPoint = new(
					x: (point.x - originValue.x) / (maxXValue - originValue.x),
					y: (point.y - originValue.y) / (maxYValue - originValue.y));

				return Vector2.Scale(normalizedPoint, new(rect.width, rect.height));
			}).ToArray();

			curve.SetPoints(finalPositions);
		}
		#endregion redrawing+

		private void Update()
		{
			RectTransform rectTransform = GetComponent<RectTransform>();
			if (rectTransform.hasChanged)
			{
				RedrawGridLines();
				RedrawGridLineLabels();
				RedrawCurves();
				rectTransform.hasChanged = false;
			}
		}

		public void UpdateTheme()
		{
			foreach (LineRenderer gridLine in hGridLines)
				gridLine.GetComponent<GraphGridLine>().UpdateTheme(theme);
			foreach (LineRenderer gridLine in vGridLines)
				gridLine.GetComponent<GraphGridLine>().UpdateTheme(theme);

			for (int i = 0; i < curves.Count; i++)
			{
				GraphCurve curve = curves[i].GetComponent<GraphCurve>();
				curve.SetColor(theme.CurveColors[i]);
				curve.SetWidth(theme.CurveWidth);
				curve.SetMaterial(theme.CurveMaterial);
			}
		}
	}
}
