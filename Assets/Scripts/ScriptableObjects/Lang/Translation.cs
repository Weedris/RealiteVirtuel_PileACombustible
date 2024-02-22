using System;
using UnityEngine;

public enum Language
{
	ENGLISH,
	FRENCH,
	PORTUGUESE
}

[Serializable]
public class LanguageRef
{
	public Language Language;
	public ContextAssembly AssemblyContext;
	public ContextExitDialog ExitDialogContext;
	public ContextIntroductionDialogs IntroductionDialogsContext;
	public ContextMainMenu MainMenuContext;
	public ContextPerformanceLab PerformanceLabContext;
}

[CreateAssetMenu(fileName = "Translation", menuName = "ScriptableObjects/Lang/Translation")]
public class Translation : ScriptableObject
{
	public LanguageRef[] refs;
}
