using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class DamageNumber : MonoBehaviour
{
    // Called from the end of the damage number's animation
    public void DestroySelf()
    {
        Destroy(gameObject);
    }
}
