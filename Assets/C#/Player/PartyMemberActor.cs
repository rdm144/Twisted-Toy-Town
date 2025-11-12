using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

public class PartyMemberActor : Actor
{
    public static GameObject partyMember1;

    private void Awake()
    {
        if (partyMember1 == null)
        {
            partyMember1 = gameObject;
            //DontDestroyOnLoad(gameObject);
        }
        else
        {
            Destroy(gameObject);
        }
    }

    // Start is called before the first frame update
    void Start()
    {
        ActorStart();
    }
}
