using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerAnimator : MonoBehaviour
{
    public Animator anim;
    Actor myActor;

    // Start is called before the first frame update
    void Start()
    {
        myActor = GetComponent<Actor>();
    }

    // Update is called once per frame
    void FixedUpdate()
    {
        if(anim != null)
        {
            anim.SetInteger("xDir", (int)myActor.Direction.x);
            anim.SetInteger("yDir", (int)myActor.Direction.y);
            if(myActor.isMoving == true)
            {
                anim.SetLayerWeight(0, 0);
                anim.SetLayerWeight(1, 1);
            }
            else
            {
                anim.SetLayerWeight(0, 1);
                anim.SetLayerWeight(1, 0);
            }
            Rotate();
        }
    }

    void Rotate()
    {
        if (myActor.Direction.x != 0)
        {
            transform.LookAt(transform.position + Vector3.forward * myActor.Direction.x);
        }
    }
}
