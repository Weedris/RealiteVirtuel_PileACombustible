using UnityEngine;

[CreateAssetMenu(fileName = "ExitDialog", menuName = "ScriptableObjects/Lang/Context/ExitDialog")]
public class ContextExitDialog : Context
{
	[SerializeField] private string title;
	[SerializeField] private string message;
	[SerializeField] private string confirm;
	[SerializeField] private string cancel;
}
