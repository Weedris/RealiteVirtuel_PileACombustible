using System;
using System.Collections;
using System.Collections.Generic;
using System.Text;
using TMPro;
using UnityEngine;
using UnityEngine.Device;
using UnityEngine.UI;
using static UnityEngine.Rendering.DebugUI;

public class CarManager : MonoBehaviour
{
    private Animator Animator;
    private int lastPlayedClipHashStart, lastPlayedClipHashEnd;
    private float lapsTime;

    [SerializeField]
    private GameObject finalScoreCanvas;
    [SerializeField]
    private TextMeshProUGUI finalScore;
    private float totalTime;
    private float bestTimeLaps;
    private int totalLaps;

    private const float ZeroTolerance = 0.0001f; // Tolerance for zero

    void Start()
    {
        Animator = GetComponent<Animator>();
        if (Animator == null) throw new Exception("Animator null!");
        finalScoreCanvas.SetActive(false);
        ResetStats();
        PauseAnimator();
        InvokeRepeating("UpdateCar", 0.0f, 0.1f);
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
        totalTime = 0;
        bestTimeLaps = -1;
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
        if (outOfHydrogen)
        {
            finalScoreCanvas.SetActive(true);
            StringBuilder sb = new StringBuilder();
            sb.AppendLine("Total time : " + FormatTime(totalTime))
                .AppendLine("Total laps : " + totalLaps)
                .AppendLine("Best Time Laps : " + FormatTime(bestTimeLaps));
            finalScore.text = sb.ToString();
        }
    }

    internal void ResumeAnimator()
    {
        Animator.speed = 1f;
    }

    private void UpdateCar()
    {
        if (AnimatorIsPlaying())
        {
            float hydrogen = GaugeManager.Instance.Hydrogen;

            float intensite = GaugeManager.Instance.Intensity;
            float tension = GaugeManager.Instance.StackVoltage;
            float speedMultiplier = Mathf.Lerp(0.15f, 0.5f, PilotagePile.roundUp(tension * intensite));
            float efficiency = GaugeManager.Instance.Efficiency / 100;

            float adjustedSpeed = speedMultiplier - (speedMultiplier * Mathf.Min(efficiency, hydrogen));
            SetCarSpeed(adjustedSpeed);

            GaugeManager.Instance.UpdateOutSliders();

            if (GaugeManager.Instance.Hydrogen < ZeroTolerance) PauseAnimator(true);
        }
    }

    void OnAnimationStart()
    {
        //Debug.Log("Reset laps time !");
        lapsTime = Time.time;
    }

    void OnAnimationEnd()
    {
        float elapsedTime = Time.time - lapsTime;
        if (bestTimeLaps == -1)
        {
            bestTimeLaps = elapsedTime;
        }
        else if (elapsedTime < bestTimeLaps)
        {
            bestTimeLaps = elapsedTime;
        }
        totalTime += elapsedTime;
        totalLaps++;
    }

    string FormatTime(float seconds)
    {
        int hours = Mathf.FloorToInt(seconds / 3600);
        int minutes = Mathf.FloorToInt((seconds % 3600) / 60);
        int remainingSeconds = Mathf.FloorToInt(seconds % 60);

        return string.Format("{0:D2}:{1:D2}:{2:D2}", hours, minutes, remainingSeconds);
    }
}
