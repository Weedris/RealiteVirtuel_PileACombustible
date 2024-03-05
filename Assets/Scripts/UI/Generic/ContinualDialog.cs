using System;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

/// <summary>
/// Perfect for "press next button 2k times" kind of dialogs.
/// </summary>
public abstract class ContinualDialog : LangUpdatable, IEndable
{
	public event Action OnDialogEnd;
	[SerializeField] protected TMP_Text message;
	[SerializeField] protected Button nextButton;
	[SerializeField] protected TMP_Text nextButtonText;

	protected string[] messages;
	protected int index = 0;

	protected void ShowCurrentMessage()
	{
		message.text = messages[index];
	}

	/// <summary>
	/// Attempt to load the next message.
	/// If there's no more message to be shown, trigger the OnDialgEnd event instead.
	/// </summary>
	private void NextMessage()
	{
		index++;
		if (index < messages.Length)
			ShowCurrentMessage();
		else
			OnDialogEnd?.Invoke();
	}

	private void OnEnable()
	{
		nextButton.onClick.AddListener(NextMessage);
	}

	private void OnDisable()
	{
		nextButton.onClick.RemoveListener(NextMessage);
	}

	private new void OnDestroy()
	{
		base.OnDestroy();
		foreach (Delegate d in OnDialogEnd.GetInvocationList())
			OnDialogEnd -= (Action)d;
	}
}
