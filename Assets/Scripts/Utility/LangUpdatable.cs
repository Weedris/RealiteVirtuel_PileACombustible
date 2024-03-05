using UnityEngine;

/// <summary>
/// Component for GameObjects which hold text that needs to be updated according to language selection.
/// </summary>
public abstract class LangUpdatable : MonoBehaviour
{
	protected void Start()
	{
		LanguageManager.Instance.RegisterUpdatable(this);
	}

	protected void OnDestroy()
	{
		LanguageManager.Instance.ForgetUpdatable(this);
	}

	/// <summary>
	/// Called automatically when the language is changed and on creation.
	/// </summary>
	/// <param name="lang">The new translation.</param>
	public abstract void OnLangUpdated(Translation lang);
}
