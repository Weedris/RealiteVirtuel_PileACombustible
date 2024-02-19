using System;
using UnityEngine;

public enum Language
{
	ENGLISH,
	FRENCH,
	PORTUGUES
}

[Serializable]
public class LanguageRef
{
	public Language language;
	public ContextAssembly assemblyContext;
	public ContextExitDialog exitDialogContext;
	public ContextIntroductionDialogs introductionDialogsContext;
	public ContextMainMenu mainMenuContext;
	public ContextPerformanceLab performanceLabContext;

}

[CreateAssetMenu(fileName = "Translation", menuName = "ScriptableObjects/Lang/Translation")]
public class Translation : ScriptableObject
{
	public LanguageRef[] refs;
}
