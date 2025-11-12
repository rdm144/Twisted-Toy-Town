using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Playables; // Required for Timeline

public class RetrySceneManager : MonoBehaviour
{
    // --- Public References (Drag and Drop in Inspector) ---
    [Header("Characters and Animators")]
    public Animator mainCharacterAnimator;      // The character who jumps/runs
    public Animator otherCharacterAnimator;     // The character who runs away

    [Header("Timeline Sequence")]
    public PlayableDirector retryTimelineDirector; // The Timeline Asset

    // --- State Control ---
    [Header("Animation Names")]
    public string mainIdleAnim = "Main_Pout_Loop";
    public string otherIdleAnim = "Other_Laugh_Loop";

    // The Boolean: true = Retry, false = Idle
    private bool isRetryActive = false;

    void Start()
    {
        // Start the scene in the Idle state
        SetSceneState(false);
        // NOTE: The SpotlightFollower script on the target object should be left ON.
    }

    // --- Public Function to be called when the player presses the 'Retry' button ---
    public void OnPlayerPressedRetry()
    {
        // Only switch if we are currently in the Idle state
        if (!isRetryActive)
        {
            SetSceneState(true); // Switch to the Retry state
        }
    }

    // --- Core Logic Function ---
    private void SetSceneState(bool activateRetry)
    {
        isRetryActive = activateRetry;

        if (isRetryActive)
        {
            // --- RETRY STATE (Sequence) ---

            // 1. Disable individual Animators (Timeline takes over their control)
            if (mainCharacterAnimator != null) mainCharacterAnimator.enabled = false;
            if (otherCharacterAnimator != null) otherCharacterAnimator.enabled = false;

            // 2. Start the Timeline (This orchestrates the light and character animations)
            if (retryTimelineDirector != null)
            {
                retryTimelineDirector.Play();
            }
        }
        else // !isRetryActive
        {
            // --- IDLE STATE (Looping) ---

            // 1. Stop the Timeline (if it was somehow still running)
            if (retryTimelineDirector != null)
            {
                retryTimelineDirector.Stop();
            }

            // 2. Enable individual Animators
            if (mainCharacterAnimator != null) mainCharacterAnimator.enabled = true;
            if (otherCharacterAnimator != null) otherCharacterAnimator.enabled = true;

            // 3. Play the looping idle animations
            if (mainCharacterAnimator != null) mainCharacterAnimator.Play(mainIdleAnim);
            if (otherCharacterAnimator != null) otherCharacterAnimator.Play(otherIdleAnim);
        }
    }

    // --- Timeline Completion Logic (Reverts to Idle) ---
    // This function must be connected to the PlayableDirector's 'Stopped' event in the Inspector.
    public void OnTimelineFinished()
    {
        // Only go back to idle if we were in the retry state
        if (isRetryActive)
        {
            SetSceneState(false);
        }
    }
}