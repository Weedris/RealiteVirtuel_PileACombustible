using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using TMPro;
using UnityEditor;
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
	[SerializeField]
	[Tooltip("The values bounds by default that need to be used if exactly 0 or 1 points are present")]
	private MinMax2D _defaultBounds = new(0, 0, 100, 50);
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
	//[SerializeField] private GameObject _pointPrefab;
	//[SerializeField] private float _pointRadius = 1f;
	[SerializeField] private Color _axisColor = Color.white;
	[SerializeField] private Color _scaleColor = Color.grey;
	[SerializeField] private List<Color> _categoryColors = new() { Color.yellow, Color.green, Color.red, Color.blue, Color.cyan, Color.magenta };
	[SerializeField] private TMP_FontAsset _font = default;
	[SerializeField][Min(0)] private float _unitFontSize = 12;
	[SerializeField][Min(0)] private float _legendFontSize = 12;
	#endregion fields

	#region variables
	[Header("Data")]
	[SerializeField] private List<string> _categoryNames;
	private List<List<Vector3>> _plots = new();

	private List<LineRenderer> _scaleHLR = new();
	private List<LineRenderer> _scaleVLR = new();
	private List<GameObject> _categoryLines = new();

	private List<GameObject> _legendItems = new();

	private Rect rect;
	private Vector3 origin;
	private Vector3 xEnd;
	private Vector3 yEnd;
	private Vector3 availableSpace;
	private MinMax2D _bounds;
	#endregion variables

	#region init
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

	private IEnumerator LateStart(float waitTime)
	{
		yield return new WaitForSeconds(waitTime);

		rect = _graph.rect;
		origin = new(_margin.Left, _margin.Bottom);
		xEnd = new(rect.width - _margin.Right, origin.y);
		yEnd = new(origin.x, rect.height - _margin.Top);
		availableSpace = new(xEnd.x - origin.x, yEnd.y - origin.y);

		RecalculateBounds();
		RedrawAxis();
		RedrawScales();
	}
	#endregion init

	#region utility
	private int GetCategoryIdFromName(string categoryName)
	{
		int lineID = 0;
		bool categoryExists = false;
		while (lineID < _categoryNames.Count && !categoryExists)
		{
			if (_categoryNames[lineID] == categoryName) categoryExists = true;
			else lineID++;
		}
		if (!categoryExists && lineID == _categoryNames.Count) AddCategory(categoryName);
		return lineID;
	}

	private void RecalculateBounds()
	{
		_bounds = new(Mathf.Infinity, Mathf.Infinity, -Mathf.Infinity, -Mathf.Infinity);

		int nbMaxPoints = 0;
		// calc bound
		foreach (List<Vector3> category in _plots)
		{
			nbMaxPoints = Math.Max(nbMaxPoints, category.Count);
			foreach (Vector3 point in category)
			{
				if (point.x < _bounds.MinX) _bounds.MinX = point.x;
				if (point.y < _bounds.MinY) _bounds.MinY = point.y;
				if (point.x > _bounds.MaxX) _bounds.MaxX = point.x;
				if (point.y > _bounds.MaxY) _bounds.MaxY = point.y;
			}
		}

		bool hasMinimumPoints = nbMaxPoints > 1;
		bool isMinXValid = _bounds.MinX == Mathf.Infinity;
		bool isMinYValid = _bounds.MinY == Mathf.Infinity;
		bool isMaxXValid = _bounds.MaxX == -Mathf.Infinity;
		bool isMaxYValid = _bounds.MaxY == -Mathf.Infinity;

		// recenter bounds depending on settings
		if (!hasMinimumPoints || isMinXValid || _forceOrigin) _bounds.MinX = _defaultBounds.MinX;
		if (!hasMinimumPoints || isMinYValid || _forceOrigin) _bounds.MinY = _defaultBounds.MinY;
		if (!hasMinimumPoints || isMaxXValid || _forceMaxX) _bounds.MaxX = _defaultBounds.MaxX;
		if (!hasMinimumPoints || isMaxYValid || _forceMaxY) _bounds.MaxY = _defaultBounds.MaxY;
	}

	#region component_creation
	private void SetRectTransformBaseOptions(RectTransform item, RectTransform parent, Vector3 localPosition = default, Quaternion rotation = default)
	{
		item.SetParent(parent);
		item.rotation = rotation;
		item.localPosition = localPosition;
		item.localScale = Vector3.one;
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

	private void SetLabelBaseOptions(TMP_Text item, string text, float fontSize = 0.5f, TextAlignmentOptions textAlignment = TextAlignmentOptions.Center, bool enableAutoSizing = false, Color? color = null)
	{
		if (color == null)
			color = Color.white;
		item.alignment = textAlignment;
		item.fontSize = fontSize;
		item.font = _font;
		item.text = text;
		item.enableAutoSizing = enableAutoSizing;
		item.fontSizeMax = fontSize;
		item.fontSizeMin = 0.5f;
		item.color = (Color) color;
	}

	private LineRenderer CreateLR(RectTransform container, string name, Color color, float width, Material material)
	{
		GameObject go = new(name, typeof(RectTransform));
		RectTransform rt = go.GetComponent<RectTransform>();
		SetRectTransformBaseOptions(rt, container);
		LineRenderer lr = go.AddComponent<LineRenderer>();
		SetLineRendererBaseOptions(lr, width, color, material);
		return lr;
	}

	private TMP_Text CreateTMP(RectTransform container, string name, string text, float fontSize)
	{
		GameObject go = new(name);
		RectTransform rt = go.AddComponent<RectTransform>();
		SetRectTransformBaseOptions(rt, container);
		TMP_Text label = go.AddComponent<TextMeshProUGUI>();
		SetLabelBaseOptions(label, text, fontSize);
		return label;
	}
	#endregion component_creation
	#endregion utility

	#region data_gestion
	public void AddPoint(float x, float y, string categoryName)
	{
		int categoryID = GetCategoryIdFromName(categoryName);

		List<Vector3> categoryPoints = _plots[categoryID];
		Vector3 pointPosition = new(x, y);

		// insert point while making sure it's sorted by x values
		int i = 0;
		bool reached = false;
		while (!reached && i < categoryPoints.Count)
		{
			if (categoryPoints[i].x > x)
			{
				categoryPoints.Insert(i, pointPosition);
				reached = true;
			}
			i++;
		}
		if (!reached)
			categoryPoints.Add(pointPosition);

		// recalculate bounds
		MinMax2D oldBounds = _bounds.Copy();
		RecalculateBounds();
		if (oldBounds.MinX != _bounds.MinX || oldBounds.MaxX != _bounds.MaxX)
			RedrawVerticalScales();
		if (oldBounds.MinY != _bounds.MinY || oldBounds.MaxY != _bounds.MaxY)
			RedrawHorizontalScales();

		RedrawPoints(categoryID);
	}

	public void AddPoints(List<Vector3> points, string categoryName)
	{
		int categoryID = GetCategoryIdFromName(categoryName);
		List<Vector3> category = _plots[categoryID];
		category.AddRange(points);
		category = category.OrderBy(point => point.x).ToList();

		RecalculateBounds();
		RedrawPoints(categoryID);
	}

	public void RemovePoint(Vector3 pointPosition, string category)
	{
		// ignore nonsense
		if (!_categoryNames.Contains(category)) return;

		int categoryID = GetCategoryIdFromName(category);
		_plots[categoryID].Remove(pointPosition);

		RedrawPoints(categoryID);
	}

	public void AddCategory(string name)
	{
		Color categoryColor = _categoryColors[_categoryNames.Count];

		GameObject line = new(name, typeof(RectTransform));
		GameObject hBox = new(name, typeof(RectTransform), typeof(LayoutElement));
		GameObject label = new("Label", typeof(RectTransform));

		HorizontalLayoutGroup hBoxLayout = hBox.AddComponent<HorizontalLayoutGroup>();
		LayoutElement labelLE = label.AddComponent<LayoutElement>();

		RectTransform lineRT = line.GetComponent<RectTransform>();
		RectTransform hBoxRT = hBox.GetComponent<RectTransform>();
		RectTransform labelRT = label.GetComponent<RectTransform>();

		LineRenderer lineLR = line.AddComponent<LineRenderer>();
		TMP_Text labelT = label.AddComponent<TextMeshProUGUI>();

		hBoxLayout.childForceExpandWidth = hBoxLayout.childForceExpandHeight = false;
		hBoxLayout.childControlWidth = hBoxLayout.childControlHeight = true;
		hBoxLayout.childAlignment = TextAnchor.MiddleLeft;

		SetRectTransformBaseOptions(lineRT, _linesContainer);
		SetRectTransformBaseOptions(hBoxRT, _legend);
		SetRectTransformBaseOptions(labelRT, hBoxRT);
		SetLineRendererBaseOptions(lineLR, _lineWidth, categoryColor, _xAxis.material);
		SetLabelBaseOptions(labelT, name, _legendFontSize, enableAutoSizing:true, color:categoryColor);

		_legendItems.Add(hBox);
		_categoryLines.Add(line);
		_categoryNames.Add(name);
		_plots.Add(new());

		if (_categoryNames.Count == 1)
		{
			RedrawAxis();
			RedrawScales();
		}
	}

	public void RemoveCategory(string categoryName)
	{
		// ignore when nothing needs to be deleted
		if (!_categoryNames.Contains(categoryName)) return;

		// retrieve categoryID
		int categoryID = GetCategoryIdFromName(categoryName);

		// destroy UI elements
		Destroy(_legendItems[categoryID]);
		Destroy(_categoryLines[categoryID]);

		// destroy related data
		_categoryNames.RemoveAt(categoryID);
		_categoryLines.RemoveAt(categoryID);
		_legendItems.RemoveAt(categoryID);
		_plots.RemoveAt(categoryID);
	}
	#endregion data_gestion

	#region redrawing
	private void RedrawHorizontalScales()
	{
		float spaceBetweenHScales = availableSpace.y / (_nbScalesH + 1);
		float hxStartPosition = _margin.Left / 2;

		// delete old horizontal scales
		foreach (Transform scale in _hScalesContainer)
			Destroy(scale.gameObject);
		_scaleHLR.Clear();

		// redraw horizontal scales
		for (int i = 1; i < _nbScalesH + 1; i++)
		{
			LineRenderer hlr = CreateLR(_hScalesContainer, "ScaleH" + i, _scaleColor, _scaleWidth, _yAxis.material);
			Vector3 startPosition = new(hxStartPosition, origin.y + spaceBetweenHScales * i);
			Vector3 endPosition = new(xEnd.x, startPosition.y);
			hlr.SetPositions(new[] { startPosition, endPosition });

			// unit indicator
			string text = (_bounds.MinY + i * (_bounds.MaxY - _bounds.MinY) / (_nbScalesH + 1)).ToString("F2");
			TMP_Text tmpText = CreateTMP(hlr.GetComponent<RectTransform>(), "UnitLabel", text, _unitFontSize);
			tmpText.transform.localPosition = startPosition + new Vector3(-_margin.Left / 2, _margin.Bottom / 2);

			// remember component
			_scaleHLR.Add(hlr);
		}

		TMP_Text maxYLabel = CreateTMP(_hScalesContainer, "MaxYLabel", _bounds.MaxY.ToString("F2"), _unitFontSize);
		maxYLabel.transform.localPosition = yEnd + new Vector3(-_margin.Left, 0);
	}

	private void RedrawVerticalScales()
	{
		float spaceBetweenVScales = availableSpace.x / (_nbScalesV + 1);
		float vyStartPosition = _margin.Bottom / 2;  // y position all scales start from

		// destroy old vertical scales
		foreach (Transform scale in _vScalesContainer)
			Destroy(scale.gameObject);

		_scaleVLR.Clear();

		// redraw vertical scales
		for (int i = 1; i < _nbScalesV + 1; i++)
		{
			LineRenderer vlr = CreateLR(_vScalesContainer, "ScaleV" + i, _scaleColor, _scaleWidth, _yAxis.material);

			Vector3 startPosition = new(origin.x + spaceBetweenVScales * i, vyStartPosition);
			Vector3 endPosition = new(startPosition.x, yEnd.y);
			vlr.SetPositions(new[] { startPosition, endPosition });

			// unit indicator
			string text = (_bounds.MinX + i * (_bounds.MaxX - _bounds.MinX) / (_nbScalesV + 1)).ToString("F2");
			TMP_Text tmpText = CreateTMP(vlr.GetComponent<RectTransform>(), "UnitLabel", text, _unitFontSize);
			tmpText.transform.localPosition = startPosition + new Vector3(_margin.Left, 0);

			// remember component
			_scaleVLR.Add(vlr);
		}

		TMP_Text maxXLabel = CreateTMP(_vScalesContainer, "MaxXLabel", _bounds.MaxX.ToString("F2"), _unitFontSize);
		maxXLabel.transform.localPosition = xEnd + new Vector3(0, -_margin.Bottom / 2);
	}

	private void RedrawScales()
	{
		RedrawHorizontalScales();
		RedrawVerticalScales();
	}

	private void RedrawAxis()
	{
		_xAxis.SetPositions(new[] { origin, xEnd });
		_yAxis.SetPositions(new[] { origin, yEnd });
	}

	/// <summary>
	/// Redraw (points/lines/curves) of the specified category.
	/// </summary>
	/// <param name="categoryID">The index of the category within the _categoryNames List</param>
	private void RedrawPoints(int categoryID)
	{
		LineRenderer lr = _categoryLines[categoryID].GetComponent<LineRenderer>();
		List<Vector3> categoryPoints = _plots[categoryID];

		Vector3[] fixedPositions = categoryPoints.Select(point =>
		{
			Vector3 normalizedPoint = new(
				(point.x - _bounds.MinX) / (_bounds.MaxX - _bounds.MinX),
				(point.y - _bounds.MinY) / (_bounds.MaxY - _bounds.MinY));
			return origin + Vector3.Scale(normalizedPoint, availableSpace);
		}).ToArray();

		if (categoryPoints.Count >= 2)
		{
			lr.positionCount = categoryPoints.Count;
			lr.SetPositions(fixedPositions);
		}
		else
		{
			lr.positionCount = 2;
			lr.SetPositions(new Vector3[] { new(_margin.Left, _margin.Bottom), new(_margin.Left, _margin.Bottom) });
		}
	}
	#endregion redrawing
}
