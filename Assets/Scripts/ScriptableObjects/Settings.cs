using UnityEngine;


[CreateAssetMenu(menuName = "ScriptableObjects/Settings")]
public class Settings : ScriptableObject
{
	public static Settings Instance { get; private set; }
	public Platform platform;
	public Language curentLanguage;
	[Range(0, 1)] public float BgmLevel;
	[Range(0, 1)] public float SfxLevel;

	private void Awake()
	{
		Instance = this;
	}
}


public enum Platform
{
	PC,
	VR
}