using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UIElements;

public class SceneLoadButton : MonoBehaviour
{
    public void LoadNewScene(string sceneName)
    {
        SceneManager.LoadScene(sceneName);
    }

    public void SwitchActiveScene(string sceneName)
    {
        StartCoroutine(SwitchToOverworld(sceneName));
    }

    IEnumerator SwitchToOverworld(string sceneName)
    {
        ScreenTransition st = GameObject.FindAnyObjectByType<ScreenTransition>();
        if (st != null) st.StartTransition();

        AsyncOperation loadOperation = null;

        GameObject[] partyMembers = GameObject.FindGameObjectsWithTag("Player");

        // Check if the desired scene is valid
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

            // wait for screen transition
            yield return new WaitForSeconds(0.25F);

            // Wait for scene to load, if it is loading
            if (loadOperation != null) yield return WaitToLoad(loadOperation);

            Vector3 position = new Vector3(RoundToNearest(partyMembers[0].transform.position.x, 1), RoundToNearest(partyMembers[0].transform.position.y, 1), 0);

            // Reposition all party members
            yield return RepositionPartyMembers(partyMembers, position);

            // Unfreeze all party members
            yield return UnfreezePartyMembers(partyMembers);

            // Start screen transition
            if (st != null) st.EndTransition();

            // Allow next scene to fully load, if it is loaded
            if (loadOperation != null) loadOperation.allowSceneActivation = true;
        }
    }

    IEnumerator RepositionPartyMembers(GameObject[] partyMembers, Vector2 SpawnPoint)
    {
        foreach (GameObject partyMember in partyMembers)
        {
            partyMember.transform.position = SpawnPoint;
        }

        // Wait 2 frames for player scripts to update
        yield return new WaitForFixedUpdate();
        yield return new WaitForFixedUpdate();
    }
    IEnumerator UnfreezePartyMembers(GameObject[] partyMembers)
    {
        foreach (GameObject partyMember in partyMembers)
            partyMember.GetComponent<Actor>().canOperate = true;

        yield return new WaitForFixedUpdate();
    }

    float RoundToNearest(float value, float multiple)
    {
        return Mathf.Round(value / multiple) * multiple;
    }

    IEnumerator WaitToLoad(AsyncOperation loadOperation)
    {
        do
        {
            yield return new WaitForFixedUpdate();
        } while (loadOperation.progress < 0.9f);
    }
}
