using UnityEngine;

[CreateAssetMenu(fileName = "Introduction", menuName = "ScriptableObjects/Lang/Context/Introduction")]
public class ContextIntroductionDialogs : Context
{
	public string nextButtonText;

	[SerializeField] private string welcome;
	[SerializeField] [TextArea(1, 3)] private string goal;
	[SerializeField] [TextArea(2, 5)] private string instruction;
	[SerializeField] private string warning;
	[SerializeField] [TextArea(4, 5)] private string warningMessage;
	[SerializeField] [TextArea(2, 4)] private string buildingMessage;

	public string[] GetMessages()
	{
		return new string[]
		{
			"<style=\"Title\">" + welcome + "</style>",
			goal,
			instruction,
			"<style=\"Title\">" + warning + "</style>\n" + warningMessage,
			buildingMessage
		};
	}
}
