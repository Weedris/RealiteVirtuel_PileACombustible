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
    private Slider IntensitySlider;
    [SerializeField]
    private Slider HydrogenLevel;
    private float hydrogen;
    private float intensite;
    private float A;

    [SerializeField]
    private GameObject finalScoreCanvas;
    [SerializeField]
    private TextMeshProUGUI finalScore;
    private float totalTime;
    private float bestTimeLaps;
    private int totalLaps;

    private float initialHydrogenValue;
    private float initialIntensityValue;

    private const float ZeroTolerance = 0.0001f; // Tolerance for zero

    void Start()
    {
        Animator = GetComponent<Animator>();
        if (Animator == null) throw new Exception("Animator null!");
        if (IntensitySlider == null) throw new Exception("IntensitySlider null!");
        if (HydrogenLevel == null) throw new Exception("HydrogenLevel null!");
        finalScoreCanvas.SetActive(false);
        intensite = IntensitySlider.value;
        initialIntensityValue = IntensitySlider.value;
        initialHydrogenValue = HydrogenLevel.value;
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
        IntensitySlider.value = initialIntensityValue;
        HydrogenLevel.value = initialHydrogenValue;
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
            hydrogen = HydrogenLevel.value;

            float tension = PilotagePile.roundUp(Mathf.Lerp(18 - (0.6f * A), 15.2f, A));
            float speedMultiplier = Mathf.Lerp(0.15f, 0.5f, PilotagePile.roundUp(tension * intensite));
            float rndmnt = PilotagePile.roundUp(Mathf.Lerp(60 - (2.1f * A), 50.7f, A)) / 1000;

            float adjustedSpeed = speedMultiplier - (speedMultiplier * Mathf.Min(rndmnt, hydrogen));
            SetCarSpeed(adjustedSpeed);

            HydrogenLevel.value = Mathf.Max(0, hydrogen - rndmnt);

            HydrogenLevel.GetComponentsInChildren<TextMeshProUGUI>()[0].text = HydrogenLevel.value.ToString("F2");

            if (HydrogenLevel.value < ZeroTolerance) PauseAnimator(true);
        }
    }

    public void changeIntensite()
    {
        intensite = IntensitySlider.value;
        IntensitySlider.GetComponentsInChildren<TextMeshProUGUI>()[0].text = intensite.ToString().Substring(0, 4);
        A = Mathf.Clamp((intensite - 20) / 39.8f, 0f, 1f);
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
