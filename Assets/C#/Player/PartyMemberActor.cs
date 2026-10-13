using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

public class PartyMemberActor : Actor
{
    public static List<PartyMemberActor> partyMembers;
    public int partyMemberNumber;

    private void Awake()
    {
        // Crate a list of followers, if none exists, and add this object to it
       if (partyMembers == null)
        {
            partyMembers = new List<PartyMemberActor>
            {
                gameObject.GetComponent<PartyMemberActor>()
            };
        }
        else // If this object's party member number already exists in the list, destroy this object
        {
            foreach (PartyMemberActor actor in partyMembers)
            {
                if(actor.partyMemberNumber == partyMemberNumber)
                    Destroy(gameObject);
            }

            // If no duplicates are found, add this object to the list
            partyMembers.Add(GetComponent<PartyMemberActor>());
        }

        // Keep this object persistant across different scenes
        DontDestroyOnLoad(gameObject);
    }

    // Start is called before the first frame update
    void Start()
    {
        ActorStart();
    }
}
