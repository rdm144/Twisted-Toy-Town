using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BattleCamera : MonoBehaviour
{
    private Vector3 initialPosition = new Vector3(-11.379f, 4.48f, -6.672f);
    private Vector3 initialRotation = new Vector3(18.316f, 62.855f, 0);

    // Start is called before the first frame update
    void Start()
    {
        if (initialPosition != null)
            transform.position = initialPosition;
        if (initialRotation != null)
            transform.rotation = Quaternion.Euler(initialRotation);
    }
}
