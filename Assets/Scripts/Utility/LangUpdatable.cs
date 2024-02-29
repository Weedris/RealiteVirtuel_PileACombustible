using UnityEngine;

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

	public abstract void UpdateLang(Translation lang);
}
