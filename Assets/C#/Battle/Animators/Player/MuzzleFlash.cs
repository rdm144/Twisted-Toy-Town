using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MuzzleFlash : MonoBehaviour
{
    public Animator flashAnim;

    public void PlayFlash()
    {
        if (flashAnim != null)
        {
            flashAnim.SetTrigger("Flash");
        }
    }
}
