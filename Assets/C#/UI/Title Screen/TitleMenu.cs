using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Playables;
using UnityEngine.SceneManagement;
using UnityEngine.Timeline;

public class TitleMenu : MonoBehaviour
{
    public PlayableDirector titleDirector;
    public GameObject titleText;
    public GameObject buttonContainer;
    public ScreenTransition transition;
    PlayableDirector fakeDirector;
    GameObject loopedRays;
    GameObject timelineRays;
    bool quit;
    bool load;

    // Start is called before the first frame update
    void Start()
    {
        quit = load = false;

        if (titleDirector != null)
        {
            // Open first scene when timeline stops
            titleDirector.stopped += LoadFirstScene;

            titleDirector.playableGraph.GetRootPlayable(0).SetSpeed(0);

            // Get loopedRays object
            loopedRays = titleDirector.transform.parent.Find("Window_Glare_Looped").gameObject;

            // Get real rays object
            timelineRays = titleDirector.transform.parent.Find("Window_Glare").gameObject;

            // show looping godrays
            loopedRays.SetActive(true);

            // hide timeline's godrays
            timelineRays.SetActive(false);
        }
        else
        {
            Debug.LogWarning("GameObject titleDirector not assigned in TitleMenu.cs!");
        }

        // Used to fix a bug with director.stopped being invoked when closing the application. See LoadFirstScene() below for more.
        Application.quitting += isQuitting;
    }

    // Function used to check if the application is quitting. See LoadFirstScene() below for more.
    void isQuitting()
    {
        quit = true;
    }

    public void LoadGame()
    {
        load = true;
        SaveDataManager.Load(SaveType.Player);
        SaveDataManager.Load(SaveType.Config);

        // open the saved level number
        int levelIndex = SaveDataManager.playerdata.progress.SceneNumber;

        StartCoroutine("LoadNewSceneAsync", levelIndex);
    }

    public void NewGame()
    {
        Debug.Log("Playing timeline animation");

        // Hide Title
        if (titleText != null)
            titleText.SetActive(false);
        else
            Debug.LogWarning("Cannot hide titleText on title screen! GameObject titleText not assigned in TitleMenu.cs!");

        // Hide buttons
        if (buttonContainer != null)
            buttonContainer.SetActive(false);
        else
            Debug.LogWarning("Cannot hide buttons on title screen! GameObject buttonContainer not assigned in TitleMenu.cs!");

        // Hide looping godrays
        loopedRays.SetActive(false);

        // Show timeline's godrays
        timelineRays.SetActive(true);
        
        // Start timeline animation
        //titleDirector.Play(titleDirector.playableAsset);
        titleDirector.playableGraph.GetRootPlayable(0).SetSpeed(1);
    }

    void LoadFirstScene(PlayableDirector pd)
    {
        // When the application quits, the timeline's director.stopped event is invoked, for some reason.
        // This causes LoadFirstScene() to run when the application is closing, which resets the save files.
        // To make sure we don't run this code when closing the application, we check to see if we are quitting the app first.
        // This quit boolean is set by the isQuitting() function, which runs when Application.quitting is invoked by the Unity system.
        // The load boolean also prevents this from happening after a SceneManager.LoadScene call.
        if(quit == false && load == false)
        {
            Debug.Log("Loading first scene");

            // Rebuild save files
            SaveDataManager.RebuildFile(SaveType.Player);
            SaveDataManager.RebuildFile(SaveType.Config);

            // open level 1
            StartCoroutine("LoadNewSceneAsync", 1);
        }
    }

    IEnumerator LoadNewSceneAsync(int levelIndex)
    {
        // Hide buttons, if they're not already hidden
        if (buttonContainer != null)
            buttonContainer.SetActive(false);
        else
            Debug.LogWarning("Cannot hide buttons on title screen! GameObject buttonContainer not assigned in TitleMenu.cs!");

        // Start screen transition
        if (transition != null)
            transition.ToOpaque();
        else
            Debug.LogWarning("Cannot start screen transition. ScreenTransition transition not assigned in TitleMenu.cs!");

        yield return transition.SceneLoadingOperation(levelIndex, false, true);
    }

    public void QuitGame()
    {
        Application.Quit();
    }

    

    private void OnDestroy()
    {
        titleDirector.stopped -= LoadFirstScene;
        Application.quitting -= isQuitting;
    }
}
