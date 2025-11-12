using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.SceneManagement;

public class VictoryScreen : MonoBehaviour
{
    public bool HasClickedContinue;
    public GameObject content;
    public string OverworldName;
    ScreenTransition st;
    Vector3 victoryCamPos = new Vector3(-1.82f, 1.87f, 2.14f);
    Vector3 victoryCamRot = new Vector3(7.35f, -113.47f, 0);

    private void Start()
    {
        HasClickedContinue = false;
        content.SetActive(false);
        Turn_Manager.PlayerVictoryEvent += VictoryAnimWait;
        st = FindAnyObjectByType<ScreenTransition>();
    }

    // Called by this screen's continue button
    public void ContinueButton()
    {
        HasClickedContinue = true;
    }

    public void MakeVisible()
    {
        content.SetActive(true);
        BeginLoadingOverworld(OverworldName);
    }

    // Called after a battle has been finished
    public void BeginLoadingOverworld(string sceneName)
    {
        StartCoroutine(LoadOverworld(sceneName));
    }

    // Async load the overworld for smoother transition
    IEnumerator LoadOverworld(string sceneName)
    {
        AsyncOperation loadOperation = null;
        if (SceneManager.GetSceneByName(sceneName) != null)
        {
            // Only load the next scene if it is not the same as the current scene
            if (!SceneManager.GetSceneByName(sceneName).Equals(SceneManager.GetActiveScene()))
            {
                loadOperation = SceneManager.LoadSceneAsync(sceneName, LoadSceneMode.Single);
                loadOperation.allowSceneActivation = false;
            }

            // Ensure time is not stopped
            Time.timeScale = 1;

            // Inform the enemy manager of a victory, if applicable
            GameObject stageInfoGameObject = GameObject.FindGameObjectWithTag("Stage Info");

            if (stageInfoGameObject != null)
            {
                stageInfoGameObject.TryGetComponent(out StageInfo si);
                if (si != null)
                {
                    if (si.battleIndex != -1)
                    {
                        si.graveyard.Add(si.battleIndex);
                    }
                    else
                        Debug.LogWarning("No instance id to send to the dead actor list!");

                    // Go through the player party and save all hp values
                    Player_Battle_Actor[] actors = FindObjectsByType<Player_Battle_Actor>(FindObjectsSortMode.None);
                    foreach(Player_Battle_Actor actor in actors)
                    {
                        foreach(PartyMember stageInfoPartyMember in si.partyMembers)
                        {
                            if(actor.partyIndex == stageInfoPartyMember.partyIndex)
                            {
                                stageInfoPartyMember.health = actor.health;
                                yield return new WaitForEndOfFrame();
                                continue;
                            }
                        }
                    }
                }
            }
                
            // Wait for player to press the continue button
            yield return new WaitUntil(() => HasClickedContinue == true);

            if (st != null)
            {
                st.StartTransition();

                // wait for screen transition
                yield return new WaitUntil(() => st.IsOpaque == true);

                // Wait for scene to load, if it is loading
                if (loadOperation != null) yield return WaitToLoad(loadOperation);
            }

            // Allow next scene to fully load, if it is loaded
            if (loadOperation != null) loadOperation.allowSceneActivation = true;
        }
        else
            Debug.LogError("The value within VictoryScreen.OverworldName does not exist in the scene list! Did you forget to set the value?");
    }

    IEnumerator WaitToLoad(AsyncOperation loadOperation)
    {
        do
        {
            yield return new WaitForFixedUpdate();
        } while (loadOperation.progress < 0.9f);
    }

    public void VictoryAnimWait()
    {
        StartCoroutine("WaitForVictoryAnimation");
    }

    IEnumerator WaitForVictoryAnimation()
    {
        // Wait
        yield return new WaitForSeconds(1);
       
        Player_Battle_Actor[] actors = Object.FindObjectsByType<Player_Battle_Actor>(FindObjectsSortMode.None);

        // Move camera to front of party based on party count
        switch(actors.Length)
        {
            //case 1: break; // Move camera to solo position
            //case 2: break; // Move camera to duo position
            //case 3: break; // Move camera to trio position
            //case 4: break; // Move camera to quad position

            default: // Move camera to duo position
                Camera.main.transform.parent.position = victoryCamPos;
                Camera.main.transform.parent.rotation = Quaternion.Euler(victoryCamRot);
                break;
        }

        // Wait
        yield return new WaitForFixedUpdate();

        // Make alive players play victory animations
        foreach (Player_Battle_Actor actor in actors)
        {
            if (actor.isDead == false)
            {
                // Play their runaway animation
                actor.battleAnim.PlayVictoryAnimation();
            }
        }

        // Close the player combat ui
        GameObject battleUI = GameObject.FindGameObjectWithTag("Battle UI");
        battleUI.GetComponentInChildren<CombatUI_Container>().DisableAllCombatUI();
        
        // Wait
        yield return new WaitForSeconds(2);

        // fade out
        st.StartTransition();

        // wait for screen transition
        yield return new WaitUntil(() => st.IsOpaque == true);

        // activate screen
        MakeVisible();

        // fade in
        st.EndTransition();
        
    }

    private void OnDestroy()
    {
        Turn_Manager.PlayerVictoryEvent -= VictoryAnimWait;
    }
}
