using UnityEngine;


[CreateAssetMenu(menuName = "ScriptableObjects/Settings")]
public class Settings : ScriptableObject
{
	public Translation currentLanguage;
	[Range(0, 1)] public float BgmVolume = .3f;
	[Range(0, 1)] public float SfxVolume = 1f;
}
