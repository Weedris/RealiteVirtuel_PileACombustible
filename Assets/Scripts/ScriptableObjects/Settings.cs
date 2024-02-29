using UnityEngine;


[CreateAssetMenu(menuName = "ScriptableObjects/Settings")]
public class Settings : ScriptableObject
{
	public Translation currentLanguage;
	[Range(0, 1)] public float BgmLevel;
	[Range(0, 1)] public float SfxLevel;
}
