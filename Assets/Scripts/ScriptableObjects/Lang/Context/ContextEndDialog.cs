using UnityEngine;

[CreateAssetMenu(fileName="EndDialog", menuName="ScriptableObjects/Lang/Context/EndDialog")]
public class ContextEndDialog : Context
{
	[TextArea] public string Message;
	public string Next;
	public string MainMenu;
	public string Quit;
}
