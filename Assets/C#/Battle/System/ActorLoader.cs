using System.Collections;
using System.Collections.Generic;
using System.Security.Cryptography;
using Unity.VisualScripting;
using UnityEngine;

public class ActorLoader : MonoBehaviour
{
    public float playerZoffset = 3.5f;
    public float enemyZoffset = 6;
    StageInfo info;

    private void Awake()
    {
        // Spawn in the player party
        SpawnThePlayersParty(); // The code for this function is wack, but i'm too dumb to fix it.

        SpawnEnemyParty();
    }

    /// <summary>
    /// Spawns players according to the StageInfo object, or the saved data, or creates dummy players if all else fails.
    /// This code fucking sucks, like holy shit these nested if's are cringe, but I'm even more cringe.
    /// </summary>
    void SpawnThePlayersParty()
    {
        // Check for the StageInfo object
        GameObject stageInfo = GameObject.FindGameObjectWithTag("Stage Info");

        if (stageInfo != null)
            stageInfo.TryGetComponent(out info);

        if (info != null)
        {
            if (info.partyMembers.Count > 0)
            {
                // spawn in the player party prefabs
                foreach(PartyMember member in info.partyMembers)
                {
                    SpawnPlayerPrefab(member, info.partyMembers.Count);
                }
            }
            else // if the stage info object does not have a party, use the save file
            {
                // load save file
                SaveDataManager.Load(SaveType.Player);

                if (SaveDataManager.playerdata.party.Count > 0)
                {
                    // spawn party prefabs from save file
                    foreach (PartyMember member in SaveDataManager.playerdata.party)
                    {
                        SpawnPlayerPrefab(member, SaveDataManager.playerdata.party.Count);
                    }
                }
                else
                {
                    // spawn in dummy players
                    for (int i = 0; i < 4; i++)
                    {
                        SpawnPlaceholderPlayerActor(i, 4);
                    }
                }
            }
        }
        else
        {
            // load save file
            SaveDataManager.Load(SaveType.Player);

            if (SaveDataManager.playerdata.party.Count > 0)
            {
                // spawn party prefabs from save file
                foreach (PartyMember member in SaveDataManager.playerdata.party)
                {
                    SpawnPlayerPrefab(member, SaveDataManager.playerdata.party.Count);
                }
            }
            else
            {
                // spawn in dummy players
                for (int i = 0; i < 4; i++)
                {
                    SpawnPlaceholderPlayerActor(i, 4);
                }
            }
        }
    }

    void SpawnPlayerPrefab(PartyMember member, int partySize)
    {
        Object obj = Resources.Load("Prefabs/Battle/Player/" + member.name);
        GameObject model = null;
        if (obj != null)
        {
            model = Instantiate((GameObject)obj);
        }

        // Spawn a placeholder if the prefab path is invalid
        if (model == null)
        {
            SpawnPlaceholderPlayerActor(member.partyIndex, partySize);
            Debug.LogWarning("Could not load prefab resource at Prefabs/Battle/Player/" + member.name);
        }
        else
        {
            model.name = member.name;
            Player_Battle_Actor modelActor = model.GetComponent<Player_Battle_Actor>();
            modelActor.maxHealth = member.maxHealth;
            modelActor.health = member.health;
            modelActor.isTakingTurn = false;
            modelActor.partyIndex = member.partyIndex;
            model.transform.position = new Vector3(-6, 1.5f, GetZ(partySize, member.partyIndex, playerZoffset, false));
        }
    }

    void SpawnPlaceholderPlayerActor(int partyIndex, int partySize)
    {
        GameObject placeHolder = GameObject.CreatePrimitive(PrimitiveType.Cube);
        Player_Battle_Actor newActor = placeHolder.AddComponent<Player_Battle_Actor>();
        newActor.gameObject.tag = "Player";
        newActor.gameObject.layer = LayerMask.NameToLayer("BattleActor");
        newActor.maxHealth = 10;
        newActor.health = 10;
        newActor.isDead = false;
        newActor.isTakingTurn = false;
        newActor.partyIndex = partyIndex;
        placeHolder.transform.position = new Vector3(-6, 1.5f, GetZ(partySize, partyIndex, playerZoffset, false));
    }

    void SpawnEnemyParty()
    {
        if(info == null)
        {
            // Check for the StageInfo object
            GameObject stageInfo = GameObject.FindGameObjectWithTag("Stage Info");

            if (stageInfo != null)
                stageInfo.TryGetComponent(out info);
        }
        if(info != null)
        {
            if(info.enemyParty != null)
            {
                // spawn enemy prefabs
                for(int i = 0; i < info.enemyParty.Length; i++)
                {
                    string enemyName = info.enemyParty[i];

                    Object obj = Resources.Load("Prefabs/Battle/Enemy/" + enemyName);
                    GameObject model = null;
                    if (obj != null)
                        model = Instantiate((GameObject)obj);

                    // Spawn a placeholder if the prefab path is invalid
                    if (model == null)
                    {
                        SpawnPlaceholderEnemyActor(i, info.enemyParty.Length);
                        Debug.LogWarning("Could not load prefab resource at Prefabs/Battle/Enemy/" + enemyName);
                    }
                    else
                    {
                        model.name = enemyName;
                        Battle_Actor modelActor = model.GetComponent<Battle_Actor>();
                        modelActor.isTakingTurn = false;
                        modelActor.partyIndex = i;
                        model.transform.position = new Vector3(3, 2.53f, GetZ(info.enemyParty.Length, i, enemyZoffset, true));
                    }
                }

                return;
            }
        }

        // if all else fails, spawn in a default enemy party
        for (int i = 0; i < 3; i++)
        {
            SpawnPlaceholderEnemyActor(i, 3);
        }
    }

    void SpawnPlaceholderEnemyActor(int partyIndex, int partySize)
    {
        GameObject placeHolder = GameObject.CreatePrimitive(PrimitiveType.Cube);
        Battle_Actor newActor = placeHolder.AddComponent<Battle_Actor>();
        newActor.gameObject.tag = "Enemy";
        newActor.gameObject.layer = LayerMask.NameToLayer("BattleActor");
        newActor.maxHealth = 10;
        newActor.health = 10;
        newActor.isDead = false;
        newActor.isTakingTurn = false;
        newActor.partyIndex = partyIndex;
        placeHolder.transform.position = new Vector3(3, 2.53f, GetZ(partySize, partyIndex, enemyZoffset, true));
    }

    float GetZ(int partySize, int partyIndex, float Zoffset, bool isEnemyParty)
    {
        if (partySize <= 0)
            return 0;

        float finalZ = Zoffset * ((partySize - 1) * 0.5f - partyIndex);

        if (isEnemyParty)
            finalZ *= -1;

        return finalZ;
    }
}
