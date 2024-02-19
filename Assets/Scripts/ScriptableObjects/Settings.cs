using UnityEngine;


[CreateAssetMenu(menuName = "ScriptableObjects/Settings")]
public class Settings : ScriptableObject
{
	public Platform platform;
	public Language curentLanguage;
	[Range(0, 1)] public float BgmLevel;
	[Range(0, 1)] public float SfxLevel;
}


public enum Platform
{
	PC,
	VR
}