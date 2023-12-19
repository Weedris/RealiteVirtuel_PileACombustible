using UnityEngine;

[CreateAssetMenu(menuName = "ScriptableObjects/Settings")]
public class Settings : ScriptableObject
{
    public static Settings Instance
    {
        get
        {
            instance ??= new Settings();
            return instance;
        }
    }

    private static Settings instance;

    public Language curentLanguage;
	public Platform platform;
	private bool pass_assembly;

    public void set_pass_assembly(bool pass)
    {
        pass_assembly=pass;
    }
    public bool get_pass_assembly()
    {
        return pass_assembly;
    }

}


public enum Platform
{
	PC,
	VR
}