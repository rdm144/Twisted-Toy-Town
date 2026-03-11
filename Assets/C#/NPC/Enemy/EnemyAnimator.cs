using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyAnimator : MonoBehaviour
{
    Animator anim;
    Actor myActor;

    // Start is called before the first frame update
    void Start()
    {
        myActor = GetComponent<Actor>();
        anim = GetComponentInChildren<Animator>();
    }

    // Update is called once per frame
    void FixedUpdate()
    {
        if (anim != null)
        {
            anim.SetBool("Walk", myActor.isMoving);
        }
    }
}
