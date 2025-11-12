using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AnimatePartyMember : MonoBehaviour
{
    Animator anim;
    PartyMemberActor pma;

    // Start is called before the first frame update
    void Start()
    {
        anim = GetComponentInChildren<Animator>();
        pma = GetComponent<PartyMemberActor>();
    }

    // Update is called once per frame
    void Update()
    {
        anim.SetBool("IsRunning", pma.isMoving);
    }
}
