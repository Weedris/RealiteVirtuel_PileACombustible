using UnityEditor;
using Assets.Scripts.UI.Graph;

[CustomEditor(typeof(Graph))]
public class GraphEditor : Editor
{
	private SerializedProperty prefabsProperty;
	private SerializedProperty referencesProperty;
	private SerializedProperty themeProperty;
	private SerializedProperty settingsProperty;

	private void OnEnable()
	{
		prefabsProperty = serializedObject.FindProperty("prefabs");
		referencesProperty = serializedObject.FindProperty("references");
		themeProperty = serializedObject.FindProperty("theme");
		settingsProperty = serializedObject.FindProperty("settings");
	}

	public override void OnInspectorGUI()
	{
		serializedObject.Update();

		EditorGUILayout.PropertyField(prefabsProperty);
		EditorGUILayout.PropertyField(referencesProperty);

		EditorGUI.BeginChangeCheck();
		EditorGUILayout.PropertyField(themeProperty);
		bool themeChanged = EditorGUI.EndChangeCheck();

		EditorGUI.BeginChangeCheck();
		EditorGUILayout.PropertyField(settingsProperty);
		bool settingsChanged = EditorGUI.EndChangeCheck();

		serializedObject.ApplyModifiedProperties();

		// update visual in editor
		Graph targetGraph = target as Graph;
		if (themeChanged)
			targetGraph.UpdateTheme();
		if (settingsChanged)
		{
			targetGraph.RedrawCurves();
			targetGraph.RedrawGridLines();
		}
	}
}
