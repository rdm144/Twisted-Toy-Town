using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class NPC : MonoBehaviour
{
    protected static float gridSize = 1;
    protected Vector3 targetDestination { get; private set; }
    public LayerMask groundMask;
    protected bool isRunning;
    protected bool isInteracting;
    protected static Dialog_UI dialogUI;

    // Start is called before the first frame update
    void Start()
    {
        if (dialogUI == null)
            dialogUI = GameObject.FindGameObjectWithTag("Dialog_UI").GetComponent<Dialog_UI>();
    }

    protected void NPCStart()
    {
        Start();
    }

    protected virtual void SetDestination(Vector3 destination)
    {
        targetDestination = destination;
    }

    protected virtual bool IsDestinationValid(Vector3 destination)
    {
        // Destination cannot be on top of the object
        if (Vector3.Distance(transform.position, destination) <= 0.05f)
            return false;

        // Ensure destination is not out of bounds
        if (Physics.Raycast(transform.position, (destination - transform.position).normalized, gridSize, groundMask))
            return false;

        return true;
    }

    protected virtual void MoveToDestination(Vector3 destination)
    {
        transform.position = Vector3.MoveTowards(transform.position, destination, 1 * Time.deltaTime);
    }

    protected virtual void Rotate(Vector3 directionVector)
    {
        // Normalize the vector before using it
        directionVector = directionVector.normalized;

        // Rotate the object to face forward
        if (directionVector != Vector3.zero)
        {
            float Theta = Mathf.Atan2(directionVector.z, directionVector.x) * Mathf.Rad2Deg;

            Quaternion targetRotation = Quaternion.Euler(0, Theta, 0);
            transform.LookAt(transform.position + directionVector);
        }
    }

    Vector3 FixFloatingZeroCoordinates(Vector3 coordinates)
    {
        if (Mathf.Abs(coordinates.x) <= 0.05f)
            coordinates.x = 0.00f;
        if (Mathf.Abs(coordinates.z) <= 0.05f)
            coordinates.z = 0.00f;

        return coordinates;
    }

    public virtual void Interact()
    {
        Debug.Log("Interacted with " + gameObject.name);
    }
}
