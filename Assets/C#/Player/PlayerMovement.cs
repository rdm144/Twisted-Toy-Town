using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using Unity.VisualScripting;

public class PlayerMovement : MonoBehaviour
{
    public LayerMask groundMask;
    public float speed = 7f;
    public float gridSize = 1;
    public bool canRun;
    public bool canInput = true;
    public bool canRotate;
    public int moveHorizontal { get; private set; }
    public int moveVertical { get; private set; }
    Actor myActor;

    KeyCode LeftKey, RightKey, UpKey, DownKey;
    public Vector3 targetDestination;

    public static event Action moveToNextPosition;

    // Start is called before the first frame update
    void Start()
    {
        if(transform.position.x % gridSize != 0 || transform.position.y % gridSize != 0)
        {
            transform.position = new Vector3(RoundToNearest(transform.position.x, gridSize), RoundToNearest(transform.position.y, gridSize), 0);
        }
        LeftKey = KeyCode.A; // Hard-coded keybinds. Remove later.
        RightKey = KeyCode.D;
        UpKey = KeyCode.W;
        DownKey = KeyCode.S;
        canRun = canRotate = true;
        myActor = GetComponent<Actor>();
        myActor.isMoving = false;
        targetDestination = transform.position;

        if (canRotate)
            Rotate();
    }

    private void FixedUpdate()
    {
        if (canInput)
            GetInput();

        if (canRun && myActor.canOperate)
        {
            // Check if the player is at the target destination
            if (transform.position != targetDestination)
            {
                myActor.isMoving = true;

                // Move to our new destination
                MoveToDestination(targetDestination);

                if (canRotate)
                    Rotate();
            }
            else if(moveHorizontal == 0 && moveVertical == 0) // reached destination and not pressing a button
            {
                myActor.isMoving = false;
            }
            else if (moveHorizontal != 0 || moveVertical != 0) // reached destination and pressing a direction
            {
                Vector3 directionVector = new Vector3(moveHorizontal, 0, moveVertical).normalized;
                if (directionVector != Vector3.zero)
                    myActor.Direction = Vector3Int.RoundToInt(directionVector);

                // Calculate our desired destination
                Vector3 desiredDestination = GetDestination();

                // Check if the desired destination is reachable
                if (IsDestinationValid(desiredDestination) == true)
                {
                    // Set new destination
                    targetDestination = desiredDestination;

                    // Fixes zeros manifesting as extremely small floating values (ex: 1.54264e-08)
                    targetDestination = FixFloatingZeroCoordinates(targetDestination);

                    // Invoke an action event to tell party members to move to their next destination
                    if (moveToNextPosition != null)
                        moveToNextPosition.Invoke();
                }
                else
                {
                    myActor.isMoving = false;
                }
            }
            //else// if(moveHorizontal == 0 && moveVertical == 0)
                //myActor.isMoving = false;
        }
        else
        {
            targetDestination = transform.position;
            myActor.isMoving = false;
        }

        // Prevent clipping through floor
        if (transform.position.z < -1)
            transform.position = new Vector3(transform.position.x, 0, transform.position.z);
    }

    void GetInput()
    {
        // Left or Right input
        if (Input.GetKey(LeftKey) && !Input.GetKey(RightKey))
            moveHorizontal = -1;
        else if (Input.GetKey(RightKey) && !Input.GetKey(LeftKey))
            moveHorizontal = 1;
        else
            moveHorizontal = 0;

        // Up or Down input
        if (Input.GetKey(DownKey) && !Input.GetKey(UpKey))
            moveVertical = -1;
        else if (Input.GetKey(UpKey) && !Input.GetKey(DownKey))
            moveVertical = 1;
        else
            moveVertical = 0;

        // Prevent diagonals
        if (moveHorizontal != 0 && moveVertical != 0)
            moveVertical = 0;
    }

    Vector3 GetDestination()
    {
        float Z = transform.position.z;
        float X = transform.position.x;

        // Calculate X-Axis
        X += gridSize * moveHorizontal;

        // Calculate Y-Axis
        Z += gridSize * moveVertical;

        Vector3 desiredDestination = new Vector3(X, transform.position.y, Z);
        //Debug.Log(desiredDestination);

        return desiredDestination;
    }

    bool IsDestinationValid(Vector3 destination)
    {
        // Destination cannot be on top of the player
        if (Vector3.Distance(transform.position, destination) <= 0.05f)
            return false;

        // Ensure destination is not out of bounds. Ground composite colliders must be set to "Polygon"
        if(Physics.OverlapSphere(destination, 0.1f, groundMask).Length > 0)
            return false;

        return true;
    }

    Vector3 FixFloatingZeroCoordinates(Vector3 coordinates)
    {
        if (Mathf.Abs(coordinates.x) <= 0.05f)
            coordinates.x = 0.00f;
        if (Mathf.Abs(coordinates.z) <= 0.05f)
            coordinates.z = 0.00f;

        return coordinates;
    }

    void MoveToDestination(Vector3 destination)
    {
        transform.position = Vector3.MoveTowards(transform.position, destination, speed * Time.deltaTime);
        /*if (Vector3.Distance(transform.position, targetDestination) <= 0.05f)
        {
            transform.position = targetDestination;
        }*/
    }

    void Rotate()
    {
        if (myActor.Direction != Vector3Int.zero)
        {
            transform.LookAt(transform.position + myActor.Direction);
        }
    }

    float RoundToNearest(float value, float multiple)
    {
        return Mathf.Round(value / multiple) * multiple;
    }
}
