using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using System;
using NesScripts.Controls.PathFind;
using UnityEngine.UIElements;

public class Enemy : NPC
{
    public static event Action StartBattle;

    Actor myActor;
    GridManager gridManager;
    public LayerMask playerMask;
    public Transform target;
    public List<Vector3> path;
    public Vector3Int originalPosition;
    public Vector3 destination;
    public Vector3 finalDestination;

    public float wanderSpeed;
    public float chaseSpeed;
    public int aggroDistance;
    public int wanderDistance;
    public bool isChasing;
    public int partySize;
    public string battleLevelName;
    public Animator alertBubble;

    private void Awake()
    {
        originalPosition = Vector3Int.RoundToInt(transform.position);
    }

    private void Start()
    {
        partySize = UnityEngine.Random.Range(1, 5);
        
        if (wanderDistance <= 0)
            wanderDistance = 5;

        myActor = GetComponent<Actor>();

        gridManager = GameObject.FindGameObjectWithTag("GridManager").GetComponent<GridManager>();
        path = new List<Vector3>
        {
            transform.position
        };
        destination = path[0];
        finalDestination = destination;

        isChasing = false;
        if(aggroDistance <= 0)
            aggroDistance = 5;

        target = GameObject.FindGameObjectWithTag("Player").transform;

        StartCoroutine("AI");
    }

    IEnumerator AI()
    {
        while (true)
        {
            if (myActor.canOperate)
            {
                // Look for any targets to chase
                if (!isChasing)
                {
                    CheckLineOfSight();
                    
                    if(isChasing) // If a target is found, play the pre-chase telegraph animation
                    {
                        yield return TelegraphChase();
                    }
                }

                if (transform.position != destination) // if we are not at our destination, keep moving
                {
                    myActor.isMoving = true;
                    MoveTowardsDestination();
                }
                else if (destination != finalDestination) // We are at our destination
                {
                    if (path.Count > 0)
                    {
                        path.RemoveAt(0);
                        destination = path[0];
                    }
                    else
                        myActor.isMoving = false;
                }
                else
                    myActor.isMoving = false;

                if (isChasing == false) // If not chasing, then wander
                {
                    yield return WanderRoutine();
                }
                else if (target != null) // else, chase the target
                {
                    yield return ChaseRoutine();
                }
                else
                    isChasing = false; // If there is no target, then we cannot chase it
            }
            else
                myActor.isMoving = false;

            yield return new WaitForEndOfFrame();
        }
    }

    IEnumerator TelegraphChase()
    {
        if (alertBubble != null)
            alertBubble.SetBool("Alert", true);
        yield return new WaitForSeconds(1);
        /*
        Exclamation.enabled = true; // play exclamation mark animation
        Exclamation.Play("Exclamation_Mark", 0, 0);
        yield return new WaitForSeconds(Exclamation.GetCurrentAnimatorClipInfo(0)[0].clip.averageDuration / 2);
        Exclamation.enabled = false;
        Exclamation.gameObject.GetComponent<SpriteRenderer>().sprite = null;
        */
    }

    IEnumerator WanderRoutine()
    {
        if (alertBubble != null)
            alertBubble.SetBool("Alert", false);

        // Create a new destination to wander towards
        if (transform.position == destination)
        {
            Vector3 newDestination;

            // if too far from original position, walk back to it
            if(Vector3.Distance(transform.position, originalPosition) > wanderDistance)
                newDestination = originalPosition;
            else // else, pick a random direction
            {
                float rand = UnityEngine.Random.Range(1, 5);
                switch (rand)
                {
                    case 1:
                        newDestination = transform.position + Vector3.forward;
                        break;
                    case 2:
                        newDestination = transform.position + Vector3.back;
                        break;
                    case 3:
                        newDestination = transform.position + Vector3.right;
                        break;
                    case 4:
                        newDestination = transform.position + Vector3.left;
                        break;
                    default:
                        newDestination = transform.position + Vector3.forward;
                        break;
                }
            }

            // Calculate the grid-based path to the new destination
            CalculatePath(transform.position, newDestination);

            if (path.Count > 0)
            {
                destination = path[0];
                finalDestination = path[^1];
            }

            // Idle for a random number of frames
            for (int i = 0; i < UnityEngine.Random.Range(1, 600); i++)
            {
                CheckLineOfSight();
                if (isChasing)
                    break;
                myActor.isMoving = false;
                yield return new WaitForFixedUpdate();
            }
        }
    }

    IEnumerator ChaseRoutine()
    {
        Vector3 targetPosition = new Vector3(RoundToNearest(target.position.x, 1), transform.position.y, RoundToNearest(target.position.z, 1));
        if (targetPosition != finalDestination && transform.position == destination) // if target has moved and we are standing still, recalculate path
        {
            Vector3 myPosition = new Vector3(RoundToNearest(transform.position.x, 1), transform.position.y, RoundToNearest(transform.position.z, 1));
            CalculatePath(myPosition, targetPosition);

            if (path.Count > 0 && path.Count <= aggroDistance * 1.5f)
            {
                path.Insert(0, myPosition); // for edge cases when the path is no longer connected
                destination = path[0];
                finalDestination = path[^1];
            }
            else
                isChasing = false;
        }
        yield return null;
    }

    void CheckLineOfSight()
    {
        RaycastHit hit;
        Physics.Raycast(transform.position, myActor.Direction, out hit, aggroDistance, playerMask);

        Debug.DrawRay(transform.position, myActor.Direction * aggroDistance, Color.blue);

        if (hit.collider != null)
        {
            if (hit.transform.parent.tag.Equals("Player"))
            {
                isChasing = true;
                target = hit.transform.parent;
            }
        } 
    }

    void CalculatePath(Vector3 myPosition, Vector3 targetPosition)
    {
        Vector3Int start = Vector3Int.RoundToInt(myPosition);
        Vector3Int end = Vector3Int.RoundToInt(targetPosition);
        path.Clear();
        path = gridManager.GetPath(start, end);
    }

    void MoveTowardsDestination()
    {
        transform.position = Vector3.MoveTowards(transform.position, destination, (isChasing ? chaseSpeed : wanderSpeed) * Time.deltaTime);
        Vector3Int value = Vector3Int.RoundToInt((destination - transform.position).normalized);
        if (value != Vector3Int.zero)
        {
            myActor.Direction = value;
        }
        
        Rotate();
    }

    public override void Interact()
    {
        if(isInteracting == false)
        {
            myActor.canOperate = false;
            isInteracting = true;
            StartCoroutine("EnterBattle");
        }
    }

    IEnumerator EnterBattle()
    {
        AsyncOperation loadOperation = SceneManager.LoadSceneAsync(battleLevelName, LoadSceneMode.Single);
        loadOperation.allowSceneActivation = false;

        GameObject[] partyMembers = GameObject.FindGameObjectsWithTag("Player");

        // Ensure time is not stopped
        Time.timeScale = 1;

        // Start screen transition
        StartBattle?.Invoke();

        StageInfo si = GameObject.FindGameObjectWithTag("Stage Info").GetComponent<StageInfo>();

        // Tell stage info our child id
        EnemyManager em = transform.parent.GetComponent<EnemyManager>();
        
        for (int i = 0; i < em.enemyActors.Length; i++)
        {
            if (em.enemyActors[i] == myActor)
            {
                si.battleIndex = i;
                break;
            }
        }
        
        em.StoreAllEnemyPositions();

        // tell stage info the player's location
        foreach(GameObject partyMember in partyMembers)
        {
            if(partyMember.GetComponent<PartyLeaderActor>() != null)
            {
                si.SetPlayerRespawnLocation(partyMember.transform.position);
            }
        }

        // tell stage info the desired party structure
        
        string enemyName = "GrabbyHands";
        string[] party = new string[partySize];
        for(int i = 0; i < party.Length; i++)
            party[i] = enemyName;
        si.enemyParty = party;

        // Stop all party members from operating
        yield return FreezePartyMembers(partyMembers);

        // wait for screen transition
        yield return new WaitForSeconds(0.5F);

        // Wait for scene to load, if it is loading
        if (loadOperation != null) yield return WaitToLoad(loadOperation);

        // Allow next scene to fully load, if it is loaded
        if (loadOperation != null) loadOperation.allowSceneActivation = true;
    }

    IEnumerator FreezePartyMembers(GameObject[] partyMembers)
    {
        foreach (GameObject partyMember in partyMembers)
        {
            partyMember.GetComponent<Actor>().canOperate = false;
            partyMember.GetComponent<Actor>().isMoving = false;
            yield return new WaitForFixedUpdate();
        }
    }

    IEnumerator WaitToLoad(AsyncOperation loadOperation)
    {
        do
        {
            yield return new WaitForFixedUpdate();
        } while (loadOperation.progress < 0.9f);
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

    float FindAngle(Vector2 origin, Vector2 point)
    {
        float theta = 0;
        origin = origin.normalized;
        point = point.normalized;

        Mathf.Atan2(point.y - origin.y, point.x - origin.x);
        theta *= Mathf.Rad2Deg;

        return theta;
    }

    public void Deactivate() // called from enemy tracker
    {
        transform.parent.gameObject.SetActive(false);
    }
}
