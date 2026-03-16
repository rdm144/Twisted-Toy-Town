using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using Unity.VisualScripting;

public class PlayerMovement : MonoBehaviour
{
    public LayerMask groundMask;
    public float speed = 7f;
    public int gridCellSize = 1;
    public bool canRun;
    public bool canInput = true;
    public bool canRotate;
    bool isDestinationValid;
    public int moveHorizontal { get; private set; }
    public int moveVertical { get; private set; }
    Actor myActor;

    KeyCode LeftKey, RightKey, UpKey, DownKey;
    public Vector3Int targetDestination;

    public static event Action moveToNextPosition;

    // Start is called before the first frame update
    void Start()
    {
        if(transform.position.x % gridCellSize != 0 || transform.position.y % gridCellSize != 0)
        {
            transform.position = new Vector3(RoundToNearest(transform.position.x, gridCellSize), RoundToNearest(transform.position.y, gridCellSize), 0);
        }
        LeftKey = KeyCode.A; // Hard-coded keybinds. Remove later.
        RightKey = KeyCode.D;
        UpKey = KeyCode.W;
        DownKey = KeyCode.S;
        canRun = canRotate = true;
        myActor = GetComponent<Actor>();
        myActor.isMoving = false;
        isDestinationValid = false;
        targetDestination = Vector3Int.RoundToInt(transform.position);

        if (canRotate)
            Rotate();
    }

    private void Update()
    {
        if (canInput)
            GetInput();

        if (canRun && myActor.canOperate)
        {
            if ((transform.position - targetDestination).sqrMagnitude > 0.0001f || ((moveHorizontal != 0 || moveVertical != 0) && isDestinationValid))
            {
                myActor.isMoving = true;
            }
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
        if (canRun && myActor.canOperate)
        {
            if ((moveHorizontal != 0 || moveVertical != 0) && (transform.position - targetDestination).sqrMagnitude <= 0.0001f) // Press a direction while not moving
            {
                // Calculate our desired destination
                Vector3Int desiredDestination = GetDestination();

                // Get new direction vector
                Vector3Int directionVector = new Vector3Int(moveHorizontal, 0, moveVertical);

                // Set as new direction and rotate
                if (myActor.Direction != directionVector && directionVector != Vector3.zero)
                {
                    myActor.Direction = directionVector;
                    if (canRotate)
                        Rotate();
                }

                isDestinationValid = IsDestinationValid(desiredDestination);

                // Check if the desired destination is reachable
                if (isDestinationValid == true)
                {
                    // Set new destination
                    targetDestination = desiredDestination;

                    // Invoke an action event to tell party members to move to their next destination
                    if (moveToNextPosition != null)
                        moveToNextPosition.Invoke();
                }
            }

            if ((transform.position - targetDestination).sqrMagnitude > 0.0001f)
            {
                // Move to our new destination
                MoveToDestination(targetDestination);
            }
        }
        else
        {
            targetDestination = Vector3Int.RoundToInt(transform.position);
        }
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

    Vector3Int GetDestination()
    {
        int Z = (int)transform.position.z;
        int X = (int)transform.position.x;

        // Calculate X-Axis
        X += gridCellSize * (int)moveHorizontal;

        // Calculate Y-Axis
        Z += gridCellSize * (int)moveVertical;

        Vector3Int desiredDestination = new Vector3Int(X, (int)transform.position.y, Z);
        //Debug.Log(desiredDestination);

        return desiredDestination;
    }

    bool IsDestinationValid(Vector3 destination)
    {
        // Destination cannot be on top of the player
        if (Vector3.Distance(transform.position, destination) <= 0.05f)
            return false;

        // Ensure destination is not out of bounds. Ground composite colliders must be set to "Polygon"
        //if(Physics.OverlapSphere(destination, 0.1f, groundMask).Length > 0)
        if(Physics.CheckSphere(destination, 0.1f, groundMask) == true)
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
        if (Vector3.Distance(transform.position, targetDestination) <= 0.05f)
        {
            transform.position = targetDestination;
        }
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
