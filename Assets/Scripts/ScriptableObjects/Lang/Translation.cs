using UnityEngine;

[CreateAssetMenu(fileName = "Translation", menuName = "ScriptableObjects/Lang/Translation")]
public class Translation: ScriptableObject
{
	public Sprite FlagSprite;
	public ContextAssembly AssemblyContext;
	public ContextExitDialog ExitDialogContext;
	public ContextIntroductionDialogs IntroductionDialogsContext;
	public ContextMainMenu MainMenuContext;
	public ContextPerformanceLab PerformanceLabContext;
	public ContextEndDialog EndDialogContext;
}
