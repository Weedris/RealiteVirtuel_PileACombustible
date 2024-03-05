using UnityEngine;

/// <summary>
/// This is the context that will hold everything text that has to do with the assembly of the fuel cell.
/// </summary>
[CreateAssetMenu(fileName = "Assembly", menuName = "ScriptableObjects/Lang/Context/Assembly")]
public class ContextAssembly : Context
{
	#region fields
	[SerializeField] private TextTag[] tags;

	[SerializeField][TextArea(3, 10)] private string StackInstructions;
	[SerializeField][TextArea(3, 10)] private string DihydrogenInstructions;
	[SerializeField][TextArea(3, 10)] private string AirCompressorInstructions;
	[SerializeField][TextArea(3, 10)] private string HumidifierInstructions;
	[SerializeField][TextArea(3, 10)] private string NitrogenInstructions;
	[SerializeField][TextArea(3, 10)] private string FanInstructions;
	[SerializeField][TextArea(3, 10)] private string WaterInstructions;
	[SerializeField][TextArea(3, 10)] private string RadiatorInstructions;
	#endregion fields

	#region getters
	public string GetStackInstructions() { return GetFormatedText(StackInstructions); }
	public string GetDihydrogenInstructions() { return GetFormatedText(DihydrogenInstructions); }
	public string GetAirCompressorInstructions() { return GetFormatedText(AirCompressorInstructions); }
	public string GetHumidifierInstructions() { return GetFormatedText(HumidifierInstructions); }
	public string GetNitrogenInstructions() { return GetFormatedText(NitrogenInstructions); }
	public string GetFanInstructions() { return GetFormatedText(FanInstructions); }
	public string GetWaterInstructions() { return GetFormatedText(WaterInstructions); }
	public string GetRadiatorInstructions() { return GetFormatedText(RadiatorInstructions); }
	#endregion getters


	/// <summary>
	/// Apply the translation with used tags.
	/// Ex: in english, "<hydrogen>" is replaced with "Hydrogen" with a red color.
	/// </summary>
	/// <param name="text">The text that needs to be formated.</param>
	/// <returns>The formated text.</returns>
	private string GetFormatedText(string text)
	{
		// can be optimised by looking at balises instead
		string newText = text;
		foreach (TextTag tag in tags)
			newText = newText.Replace(tag.sequence, "<color=#" + ColorUtility.ToHtmlStringRGB(tag.color) + ">" + tag.replaceWith + "</color>");
		return newText;
	}

	/// <summary>
	/// Retrieves all instructions that needs to be displayed to the player with the correct formating.
	/// </summary>
	/// <returns>The array of instructions.</returns>
	public string[] GetAllInstructions()
	{
		return new string[]
		{
			GetStackInstructions(),
			GetDihydrogenInstructions(),
			GetAirCompressorInstructions(),
			GetHumidifierInstructions(),
			GetNitrogenInstructions(),
			GetFanInstructions(),
			GetWaterInstructions(),
			GetRadiatorInstructions()
		};
	}
}
