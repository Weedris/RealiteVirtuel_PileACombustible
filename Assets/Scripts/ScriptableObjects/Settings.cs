using System;
using UnityEngine;

[CreateAssetMenu(menuName = "ScriptableObjects/Settings")]
public class Settings : ScriptableObject
{
    public static Settings Instance
    {
        get
        {
            instance ??= new Settings();
            return instance;
        }
    }

    private static Settings instance;

    public Language curentLanguage;
	public Platform platform;
	[NonSerialized] public bool isPlayerPastAssembly;
}


public enum Platform
{
	PC,
	VR
}