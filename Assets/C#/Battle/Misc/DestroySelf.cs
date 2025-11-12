using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DestroySelf : MonoBehaviour
{
    public void DestroyThis()
    {
        Destroy(gameObject.transform.root.gameObject);
    }
}
