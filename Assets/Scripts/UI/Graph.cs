using System.Collections;
using System.Collections.Generic;
using System.Linq;
using TMPro;
using UnityEngine;
using UnityEngine.Rendering;

public class Graph : MonoBehaviour
{
	[Header("References")]
	[SerializeField] private LineRenderer _xAxis;
	[SerializeField] private LineRenderer _yAxis;
	[SerializeField] private RectTransform _linesContainer;
	[SerializeField] private RectTransform _hScalesContainer;
	[SerializeField] private RectTransform _vScalesContainer;
	[SerializeField] private RectTransform _legend;

	[Header("Graph Settings")]
	[SerializeField] private int _nbScalesH = 5;
	[SerializeField] private int _nbScalesV = 4;
	[SerializeField] private float _scaleWidth = 0.005f;
	[SerializeField] private float _lineWidth = 0.01f;
	[SerializeField] private float _axisWidth = 0.01f;
	[SerializeField] private float _marginBottom = 2f;
	[SerializeField] private float _marginTop = 1f;
	[SerializeField] private float _marginLeft = 1f;
	[SerializeField] private float _marginRight = 1f;
	[SerializeField] private float _pointSize = 1f;

	[Header("Theme")]
	[SerializeField] private Color _axisColor = Color.gray;
	[SerializeField] private Color _scaleColor = Color.grey;
	[SerializeField] private List<Color> _categoryColors = new() { Color.yellow, Color.green, Color.red, Color.blue, Color.cyan, Color.magenta };


	[Header("Rendering")]
	[SerializeField] private GameObject _pointPrefab;

	[Header("Data")]
	[SerializeField] private List<string> _categoryNames;

	private List<List<Vector3>> _plots = new();

	[Header("BaseGrid")]
	[SerializeField] private float minX = Mathf.Infinity;
	[SerializeField] private float minY = Mathf.Infinity;
	[SerializeField] private float maxX = -Mathf.Infinity;
	[SerializeField] private float maxY = -Mathf.Infinity;

	private float _minX;
	private float _minY;
	private float _maxX;
	private float _maxY;

	private LineRenderer[] GetLineRenderers()
	{
		return new[] { _xAxis };
	}

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
		//AddPoints(new() { new(20f, 18f), new(29.7f, 17.4f), new(39.8f, 16.8f), new(49.8f, 15.9f), new(59.8f, 15.2f) }, "Student");
		Redraw();
	}


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

		Redraw();
	}

	public void AddPoints(List<Vector3> points, string categoryName)
	{
		int categoryID = GetCategoryIdFromName(categoryName);
		List<Vector3> category = _plots[categoryID];
		category.AddRange(points);
		category = category.OrderBy(point => point.x).ToList();

		Redraw();
	}

	public void RemovePoint(Vector3 pointPosition, string category)
	{
		int lineID = GetCategoryIdFromName(category);
		var line = _plots[lineID];
		line.Remove(pointPosition);

		Redraw();
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
	}

	public void RemoveCategory(string categoryName)
	{
		int categoryID = GetCategoryIdFromName(categoryName);
		Destroy(_linesContainer.GetChild(categoryID).gameObject);
		_categoryNames.Remove(categoryName);
		_plots.RemoveAt(categoryID);

		CalculateMinMaxValues();
	}

	private LineRenderer CreateLR(RectTransform container, string name, Color color, float width, Material material)
	{
		GameObject go = new(name, typeof(LineRenderer), typeof(RectTransform));

		RectTransform t = go.GetComponent<RectTransform>();
		t.SetParent(container);
		t.localScale = Vector3.one;
		t.rotation = new(0, 0, 0, 0);
		t.localPosition = Vector3.zero;


		LineRenderer lr = go.GetComponent<LineRenderer>();
		lr.shadowCastingMode = ShadowCastingMode.Off;
		lr.alignment = LineAlignment.TransformZ;
		lr.material = material;
		lr.startColor = color;
		lr.endColor = color;
		lr.startWidth = width;
		lr.endWidth = width;
		lr.useWorldSpace = false;

		return lr;
	}

	private TMP_Text CreateTMP(Transform container, string name, string text)
	{
		GameObject go = new(name, typeof(RectTransform), typeof(TextMeshProUGUI));

		Transform t = go.transform;
		t.SetParent(container);
		t.rotation = new(0, 0, 0, 0);
		t.localScale = Vector3.one;

		TMP_Text label = go.GetComponent<TMP_Text>();
		label.text = text;
		label.alignment = TextAlignmentOptions.Center;
		label.fontSize = 0.5f;

		return label;
	}

	private void CalculateMinMaxValues()
	{
		_minX = Mathf.Infinity;
		_minY = Mathf.Infinity;
		_maxX = -Mathf.Infinity;
		_maxY = -Mathf.Infinity;
		foreach (List<Vector3> category in _plots)
		{
			foreach (Vector3 point in category)
			{
				if (point.x < _minX) _minX = point.x;
				if (point.y < _minY) _minY = point.y;
				if (point.x > _maxX) _maxX = point.x;
				if (point.y > _maxY) _maxY = point.y;
			}
		}
		if (_minX == Mathf.Infinity) _minX = minX;
		if (_minY == Mathf.Infinity) _minY = minY;
		if (_maxX == -Mathf.Infinity) _maxX = maxX;
		if (_maxY == -Mathf.Infinity) _maxY = maxY;
	}

	public void Redraw()
	{
		CalculateMinMaxValues();

		Rect rect = GetComponent<RectTransform>().rect;
		Vector3 origin = new(_marginLeft, _marginBottom);
		Vector3 xEnd = new(rect.width - _marginRight, origin.y);
		Vector3 yEnd = new(origin.x, rect.height - _marginTop);
		Vector3 availableSpace = new(xEnd.x - origin.x, yEnd.y - origin.y);
		float spaceBetweenHScales = availableSpace.y / (_nbScalesH + 1);
		float spaceBetweenVScales = availableSpace.x / (_nbScalesV + 1);
		float hxStartPosition = _marginLeft / 2;
		float vyStartPosition = _marginBottom / 2;

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
			TMP_Text tmpText = CreateTMP(hlr.transform, "UnitLabel", (_minY + i * (_maxY - _minY) / (_nbScalesH + 1)).ToString("F2"));
			tmpText.transform.localPosition = startPosition + new Vector3(-_marginLeft / 2, _marginBottom / 2);
		}
		TMP_Text maxYLabel = CreateTMP(_hScalesContainer, "MaxYLabel", _maxY.ToString("F2"));
		maxYLabel.transform.localPosition = yEnd + new Vector3(-_marginLeft, 0);

		// redraw V Scales
		for (int i = 1; i < _nbScalesV + 1; i++)
		{
			LineRenderer vlr = CreateLR(_vScalesContainer, "ScaleV" + i, _scaleColor, _scaleWidth, _yAxis.material);

			Vector3 startPosition = new(origin.x + spaceBetweenVScales * i, vyStartPosition);
			Vector3 endPosition = new(startPosition.x, yEnd.y);
			vlr.SetPositions(new[] { startPosition, endPosition });

			// unit indicator
			TMP_Text tmpText = CreateTMP(vlr.transform, "UnitLabel", (_minX + i * (_maxX - _minX) / (_nbScalesV + 1)).ToString());
			tmpText.transform.localPosition = startPosition + new Vector3(_marginLeft, 0);
		}
		TMP_Text maxXLabel = CreateTMP(_vScalesContainer, "MaxXLabel", _maxX.ToString("F2"));
		maxXLabel.transform.localPosition = xEnd + new Vector3(0, -_marginBottom/2);

		// TODO redraw legend


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
					(point.x - _minX) / (_maxX - _minX),
					(point.y - _minY) / (_maxY - _minY));
				return origin + Vector3.Scale(normalizedPoint, availableSpace);
			}).ToArray());
		}
	}
}
