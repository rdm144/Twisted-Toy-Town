using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Player_Interact : MonoBehaviour
{
    KeyCode Interact;
    public LayerMask npcMask;
    Actor myActor;

    // Start is called before the first frame update
    void Start()
    {
        Interact = KeyCode.E;
        myActor = GetComponent<Actor>();
    }

    // Update is called once per frame
    void Update()
    {
        if(myActor.canOperate && GetInput())
        {
            Collider npc = GetNPCInFront();
            if (npc != null)
                InteractWithNPC(npc);
        }
    }

    bool GetInput()
    {
        return Input.GetKeyDown(Interact);
    }

    Collider GetNPCInFront()
    {
        Collider[] npcColliders = Physics.OverlapSphere(transform.position + myActor.Direction, 0.1f, npcMask);
        
        if (npcColliders.Length > 0)
            return npcColliders[0];
        else
            return null;
    }

    void InteractWithNPC(Collider npc_Collider)
    {
        Debug.Log(npc_Collider.transform.parent.name);
        npc_Collider.transform.parent.GetComponent<NPC>().Interact();
    }
}
