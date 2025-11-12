using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AnimatePlayer : MonoBehaviour
{
    Animator anim;
    PartyLeaderActor pla;

    // Start is called before the first frame update
    void Start()
    {
        anim = GetComponentInChildren<Animator>();
        pla = GetComponent<PartyLeaderActor>();
    }

    // Update is called once per frame
    void Update()
    {
        anim.SetBool("IsRunning", pla.isMoving);
    }
}
