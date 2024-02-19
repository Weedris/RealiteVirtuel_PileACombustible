using UnityEngine;

[CreateAssetMenu(fileName = "Assembly", menuName = "ScriptableObjects/Lang/Context/Assembly")]
public class ContextAssembly : Context
{
	[SerializeField] private TextTag[] tags;

	[SerializeField] [TextArea(3, 10)] private string StackInstructions;
	[SerializeField] [TextArea(3, 10)] private string DihydrogenInstructions;
	[SerializeField] [TextArea(3, 10)] private string AirCompressorInstructions;
	[SerializeField] [TextArea(3, 10)] private string HumidifierInstructions;
	[SerializeField] [TextArea(3, 10)] private string NitrogenInstructions;
	[SerializeField] [TextArea(3, 10)] private string FanInstructions;
	[SerializeField] [TextArea(3, 10)] private string WaterInstructions;
	[SerializeField] [TextArea(3, 10)] private string RadiatorInstructions;

	public string GetStackInstructions() { return GetFormatedText(StackInstructions); }
	public string GetDihydrogenInstructions() { return GetFormatedText(DihydrogenInstructions); }
	public string GetAirCompressorInstructions() { return GetFormatedText(AirCompressorInstructions); }
	public string GetHumidifierInstructions() { return GetFormatedText(HumidifierInstructions); }
	public string GetNitrogenInstructions() { return GetFormatedText(NitrogenInstructions); }
	public string GetFanInstructions() { return GetFormatedText(FanInstructions); }
	public string GetWaterInstructions() { return GetFormatedText(WaterInstructions); }
	public string GetRadiatorInstructions() { return GetFormatedText(RadiatorInstructions); }

	private string GetFormatedText(string text)
	{
		// can be optimised by looking at balises instead
		string newText = text;
		foreach (TextTag tag in tags)
			newText = newText.Replace(tag.sequence, "<color=#" + ColorUtility.ToHtmlStringRGB(tag.color) + ">" + tag.replaceWith + "</color>");
		return newText;
	}

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

	private void Awake()
	{
		System.Array.Sort(tags, (TextTag a,TextTag b) => Mathf.Clamp(a.replaceWith.Length - b.replaceWith.Length, -1, 1));
	}
}
