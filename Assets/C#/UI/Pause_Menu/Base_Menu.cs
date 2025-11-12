using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEditor.Rendering.LookDev;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class Base_Menu : MonoBehaviour
{
    public Pause_UI pui;
    public Transform buttons;
    [SerializeField] List<GameObject> buttonList;
    public int cursorIndex;

    private void Start()
    {
        /*
        buttonList = new List<GameObject>();
        foreach (Transform child in buttons)
            buttonList.Add(child.gameObject);
        cursorIndex = 0;
        if(buttonList.Count > 0)
            HighlightButton(buttonList[cursorIndex]);
        */
    }

    private void Update()
    {
        /*
        // Ensure button is highlighted during edge cases
        if(buttonList[cursorIndex].GetComponent<Animator>().GetBool("Highlight") != true)
        {
            HighlightButton(buttonList[cursorIndex]);
        }

        // Highlight new button upon player input
        if(pui.moveVertical != 0 && buttonList.Count > 0)
        {
            int newIndex = cursorIndex - pui.moveVertical;
            if (newIndex < 0)
                newIndex = buttonList.Count - 1;
            else if (newIndex >= buttonList.Count)
                newIndex = 0;
            
            if(buttonList[cursorIndex] != buttonList[newIndex])
            {
                UnhighlightButton(buttonList[cursorIndex]);
                HighlightButton(buttonList[newIndex]);
                cursorIndex = newIndex;
            }
        }
        */
    }

    void HighlightButton(GameObject button)
    {
        button.GetComponent<Animator>().SetBool("Highlight", true);
        button.GetComponent<Animator>().SetBool("Unhighlight", false);
    }

    void UnhighlightButton(GameObject button)
    {
        button.GetComponent<Animator>().SetBool("Highlight", false);
        button.GetComponent<Animator>().SetBool("Unhighlight", true);
    }

    void ClickButton(GameObject button)
    {
        button.GetComponent<Button>().onClick.Invoke();
    }

    public IEnumerator UpdatePartyUI()
    {
        // Wait a few frames for PlayerStatus to run its Start() function on the first pause.
        // Testing showed only a 1 frame wait time.
        for(int i = 0; i < 5; i++)
        {
            if (PlayerStatus.updatePauseUI != null)
                break;

            yield return new WaitForEndOfFrame();
        }

        if (pui.stageInfo != null)
        {
            foreach (PartyMember partyMember in pui.stageInfo.partyMembers)
            {
                //Debug.Log(partyMember.name);
                PlayerStatus.updatePauseUI?.Invoke(partyMember);
            }
        }
        else
        {
            SaveDataManager.Load(SaveType.Player);
            foreach (PartyMember partyMember in SaveDataManager.playerdata.party)
            {
                PlayerStatus.updatePauseUI?.Invoke(partyMember);
            }
        }
    }

    private void OnEnable()
    {
        StartCoroutine("UpdatePartyUI");
    }

    private void OnDisable()
    {
        StopAllCoroutines();
    }

    #region ButtonFunctions
    public void DevTools()
    {
        // tell pui to open dev menu
    }

    public void Options()
    {
        // tell pui to open options menu
    }

    public void Unpause()
    {
        if (pui != null)
            pui.UnpauseGame();
    }

    public void SaveGame()
    {
        SaveDataManager.Load(SaveType.Player);

        // scene number
        SaveDataManager.playerdata.progress.SceneNumber = SceneManager.GetActiveScene().buildIndex;

        // position
        Vector3 playerPosition = GameObject.FindAnyObjectByType<PartyLeaderActor>().transform.position;
        SaveDataManager.playerdata.progress.respawnPointX = playerPosition.x;
        SaveDataManager.playerdata.progress.respawnPointZ = playerPosition.z;

        GameObject stageInfoGameObject = GameObject.FindGameObjectWithTag("Stage Info");
        if (stageInfoGameObject != null)
        {
            stageInfoGameObject.TryGetComponent(out StageInfo stageInfo);

            if (stageInfo != null)
            {
                // current party
                foreach (PartyMember saveDataPartyMember in SaveDataManager.playerdata.party)
                {
                    foreach (PartyMember stageInfoPartyMember in stageInfo.partyMembers)
                    {
                        if (stageInfoPartyMember.partyIndex == saveDataPartyMember.partyIndex)
                        {
                            saveDataPartyMember.health = stageInfoPartyMember.health;
                            continue;
                        }
                    }
                }

                // player location for stage info. Needed for title menu transitions.
                stageInfo.playerLocation = playerPosition;

                // Get the enemy list from the save file, if it exists
                EnemyList e_list = null;
                foreach (EnemyList list in SaveDataManager.playerdata.progress.enemyLists)
                {
                    if (list.sceneNumber == SceneManager.GetActiveScene().buildIndex)
                    {
                        e_list = list;
                        break;
                    }
                }
                if(e_list == null) // If the list does not exist, add a new one
                {
                    SaveDataManager.playerdata.progress.enemyLists.Add(e_list = new EnemyList(SceneManager.GetActiveScene().buildIndex));
                }

                // get the enemy graveyard list
                e_list.graveyard = stageInfo.graveyard;

                // Add all current enemy positions to the list, ideally from the enemy manager
                EnemyManager em = FindAnyObjectByType<EnemyManager>();
                if (em != null)
                {
                    List<EnemyPosition> newPositions = new();
                    foreach (Actor enemy in em.enemyActors)
                    {
                        EnemyPosition newEP = stageInfo.ConvertToEnemyPosition(enemy.transform.position);
                        newPositions.Add(newEP);
                        //Debug.Log("Adding " + enemy.name + "'s position: " + enemy.transform.position.ToString());
                    }
                    e_list.enemyPositions = newPositions;
                }
                else // if the enemy manager does not exist, use the stage info as a non-ideal backup option
                {
                    e_list.enemyPositions = stageInfo.enemyPositions;
                }
            }
        }

        SaveDataManager.Save(SaveType.Player);

        Debug.Log("Saved");
    }

    public void ReturnToTitle()
    {
        SceneManager.LoadScene(0);
    }

    public void QuitGame()
    {
        Application.Quit();
    }
    #endregion
}
