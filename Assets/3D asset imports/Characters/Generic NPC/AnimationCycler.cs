using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AnimationCycler : MonoBehaviour
{
    public Animator animator;
    public string[] animationNames; // List of animation state names
    private int currentIndex = 0;

    void Start()
    {
        // Ensure animator is assigned
        if (animator == null)
        {
            animator = GetComponent<Animator>();
        }

        // Check if an Animator Controller is assigned
        if (animator.runtimeAnimatorController == null)
        {
            Debug.LogError("Animator Controller is missing!");
            return;
        }
        /* this thing keeps clogging the console
        // Print all available animation states in the console
        foreach (var clip in animator.runtimeAnimatorController.animationClips)
        {
            Debug.Log("Available Animation State: " + clip.name);
        }
        */
        // Check if animation names are assigned
        if (animationNames == null || animationNames.Length == 0)
        {
            Debug.LogError("No animations assigned to the list.");
        }
    }

    void Update()
    {
        if (Input.GetKeyDown(KeyCode.UpArrow) && animationNames.Length > 0)
        {
            currentIndex = (currentIndex + 1) % animationNames.Length;

            // Check if the animation state exists before playing
            if (!animator.HasState(0, Animator.StringToHash(animationNames[currentIndex])))
            {
                Debug.LogError("Animation state not found: " + animationNames[currentIndex]);
                return;
            }

            // Play the animation and log it
            animator.Play(animationNames[currentIndex], 0, 0);
            Debug.Log("Playing: " + animationNames[currentIndex]);
        }
    }
}
