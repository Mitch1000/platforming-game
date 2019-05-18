using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public class RunningHandler
{
    private float lerpTime = 0.0f;
    private float lerpTime2 = 0.0f;
    private bool isRunning = false;
    private bool wasRunningPrevious = false;
    // must be inline with animator blend tree
    private readonly float runToWalkTransitionThreshold = 0.2f;
    private readonly float runToWalkTransitionSpeed = 0.05f;

    // Gets a value that transistions from 0.5 to 1 or 1 to 0.5 
    // to smoothly transition the player between running and walking
    public float GetIsRunningModifier(float running)
    {
        wasRunningPrevious = isRunning;
        isRunning = running > 0.5f;
        bool wasRunningPressed = wasRunningPrevious != isRunning;

        float runningModifier = isRunning ? Mathf.Lerp(1f, runToWalkTransitionThreshold, lerpTime2) : Mathf.Lerp(runToWalkTransitionThreshold, 1f, lerpTime);

        // prevent lerpTimes from being added to infinitely
        if (!isRunning && lerpTime <= 1.0f)
        {
            lerpTime += runToWalkTransitionSpeed;
        }

        if (isRunning && lerpTime2 <= 1.0f)
        {
            lerpTime2 += runToWalkTransitionSpeed;
        }

        if (wasRunningPressed)
        {
            lerpTime = 0.0f;
            lerpTime2 = 0.0f;
        }

        return runningModifier;
    }
}
