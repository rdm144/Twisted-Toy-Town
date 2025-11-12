using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PartyFollow : MonoBehaviour
{
    public Transform targetToFollow;
    Actor myActor;
    public Vector3 nextPosition;
    public Quaternion nextRotation;
    public float speed = 7;
    
    bool hasWaitedOneFrame;
    // Start is called before the first frame update
    void Start()
    {
        myActor = GetComponent<Actor>();
        if (targetToFollow != null)
        {
            nextPosition = transform.position;
            nextRotation = transform.rotation;
            PlayerMovement.moveToNextPosition += StartMoving;
            transform.position = nextPosition;
        }
        hasWaitedOneFrame = false;
        Rotate();
    }

    private void FixedUpdate()
    {
        if(myActor.isMoving == true && myActor.canOperate == true)
        {
            if (transform.position != nextPosition) // have not reached destination
            {
                MoveToDestination(nextPosition);
                Rotate();
            }
            else if (hasWaitedOneFrame == false) // has reached destination, but has not waited a frame
            {
                hasWaitedOneFrame = true;
            }
            else // has reached destination, and has waited a frame
            {
                myActor.isMoving = false;
            }
        }
        if(myActor.canOperate == false)
        {
            nextPosition = transform.position; 
            nextRotation = transform.rotation;
            myActor.isMoving = false;
        }
        // Prevent clipping through floor
        if (transform.position.y < -1)
            transform.position = new Vector3(transform.position.x, transform.position.y, 0);
    }

    void StartMoving()
    {
        if (targetToFollow != null)
        {
            nextPosition = targetToFollow.position;
            nextRotation = targetToFollow.rotation;
        }

        Vector3 directionVector = (nextPosition - transform.position).normalized;
        if (directionVector != Vector3.zero)
            myActor.Direction = Vector3Int.RoundToInt(directionVector);

        myActor.isMoving = true;
        hasWaitedOneFrame = false;
    }

    void MoveToDestination(Vector3 destination)
    {
        transform.position = Vector3.MoveTowards(transform.position, destination, speed * Time.deltaTime);
    }

    void Rotate()
    {
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
