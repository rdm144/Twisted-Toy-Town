using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraFollow : MonoBehaviour
{
    public Transform targetToFollow;
    public float LERP_MAGNITUDE = 0.1f;
    public bool CanFollow_X, CanFollow_Y;
    Vector3 offset = new Vector3(0, 5, -5);
    public Vector3 CameraTargetPosition { get; private set; }
    private Vector3 velocity = Vector3.zero;

    // Start is called before the first frame update
    void Start()
    {
        if(targetToFollow == null)
        {
            FindPartyLeader();
        }
            
        CanFollow_X = CanFollow_Y = true;
        GetTargetPosition();
    }

    // Update is called once per frame
    void LateUpdate()
    {
        if (targetToFollow != null)
        {
            GetTargetPosition();
            FollowPlayer(); // Attempt to follow the player
        }
        else
            FindPartyLeader();
    }

    private void FindPartyLeader()
    {
        GameObject[] partyMembers = GameObject.FindGameObjectsWithTag("Player");
        foreach (GameObject partyMember in partyMembers)
        {
            if (partyMember.GetComponent<PartyLeaderActor>() != null)
            {
                targetToFollow = partyMember.transform;
                break;
            }
        }
    }

    private void GetTargetPosition()
    {
        //CameraTargetPosition = SmoothFollow();
        //CameraTargetPosition = SmoothHorizontalLead();
        CameraTargetPosition = SimpleFollow();
        //CameraTargetPosition = TransformFollow();
    }

    private void FollowPlayer()
    {
        if (CanFollow_X) FollowHorizontal();
        if (CanFollow_Y) FollowVertical();
    }

    private void FollowHorizontal()
    {
        transform.position = new Vector3(CameraTargetPosition.x, transform.position.y, transform.position.z);
    }

    private void FollowVertical()
    {
        transform.position = new Vector3(transform.position.x, transform.position.y, CameraTargetPosition.z);
    }

    Vector3 SimpleFollow()
    {
        return targetToFollow.transform.position + offset;
    }


    Vector3 SmoothFollow()
    {
        return Vector3.Lerp(transform.position, targetToFollow.position + offset, Time.deltaTime * LERP_MAGNITUDE);
    }

    Vector3 TransformFollow()
    {
        return Vector3.MoveTowards(transform.position, targetToFollow.position + offset, Time.deltaTime * 5);
    }
}
