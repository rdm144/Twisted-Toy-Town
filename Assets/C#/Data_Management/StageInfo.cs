using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

/// <summary>
/// This acts as a temporary storage location for information specific to the current level/stage.
/// </summary>
public class StageInfo : MonoBehaviour
{
    public static StageInfo instance;

    public List<PartyMember> partyMembers;
    public Vector3 playerLocation;// { get; private set; }
    public Vector3 playerRotation;// { get; private set; }
    public List<EnemyPosition> enemyPositions;

    public List<int> graveyard; // Battle screen should add to this if the player is victorious
    public int stunIndex; // Battle screen should set this if the player runs
    public int battleIndex; // The Enemy should inform the enemy manager who is about to enter a battle
    public string[] enemyParty; // The Enemy should change this only if a specific party is desired. Used by ActorLoader.

    public delegate void ReloadStageInfo(int sceneNumber);
    public static ReloadStageInfo reloadStageInfo;

    private void Awake()
    {
        if (instance == null)
            instance = this;
        else
            Destroy(gameObject);

        DontDestroyOnLoad(gameObject);
        enemyParty = null;
        playerLocation = Vector3.zero;
        playerRotation = Vector3.zero;
        graveyard = new List<int>();
        enemyPositions = new List<EnemyPosition>();
        partyMembers = new List<PartyMember>();
        LoadFromDisk();

        SceneManager.activeSceneChanged += DestroyOnTitle;
        reloadStageInfo += LoadFromDisk;
    }

    void DestroyOnTitle(Scene current, Scene next)
    {
        if(next.buildIndex == 0)
        {
            Destroy(gameObject);
        }
    }

    public Vector3 ConvertToVector3(EnemyPosition pos)
    {
        return new Vector3(pos.x, pos.y, pos.z);
    }

    public EnemyPosition ConvertToEnemyPosition(Vector3 position)
    {
        EnemyPosition epos = new EnemyPosition();
        Vector3Int pos_int = Vector3Int.RoundToInt(position);
        epos.x = pos_int.x;
        epos.y = pos_int.y;
        epos.z = pos_int.z;
        return epos;
    }

    void LoadFromDisk()
    {
        enemyPositions.Clear();
        graveyard.Clear();
        partyMembers.Clear();
        enemyParty = null;

        SaveDataManager.Load(SaveType.Player);

        playerLocation = new Vector3(
            SaveDataManager.playerdata.progress.respawnPointX,
            0,
            SaveDataManager.playerdata.progress.respawnPointZ);

        partyMembers = SaveDataManager.playerdata.party;

        stunIndex = -1;
        battleIndex = -1;

        // Get enemy info for this stage from the save file
        foreach (EnemyList list in SaveDataManager.playerdata.progress.enemyLists)
        {
            if (list.sceneNumber == SceneManager.GetActiveScene().buildIndex)
            {
                graveyard = list.graveyard;
                enemyPositions = list.enemyPositions;
                continue;
            }
        }
    }

    void LoadFromDisk(int sceneNumber)
    {
        enemyPositions.Clear();
        graveyard.Clear();
        partyMembers.Clear();
        enemyParty = null;

        SaveDataManager.Load(SaveType.Player);

        playerLocation = new Vector3(
            SaveDataManager.playerdata.progress.respawnPointX,
            0,
            SaveDataManager.playerdata.progress.respawnPointZ);

        partyMembers = SaveDataManager.playerdata.party;

        stunIndex = -1;
        battleIndex = -1;

        // Get enemy info for this stage from the save file
        foreach (EnemyList list in SaveDataManager.playerdata.progress.enemyLists)
        {
            if (list.sceneNumber == sceneNumber)
            {
                graveyard = list.graveyard;
                enemyPositions = list.enemyPositions;
                continue;
            }
        }
    }

    #region Setters
    public void SetPlayerRespawnLocation(Vector3 newPosition)
    {
        //hasBeenAltered = true;
        playerLocation = Vector3Int.RoundToInt(newPosition);
    }

    public void SetPlayerRotation(Vector3 newRotation)
    {
        //hasBeenAltered = true;
        playerRotation = newRotation;
    }
    #endregion

    private void OnDestroy()
    {
        SceneManager.activeSceneChanged -= DestroyOnTitle;
        reloadStageInfo -= LoadFromDisk;
    }
}
