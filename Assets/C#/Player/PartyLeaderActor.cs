using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PartyLeaderActor : Actor
{
    public static GameObject partyLeader;
    StageInfo stageInfo;

    private void Awake()
    {
        if (partyLeader == null)
        {
            partyLeader = gameObject;
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
        MoveToSavedLocation();

        ActorStart();   
    }
    
    void MoveToSavedLocation()
    {
        GameObject stageInfoGameObject = GameObject.FindGameObjectWithTag("Stage Info");
        if (stageInfoGameObject != null)
            stageInfoGameObject.TryGetComponent(out stageInfo);

        // Get a list of party members
        GameObject[] partyMembers = GameObject.FindGameObjectsWithTag("Player");

        // Place each party member on top of the player
        foreach (GameObject partyMember in partyMembers)
            partyMember.transform.position = stageInfo.playerLocation;

        transform.position = stageInfo.playerLocation;
    }
}
