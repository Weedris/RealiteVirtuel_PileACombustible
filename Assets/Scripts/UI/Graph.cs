using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using TMPro;
using UnityEngine;
using UnityEngine.Rendering;
using UnityEngine.UI;


public class Graph : MonoBehaviour
{
	#region fields
	[Header("Components")]
	[SerializeField] private RectTransform _graph;
	[SerializeField] private RectTransform _legend;

	[Header("Graph Components")]
	[SerializeField] private LineRenderer _xAxis;
	[SerializeField] private LineRenderer _yAxis;
	[SerializeField] private RectTransform _linesContainer;
	[SerializeField] private RectTransform _hScalesContainer;
	[SerializeField] private RectTransform _vScalesContainer;

	[Header("Graph Settings")]
	[SerializeField] private int _nbScalesH = 5;
	[SerializeField] private int _nbScalesV = 4;
	[SerializeField] private float _scaleWidth = 0.005f;
	[SerializeField] private float _lineWidth = 0.01f;
	[SerializeField] private float _axisWidth = 0.01f;
	[SerializeField] private Margin _margin = new(1, 1, 2, 1);
	[SerializeField] private MinMax2D _defaultBounds = new(0, 0, 100, 50);
	[SerializeField]
	[Tooltip("Wether you want the origin to stay at the specified minX and minY")]
	private bool _forceOrigin = false;
	[SerializeField]
	[Tooltip("Wether or not you want the end of the x axis to the specified maxX")]
	private bool _forceMaxX = false;
	[SerializeField]
	[Tooltip("Wether or not you want the end of the y axis to the specified maxY")]
	private bool _forceMaxY = false;

	[Header("Theme")]
	[SerializeField] private GameObject _pointPrefab;
	[SerializeField] private float _pointRadius = 1f;
	[SerializeField] private Color _axisColor = Color.white;
	[SerializeField] private Color _scaleColor = Color.grey;
	[SerializeField] private List<Color> _categoryColors = new() { Color.yellow, Color.green, Color.red, Color.blue, Color.cyan, Color.magenta };

	[Header("Data")]
	[SerializeField] private List<string> _categoryNames;
	#endregion fields

	private MinMax2D _bounds;
	private List<List<Vector3>> _plots = new();

	private void Start()
	{
		_xAxis.startWidth = _axisWidth;
		_yAxis.startWidth = _axisWidth;

		_xAxis.startColor = _axisColor;
		_yAxis.startColor = _axisColor;
		_xAxis.endColor = _axisColor;
		_yAxis.endColor = _axisColor;

		for (int i = 0; i < _categoryNames.Count; i++)
			_plots.Add(new());

		StartCoroutine(LateStart(1f));
	}

	IEnumerator LateStart(float waitTime)
	{
		yield return new WaitForSeconds(waitTime);
		RedrawGraph();
		RedrawLegend();
	}

	#region data_gestion
	public void AddPoint(float x, float y, string categoryName)
	{
		Vector3 position = new(x, y);
		int lineID = GetCategoryIdFromName(categoryName);
		List<Vector3> categoryPoints = _plots[lineID];

		// insert point while making sure it's sorted by x values
		bool reached = false;
		int i = 0;
		while (!reached && i < categoryPoints.Count)
		{
			if (categoryPoints[i].x > x)
			{
				categoryPoints.Insert(i, position);
				reached = true;
			}
			i++;
		}
		if (!reached) categoryPoints.Append(position);

		RedrawGraph();
	}

	public void AddPoints(List<Vector3> points, string categoryName)
	{
		int categoryID = GetCategoryIdFromName(categoryName);
		List<Vector3> category = _plots[categoryID];
		category.AddRange(points);
		category = category.OrderBy(point => point.x).ToList();

		RedrawGraph();
	}

	public void RemovePoint(Vector3 pointPosition, string category)
	{
		int lineID = GetCategoryIdFromName(category);
		var line = _plots[lineID];
		line.Remove(pointPosition);

		RedrawGraph();
	}

	private int GetCategoryIdFromName(string categoryName)
	{
		return _categoryNames.IndexOf(categoryName);
	}

	public void AddCategory(string name)
	{
		new GameObject(name, typeof(LineRenderer)).transform.parent = _linesContainer;
		_categoryNames.Add(name);
		_plots.Add(new());
		RedrawLegend();
	}

	public void RemoveCategory(string categoryName)
	{
		int categoryID = GetCategoryIdFromName(categoryName);
		Destroy(_linesContainer.GetChild(categoryID).gameObject);
		_categoryNames.Remove(categoryName);
		_plots.RemoveAt(categoryID);

		RedrawGraph();
		RedrawLegend();
	}

	private void CalculateMinMaxValues()
	{
		_bounds = new(Mathf.Infinity, Mathf.Infinity, -Mathf.Infinity, -Mathf.Infinity);

		foreach (List<Vector3> category in _plots)
		{
			foreach (Vector3 point in category)
			{
				if (point.x < _bounds.MinX) _bounds.MinX = point.x;
				if (point.y < _bounds.MinY) _bounds.MinY = point.y;
				if (point.x > _bounds.MaxX) _bounds.MaxX = point.x;
				if (point.y > _bounds.MaxY) _bounds.MaxY = point.y;
			}
		}
		if (_forceOrigin || _bounds.MinX == Mathf.Infinity) _bounds.MinX = _defaultBounds.MinX;
		if (_forceOrigin || _bounds.MinY == Mathf.Infinity) _bounds.MinY = _defaultBounds.MinY;
		if (_forceMaxX || _bounds.MaxX == -Mathf.Infinity) _bounds.MaxX = _defaultBounds.MaxX;
		if (_forceMaxY || _bounds.MaxY == -Mathf.Infinity) _bounds.MaxY = _defaultBounds.MaxY;
	}
	#endregion data_gestion

	#region redrawing
	public void RedrawGraph()
	{
		CalculateMinMaxValues();

		Rect rect = _graph.rect;
		Vector3 origin = new(_margin.Left, _margin.Bottom);
		Vector3 xEnd = new(rect.width - _margin.Right, origin.y);
		Vector3 yEnd = new(origin.x, rect.height - _margin.Top);
		Vector3 availableSpace = new(xEnd.x - origin.x, yEnd.y - origin.y);
		float spaceBetweenHScales = availableSpace.y / (_nbScalesH + 1);
		float spaceBetweenVScales = availableSpace.x / (_nbScalesV + 1);
		float hxStartPosition = _margin.Left / 2;
		float vyStartPosition = _margin.Bottom / 2;

		// redraw axis
		_xAxis.SetPositions(new[] { origin, xEnd });
		_yAxis.SetPositions(new[] { origin, yEnd });

		// delete old scales
		while (_hScalesContainer.childCount > 0) Destroy(_hScalesContainer.GetChild(0).gameObject);
		while (_vScalesContainer.childCount > 0) Destroy(_vScalesContainer.GetChild(0).gameObject);

		// redraw H Scales
		for (int i = 1; i < _nbScalesH + 1; i++)
		{
			LineRenderer hlr = CreateLR(_hScalesContainer, "ScaleH" + i, _scaleColor, _scaleWidth, _yAxis.material);

			Vector3 startPosition = new(hxStartPosition, origin.y + spaceBetweenHScales * i);
			Vector3 endPosition = new(xEnd.x, startPosition.y);
			hlr.SetPositions(new[] { startPosition, endPosition });

			// unit indicator
			TMP_Text tmpText = CreateTMP(hlr.GetComponent<RectTransform>(), "UnitLabel", (_bounds.MinY + i * (_bounds.MaxY - _bounds.MinY) / (_nbScalesH + 1)).ToString("F2"));
			tmpText.transform.localPosition = startPosition + new Vector3(-_margin.Left / 2, _margin.Bottom / 2);
		}

		TMP_Text maxYLabel = CreateTMP(_hScalesContainer, "MaxYLabel", _bounds.MaxY.ToString("F2"));
		maxYLabel.transform.localPosition = yEnd + new Vector3(-_margin.Left, 0);

		// redraw V Scales
		for (int i = 1; i < _nbScalesV + 1; i++)
		{
			LineRenderer vlr = CreateLR(_vScalesContainer, "ScaleV" + i, _scaleColor, _scaleWidth, _yAxis.material);

			Vector3 startPosition = new(origin.x + spaceBetweenVScales * i, vyStartPosition);
			Vector3 endPosition = new(startPosition.x, yEnd.y);
			vlr.SetPositions(new[] { startPosition, endPosition });

			// unit indicator
			TMP_Text tmpText = CreateTMP(vlr.GetComponent<RectTransform>(), "UnitLabel", (_bounds.MinX + i * (_bounds.MaxX - _bounds.MinX) / (_nbScalesV + 1)).ToString());
			tmpText.transform.localPosition = startPosition + new Vector3(_margin.Left, 0);
		}

		TMP_Text maxXLabel = CreateTMP(_vScalesContainer, "MaxXLabel", _bounds.MaxX.ToString("F2"));
		maxXLabel.transform.localPosition = xEnd + new Vector3(0, -_margin.Bottom / 2);

		// redraw points
		while (_linesContainer.childCount > 0)
			Destroy(_linesContainer.GetChild(0).gameObject);

		for (int i = 0; i < _categoryNames.Count; i++)
		{
			string categoryName = _categoryNames[i];
			List<Vector3> categoryPoints = _plots[i];
			Color categoryColor = _categoryColors[i];

			LineRenderer lr = CreateLR(_linesContainer, categoryName, categoryColor, _lineWidth, _yAxis.material);
			lr.positionCount = categoryPoints.Count;

			lr.SetPositions(categoryPoints.Select(point =>
			{
				Vector3 normalizedPoint = new(
					(point.x - _bounds.MinX) / (_bounds.MaxX - _bounds.MinX),
					(point.y - _bounds.MinY) / (_bounds.MaxY - _bounds.MinY));
				return origin + Vector3.Scale(normalizedPoint, availableSpace);
			}).ToArray());
		}
	}

	private void RedrawLegend()
	{
		while (_legend.childCount > 0)
			Destroy(_legend.GetChild(0).gameObject);

		for (int i = 0; i < _categoryNames.Count; i++)
		{
			string categoryName = _categoryNames[i];
			Color categoryColor = _categoryColors[i];

			GameObject hBox = new(categoryName, typeof(LayoutElement));
			GameObject coloredLine = new("Line");
			GameObject label = new("Label");

			HorizontalLayoutGroup hBoxLayout = hBox.AddComponent<HorizontalLayoutGroup>();
			hBoxLayout.childControlWidth = true;

			LayoutElement colorLineLE = coloredLine.AddComponent<LayoutElement>();
			LayoutElement labelLE = label.AddComponent<LayoutElement>();
			colorLineLE.flexibleWidth = 0.2f;
			labelLE.flexibleWidth = 0.8f;

			RectTransform hBoxRT = hBox.AddComponent<RectTransform>();
			RectTransform coloredLineRT = coloredLine.AddComponent<RectTransform>();
			RectTransform labelRT = label.AddComponent<RectTransform>();
			SetRectTransformBaseOptions(hBoxRT, _legend);
			SetRectTransformBaseOptions(coloredLineRT, hBoxRT);
			SetRectTransformBaseOptions(labelRT, hBoxRT);

			LineRenderer colorLineLR = coloredLine.AddComponent<LineRenderer>();
			SetLineRendererBaseOptions(colorLineLR, color: categoryColor, mat: _xAxis.material);

			TMP_Text labelT = label.AddComponent<TextMeshProUGUI>();
			SetLabelBaseOptions(labelT, categoryName);
		}
	}
	#endregion redrawing

	#region component_creation
	private void SetRectTransformBaseOptions(RectTransform item, RectTransform parent, Vector3 localPosition = default, Quaternion rotation = default, Vector3? localScale = null)
	{
		localScale ??= Vector3.one;

		item.SetParent(parent);
		item.rotation = rotation;
		item.localPosition = localPosition;
		item.localScale = (Vector3)localScale;
	}

	private void SetLineRendererBaseOptions(LineRenderer item, float width = 1f, Color color = default, Material mat = default)
	{
		item.shadowCastingMode = ShadowCastingMode.Off;
		item.alignment = LineAlignment.TransformZ;
		item.startWidth = item.endWidth = width;
		item.startColor = item.endColor = color;
		item.useWorldSpace = false;
		item.material = mat;
	}

	private void SetLabelBaseOptions(TMP_Text item, string text, float fontSize = 0.5f)
	{
		item.text = text;
		item.alignment = TextAlignmentOptions.Center;
		item.fontSize = 0.5f;
	}

	private LineRenderer CreateLR(RectTransform container, string name, Color color, float width, Material material)
	{
		GameObject go = new(name);
		RectTransform t = go.AddComponent<RectTransform>();
		SetRectTransformBaseOptions(t, container);
		LineRenderer lr = go.AddComponent<LineRenderer>();
		SetLineRendererBaseOptions(lr, width, color, material);
		return lr;
	}

	private TMP_Text CreateTMP(RectTransform container, string name, string text)
	{
		GameObject go = new(name);
		RectTransform t = go.AddComponent<RectTransform>();
		SetRectTransformBaseOptions(t, container);
		TMP_Text label = go.AddComponent<TextMeshProUGUI>();
		SetLabelBaseOptions(label, text);
		return label;
	}
	#endregion component_creation
}
