using System.Collections;
using System.Collections.Generic;
using UnityEngine;
/// <summary>
/// Spawn the muzzle flash for jimmy's attack
/// </summary>
public class StapleFlash : MonoBehaviour
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
