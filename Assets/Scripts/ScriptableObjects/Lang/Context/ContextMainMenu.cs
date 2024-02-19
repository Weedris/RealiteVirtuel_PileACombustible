using UnityEngine;


[CreateAssetMenu(fileName = "MainMenu", menuName = "ScriptableObjects/Lang/Context/MainMenu")]
public class ContextMainMenu : Context
{
	[SerializeField] private string go;
	[SerializeField][TextArea(3, 5)] private string part1Label;
	[SerializeField][TextArea(3, 5)] private string part2Label;
}
