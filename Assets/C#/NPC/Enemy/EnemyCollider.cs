using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyCollider : MonoBehaviour
{
    NPC myNPC;
    Actor myActor;
    bool hasInteracted;

    // Start is called before the first frame update
    void Start()
    {
        myNPC = transform.parent.GetComponent<NPC>();
        myActor = transform.parent.GetComponent<Actor>();
        hasInteracted = false;
    }

    private void OnTriggerEnter(Collider collision)
    {
        if (collision.transform.parent.tag.Equals("Player") && myActor.canOperate)
        {
            if (hasInteracted == false)
            {
                hasInteracted = true;
                myNPC.Interact();
            }
        }
    }
}
