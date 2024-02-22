using System.Linq;
using TMPro;
using UnityEngine;

public class ScreenInstructionsBuilding : MonoBehaviour, ILangUpdatable
{
	[SerializeField] TMP_Text screenText;

	private string[] instructions;
	private int currentIndex;

	public void NextInstruction()
	{
		currentIndex++;
		if (currentIndex < instructions.Length)
			UpdateCurrentInstruction();
		else
			OnEnd();
	}

	private void OnEnd()
	{
		GameManager.Instance.End();
	}

	private void UpdateCurrentInstruction()
	{
		screenText.text = instructions[currentIndex];
	}

	public void UpdateLang(LanguageRef lang)
	{
		ContextAssembly assemblyContext = lang.AssemblyContext;
		string welcome = lang.IntroductionDialogsContext.GetMessages()[0];
		string[] assemblyInstructions = assemblyContext.GetAllInstructions();
		instructions = new[] { welcome }.Concat(assemblyInstructions).ToArray();
		UpdateCurrentInstruction();
	}
}