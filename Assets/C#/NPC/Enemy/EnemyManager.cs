using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyManager : MonoBehaviour
{
    public static EnemyManager instance;
    public bool turnOffEnemies;
    public Actor[] enemyActors { get; private set; }
    StageInfo stageInfo;
    public int stunFrameCount;

    // Start is called before the first frame update
    void Awake()
    {
        if (instance == null)
            instance = this;
        else
            Destroy(gameObject);

        GameObject stageInfoGameObject = GameObject.FindGameObjectWithTag("Stage Info");
        if (stageInfoGameObject != null)
            stageInfoGameObject.TryGetComponent(out stageInfo);
        enemyActors = transform.GetComponentsInChildren<Actor>();
    }

    private void Start()
    {
        if (turnOffEnemies)
            StopAllEnemyActors();

        if(stageInfo != null)
        {
            PlaceAllEnemyActors();

            // Check dead actor list
            foreach (int index in stageInfo.graveyard)
            {
                enemyActors[index].gameObject.SetActive(false);
            }

            // Check stunned actor index
            if (stageInfo.stunIndex > -1)
            {
                PauseEnemyActor(enemyActors[stageInfo.stunIndex]);
            }
        }
    }

    public void StopAllEnemyActors()
    {
        foreach(Actor actor in enemyActors)
        {
            actor.canOperate = false;
            actor.isMoving = false;
        }
    }

    // Called from the battle scene
    public void PauseEnemyActor(Actor enemyActor)
    {
        enemyActor.canOperate = false;
        enemyActor.isMoving = false;
        StartCoroutine(PauseEnemy(enemyActor));
    }

    IEnumerator PauseEnemy(Actor enemyActor)
    {
        for(int i = 0; i < stunFrameCount; i++)
            yield return new WaitForFixedUpdate();

        enemyActor.canOperate = true;
        enemyActor.isMoving = true;
    }

    public void StoreAllEnemyPositions()
    {
        foreach(Actor actor in enemyActors)
        {
            stageInfo.enemyPositions.Add(stageInfo.ConvertToEnemyPosition(actor.transform.position));
        }
    }

    void PlaceAllEnemyActors()
    {
        if(stageInfo.enemyPositions.Count > 0)
        {
            for (int i = 0; i < stageInfo.enemyPositions.Count; i++)
            {
                //Debug.Log("Placing " + enemyActors[i].name + " at position: " + stageInfo.ConvertToVector3(stageInfo.enemyPositions[i]).ToString());
                enemyActors[i].transform.position = stageInfo.ConvertToVector3(stageInfo.enemyPositions[i]);
            }
        }

        stageInfo.enemyPositions.Clear();
    }    
}
