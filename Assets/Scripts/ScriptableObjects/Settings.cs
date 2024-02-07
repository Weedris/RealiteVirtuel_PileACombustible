using System;
using UnityEngine;

[CreateAssetMenu(menuName = "ScriptableObjects/Settings")]
public class Settings : ScriptableObject
{
	private static Settings instance;
	public static Settings Instance
	{
		get
		{
			instance ??= new();
			return instance;
		}
	}

	public Language curentLanguage;
	public Platform platform;
	[NonSerialized] public bool isPlayerPastAssembly;
}


public enum Platform
{
	PC,
	VR
}