using System;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

/// <summary>
/// Perfect for "press next button 2k times" kind of dialogs.
/// </summary>
public class ContinualDialog : MonoBehaviour, IEndable
{
	public event Action OnDialogEnd;

	[SerializeField] private TMP_Text nextButtonLabel;
	[SerializeField] private TMP_Text message;
	[SerializeField] private Button nextButton;

	private string[] messages;
	private int index = 0;

	public void ReloadCurrentMessage()
	{
		if (index >= messages.Length) return; // foolproof
		message.text = messages[index];
	}

	public void SetMessages(string[] messages)
	{
		this.messages = messages;
	}

	public void SetNewtButtonText(string newtButtonText)
	{
		nextButtonLabel.text = newtButtonText;
	}

	/// <summary>
	/// Attempt to load the next message.
	/// If there's no more message to be shown, trigger the OnDialgEnd event instead.
	/// </summary>
	private void NextMessage()
	{
		index++;
		if (index < messages.Length)
			ReloadCurrentMessage();
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

	private void OnDestroy()
	{
		foreach (Delegate d in OnDialogEnd.GetInvocationList())
			OnDialogEnd -= (Action)d;
	}
}
