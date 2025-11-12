using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class DefeatScreen : MonoBehaviour
{
    ScreenTransition st;
    public GameObject content;
    //public string TitleScreenName;
    public Camera defeatScreenPrefabCamera;
    public Canvas defeatScreenPrefabCanvas;
    public Animator jimmyAnim;
    public Animator robotAnim;

    private void Start()
    {
        content.SetActive(false);
        Turn_Manager.PlayerDefeatEvent += DefeatAnimWait;
        st = FindAnyObjectByType<ScreenTransition>();
    }

    public void DefeatAnimWait()
    {
        StartCoroutine("WaitForDefeatAnimation");
    }

    IEnumerator WaitForDefeatAnimation()
    {
        // wait for animation
        yield return new WaitForSeconds(2); // TEMPORARY

        // Fade out
        if (st != null)
        {
            st.StartTransition();

            // wait for screen transition
            yield return new WaitUntil(() => st.IsOpaque == true);
        }

        // set regular camera component to disabled
        Camera.main.enabled = false;

        // set defeat prefab camera component to enabled
        defeatScreenPrefabCamera.enabled = true;

        // Move screen transition over to defeat screen canvas
        if (st != null)
            st.transform.SetParent(defeatScreenPrefabCanvas.transform, false);

        // Fade in
        if (st != null)
        {
            st.EndTransition();

            // wait for screen transition
            yield return new WaitUntil(() => st.IsOpaque == false);
        }

        // Wait
        yield return new WaitForSeconds(0.5f);

        // activate screen
        content.transform.SetParent(defeatScreenPrefabCanvas.transform, false);
        MakeVisible();
    }

    public void MakeVisible()
    {
        content.SetActive(true);
    }

    public void RestartBattle()
    {
        // Reload this scene
        StartCoroutine(LoadScene(SceneManager.GetActiveScene().name));
    }

    public void LoadGame()
    {
        SaveDataManager.Load(SaveType.Player);
        SaveDataManager.Load(SaveType.Config);

        // open the saved level number
        int levelIndex = SaveDataManager.playerdata.progress.SceneNumber;

        // Destroy the stage info object, if it exists
        StageInfo.reloadStageInfo?.Invoke(levelIndex);

        SceneManager.LoadScene(levelIndex);
    }

    public void ReturnToTitle()
    {
        // Load title screen
        //StartCoroutune(LoadScene(TitleScreenName));
    }

    public void ExitGame()
    {
        Application.Quit();
    }

    IEnumerator LoadScene(string sceneName)
    {
        // Close the selection menu
        content.SetActive(false);

        AsyncOperation loadOperation = null;

        loadOperation = SceneManager.LoadSceneAsync(sceneName, LoadSceneMode.Single);
        loadOperation.allowSceneActivation = false;

        // Jimmy jumps up and runs after the enemy
        jimmyAnim.SetTrigger("Retry");
        robotAnim.SetTrigger("Retry");

        // Wait
        yield return new WaitForSeconds(4.5f);

        // Ensure time is not stopped
        Time.timeScale = 1;

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

    IEnumerator WaitToLoad(AsyncOperation loadOperation)
    {
        do
        {
            yield return new WaitForFixedUpdate();
        } while (loadOperation.progress < 0.9f);
    }

    private void OnDestroy()
    {
        Turn_Manager.PlayerDefeatEvent -= DefeatAnimWait;
    }
}
