using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Playables;
using UnityEngine.SceneManagement;
using UnityEngine.Timeline;

public class TitleMenu : MonoBehaviour
{
    public PlayableDirector titleDirector;
    PlayableDirector fakeDirector;
    GameObject loopedRays;
    GameObject timelineRays;

    // Start is called before the first frame update
    void Start()
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

    public void LoadGame()
    {
        SaveDataManager.Load(SaveType.Player);
        SaveDataManager.Load(SaveType.Config);

        // open the saved level number
        int levelIndex = SaveDataManager.playerdata.progress.SceneNumber;

        SceneManager.LoadScene(levelIndex);
    }

    public void NewGame()
    {
        Debug.Log("Playing timeline animation");

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
        Debug.Log("Loading first scene");

        // Rebuild save files
        SaveDataManager.RebuildFile(SaveType.Player);
        SaveDataManager.RebuildFile(SaveType.Config);

        // open level 1
        SceneManager.LoadScene(1);
    }

    public void QuitGame()
    {
        Application.Quit();
    }

    private void OnDestroy()
    {
        titleDirector.stopped -= LoadFirstScene;
    }
}
