using UnityEngine;

[CreateAssetMenu(menuName = "ScriptableObjects/Settings")]
public class Settings : ScriptableObject
{
    public Language curentLanguage;
	public Platform platform;
}


public enum Platform
{
    PC,
	VR
}