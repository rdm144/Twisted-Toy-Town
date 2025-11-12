using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Actor : MonoBehaviour
{
    public Vector3Int Direction;
    public bool isMoving;
    public bool canOperate;

    // Start is called before the first frame update
    void Start()
    {
        isMoving = false;
        if(Direction != Vector3Int.back && Direction != Vector3Int.forward && Direction != Vector3Int.left && Direction != Vector3Int.right)
            Direction = Vector3Int.back;
    }

    protected void ActorStart()
    {
        this.Start();
    }
}
