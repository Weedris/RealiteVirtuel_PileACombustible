using System;
using UnityEngine;


[Serializable]
public class Session
{
	public ScenesManager.Scene Scene;
	[TextArea]public string Description;
}

[CreateAssetMenu(fileName = "MainMenu", menuName = "ScriptableObjects/Lang/Context/MainMenu")]
public class ContextMainMenu : Context
{
	public string SessionChooserHeader;
	public string StartSession;
	public Session[] Sessions;
}
