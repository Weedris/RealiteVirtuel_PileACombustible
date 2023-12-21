using System;
using System.Text;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class CarManager : MonoBehaviour
{
	private Animator Animator;
	private int lastPlayedClipHashStart, lastPlayedClipHashEnd;
	private DateTime lapsTime;

	[SerializeField] private GameObject finalScoreCanvas;
	[SerializeField] private TextMeshProUGUI finalScore;
	[SerializeField] private Button _startButton;

	private DateTime totalTime;
	private TimeSpan bestTimeLaps;
	private int totalLaps;

	private const float ZeroTolerance = 0.0001f; // Tolerance for zero

	void Start()
	{
		if (!TryGetComponent(out Animator))
			throw new Exception("Animator null!");
		finalScoreCanvas.SetActive(false);
		ResetStats();
		PauseAnimator();
		InvokeRepeating(nameof(UpdateCar), 0.0f, 0.1f);
		_startButton.onClick.AddListener(delegate { StartCar(); });
	}

	void Update()
	{
		if (AnimatorIsPlaying())
		{
			AnimatorStateInfo stateInfo = Animator.GetCurrentAnimatorStateInfo(0);
			if (stateInfo.normalizedTime <= 0.05f && lastPlayedClipHashStart != stateInfo.shortNameHash)
			{
				lastPlayedClipHashStart = stateInfo.shortNameHash;
				OnAnimationStart();
			}
			if (stateInfo.normalizedTime >= 1.0f && lastPlayedClipHashEnd != stateInfo.shortNameHash)
			{
				lastPlayedClipHashEnd = stateInfo.shortNameHash;
				OnAnimationEnd();
			}
		}
	}

	private void ResetStats()
	{
		totalTime = DateTime.MinValue;
		bestTimeLaps = TimeSpan.MaxValue;
		totalLaps = 0;
	}

	public void ResetCarManager()
	{
		ResetStats();
	}

	public void SetHydrogenAndResetIntensity(float hydrogenValue)
	{
		GaugeManager.Instance.ResetValues(hydrogenValue);
	}

	void SetCarSpeed(float speed)
	{
		Animator.SetFloat("Speed", speed);
	}

	bool AnimatorIsPlaying()
	{
		return Animator.speed > 0;
	}

	void PauseAnimator(bool outOfHydrogen = false)
	{
		Animator.speed = 0f;
		GaugeManager.Instance.isHydrogenConsumed = false;
		if (outOfHydrogen)
		{
			AddLapsTime();
			finalScoreCanvas.SetActive(true);
			StringBuilder sb = new StringBuilder();
			sb.AppendLine("Total time : " + FormatTime(totalTime))
				.AppendLine("Total laps : " + totalLaps)
				.AppendLine("Best Time Laps : " + FormatTime(bestTimeLaps));
			finalScore.text = sb.ToString();
		}
	}

	public void StartCar()
	{
		ResumeAnimator();
		_startButton.gameObject.SetActive(false);
	}

	public void ResumeAnimator()
	{
		GaugeManager.Instance.isHydrogenConsumed = true;
		Animator.speed = 1f;
	}

	private void UpdateCar()
	{
		if (AnimatorIsPlaying())
		{
			GaugeManager.Instance.ConsumeHydrogen();

			float hydrogen = GaugeManager.Instance.Hydrogen;
			float speedMultiplier = Mathf.Lerp(0.15f, 0.5f, GaugeManager.Instance.Power / 3039.276f);
			float efficiency = GaugeManager.Instance.Efficiency / 100;

			float adjustedSpeed = speedMultiplier - (speedMultiplier * Mathf.Min(efficiency, hydrogen));
			SetCarSpeed(adjustedSpeed);

			if (GaugeManager.Instance.Hydrogen < ZeroTolerance)
				PauseAnimator(true);
		}
	}

	void OnAnimationStart()
	{
		lapsTime = DateTime.Now;
	}

	void OnAnimationEnd()
	{
		AddLapsTime(true);
		totalLaps++;
	}

	void AddLapsTime(bool lapsFined = false)
	{
		TimeSpan elapsedTime = DateTime.Now - lapsTime;
		if ((elapsedTime < bestTimeLaps) && lapsFined)
		{
			bestTimeLaps = elapsedTime;
		}
		totalTime += elapsedTime;
	}

	string FormatTime(TimeSpan time)
	{
		return $"{time.Minutes:D2}:{time.Seconds:D2}:{time.Milliseconds:D3}";
	}

	string FormatTime(DateTime time)
	{
		return $"{time.Minute:D2}:{time.Second:D2}:{time.Millisecond:D3}";
	}
}
