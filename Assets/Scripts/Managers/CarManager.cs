using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CarManager : MonoBehaviour
{
    private Animator animator;
    private int lastPlayedClipHashStart, lastPlayedClipHashEnd;
    private float lapsTime;
    private float totalTime;

    void Start()
    {
        animator = GetComponent<Animator>();
        if (animator == null) throw new System.Exception("Animator null!");
    }

    void Update()
    {
        AnimatorStateInfo stateInfo = animator.GetCurrentAnimatorStateInfo(0);
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

    void OnAnimationStart()
    {
        //Debug.Log("Reset laps time !");
        lapsTime = Time.time;
    }

    void OnAnimationEnd()
    {
        //Debug.Log("Clip end !");

        float elapsedTime = Time.time - lapsTime;
        totalTime += elapsedTime;

        Debug.Log("Elapsed time in clip : " + FormatTime(elapsedTime));
        Debug.Log("Total time : " + FormatTime(totalTime));
    }

    string FormatTime(float seconds)
    {
        int hours = Mathf.FloorToInt(seconds / 3600);
        int minutes = Mathf.FloorToInt((seconds % 3600) / 60);
        int remainingSeconds = Mathf.FloorToInt(seconds % 60);

        return string.Format("{0:D2}:{1:D2}:{2:D2}", hours, minutes, remainingSeconds);
    }
}
