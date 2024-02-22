using System;
using UnityEngine;
using UnityEngine.UI;

namespace Assets.Scripts.UI
{
	public class LanguageSelector : MonoBehaviour
	{
		[Serializable]
		private struct ButtonLangTuple
		{
			public Button button;
			public Language lang;
		}
		[SerializeField] private ButtonLangTuple[] buttons;

		private void OnEnable()
		{
			// add buttons events
			foreach (ButtonLangTuple tuple in buttons)
			{
				tuple.button.onClick.AddListener(() =>
				{
					LanguageManager.Instance.SwitchLanguage(tuple.lang);
					Destroy(gameObject);
				});
			}
		}

		private void OnDisable()
		{
			foreach (ButtonLangTuple tuple in buttons)
				tuple.button.onClick.RemoveAllListeners();
		}
	}
}
