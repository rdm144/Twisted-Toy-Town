using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Battle_Animator : MonoBehaviour
{
    public Animator anim;

    // Start is called before the first frame update
    void Start()
    {
        anim = GetComponentInChildren<Animator>();
    }

    public void PlayShootAnimation()
    {
        ResetAllTriggers();
        anim.SetTrigger("Shoot");
    }

    public void PlayHitReelAnimation()
    {
        ResetAllTriggers();
        anim.SetTrigger("HitReel");
    }

    public void PlayHealAnimation()
    {
        ResetAllTriggers();
        anim.SetTrigger("Heal");
    }

    public void PlayRunAwayAnimation()
    {
        ResetAllTriggers();
        anim.SetTrigger("RunAway");
    }

    public void PlayVictoryAnimation()
    {
        ResetAllTriggers();
        anim.SetTrigger("Victory");
    }

    public void PlayDeathAnimation()
    {
        ResetAllTriggers();
        anim.SetTrigger("Die");
    }

    private void ResetAllTriggers()
    {
        foreach (var param in anim.parameters)
        {
            if (param.type == AnimatorControllerParameterType.Trigger)
            {
                anim.ResetTrigger(param.name);
            }
        }
    }
}
