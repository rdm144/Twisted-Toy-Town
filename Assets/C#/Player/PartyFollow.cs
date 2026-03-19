using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using static UnityEditor.PlayerSettings;

public class PartyFollow : MonoBehaviour
{
    public Transform targetToFollow;
    Actor myActor;
    public Vector3Int nextPosition;
    public Quaternion nextRotation;
    public float speed = 7;
    Actor targetActor;

    // Start is called before the first frame update
    void Start()
    {
        myActor = GetComponent<Actor>();
        targetActor = targetToFollow.GetComponent<Actor>();
        if (targetToFollow != null)
        {
            PlayerMovement.moveToNextPosition += StartMoving;
            ResetPositionToPartyLeader();
        }

        Rotate();
    }

    public void ResetPositionToPartyLeader()
    {
        nextPosition = Vector3Int.RoundToInt(targetToFollow.position);
        nextRotation = targetToFollow.rotation;
        transform.position = nextPosition;
    }

    private void Update()
    {
        if (myActor.canOperate == true)
        {
            if((transform.position - nextPosition).sqrMagnitude > 0.0001f || targetActor.isMoving)
                myActor.isMoving = true;
            else 
                myActor.isMoving = false;
        }
        else 
        {
            myActor.isMoving = false;
        }
    }

    private void FixedUpdate()
    {
        if(myActor.canOperate == true && (transform.position - nextPosition).sqrMagnitude > 0.0001f)
        {
            MoveToDestination(nextPosition);
        }
    }

    void StartMoving()
    {
        if (targetToFollow != null)
        {
            nextPosition = Vector3Int.RoundToInt(targetToFollow.position);
            nextRotation = targetToFollow.rotation;
        }

        Rotate();
    }

    void MoveToDestination(Vector3 destination)
    {
        transform.position = Vector3.MoveTowards(transform.position, destination, speed * Time.deltaTime);
    }

    void Rotate()
    {
        Vector3Int directionVector = Vector3Int.RoundToInt(nextPosition - transform.position);
        if (directionVector != Vector3.zero)
            myActor.Direction = directionVector;

        if (myActor.Direction != Vector3Int.zero)
        {
            transform.LookAt(transform.position + myActor.Direction);
        }
    }

    private void OnDestroy()
    {
        PlayerMovement.moveToNextPosition -= StartMoving;
    }
}
