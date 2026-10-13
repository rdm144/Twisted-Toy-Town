using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using System;

public class ScreenTransition : MonoBehaviour
{
    Animator anim;
    public Image blackImage;
    public bool IsOpaque;
    public static event Action BecameOpaque;
    public static event Action BecameTransparent;
    static bool isTransitioning; // Prevents parallel level loading

    private void Awake()
    {
        blackImage.color = Color.black;
    }

    // Start is called before the first frame update
    void Start()
    {
        anim = GetComponent<Animator>();
        isTransitioning = false;

        // Action subscribtions for when direct access to this script comes from outside the UI.
        Teleporter.DimScreen += ToOpaque;
        Teleporter.UndimScreen += ToTransparent;
        Enemy.StartBattle += FightTransition;
        RunAbility.EndBattle += EscapeTransition; // For some reason, this event is invoked twice despite having only 1 subscriber and 1 Invoke() call

        ToTransparent();
    }

    // Used by Fade in animation
    public void SetOpaqueState()
    {
        IsOpaque = true;
        isTransitioning = false;
        if (BecameOpaque != null) BecameOpaque.Invoke();
        anim.ResetTrigger("ToOpaque");
        anim.ResetTrigger("ToTransparent");
        anim.ResetTrigger("Fight");
    }

    // Used by fade out animation
    public void SetTransparentState()
    {
        IsOpaque = false;
        isTransitioning = false;
        if (BecameTransparent != null) BecameTransparent.Invoke();
        anim.ResetTrigger("ToOpaque");
        anim.ResetTrigger("ToTransparent");
        anim.ResetTrigger("Fight");
    }

    public void ToOpaque()
    {
        anim.SetTrigger("ToOpaque");
    }

    public void ToTransparent()
    {
        anim.SetTrigger("ToTransparent");
        
    }

    public void ToBattle()
    {
        anim.SetTrigger("Fight");
    }

    public void FightTransition(string levelToLoad)
    {
        ToBattle();
        StartCoroutine(SceneLoadingOperation(levelToLoad, false, false));
    }

    public void EscapeTransition(string levelToLoad)
    {
        ToOpaque();
        StartCoroutine(SceneLoadingOperation(levelToLoad, false, true));
    }

    public void NormalTransition(string levelToLoad, bool deletePersitants, bool useOverworldCam)
    {
        ToOpaque();
        StartCoroutine(SceneLoadingOperation(levelToLoad, deletePersitants, useOverworldCam));
    }

    public IEnumerator SceneLoadingOperation(int levelIndex)
    {
        if (isTransitioning == false)
        {
            isTransitioning = true;

            AsyncOperation loadOperation = null;
            GameObject[] partyMembers = GameObject.FindGameObjectsWithTag("Player");

            // Check if the desired scene is valid
            if (SceneManager.GetSceneByBuildIndex(levelIndex) != null)
            {
                // Only load the next scene if it is not the same as the current scene
                if (!SceneManager.GetSceneByBuildIndex(levelIndex).Equals(SceneManager.GetActiveScene()))
                {
                    loadOperation = SceneManager.LoadSceneAsync(levelIndex, LoadSceneMode.Single);
                    loadOperation.allowSceneActivation = false;
                }

                // Ensure time is not stopped
                Time.timeScale = 1;

                // wait for screen transition to complete
                do
                {
                    yield return null;
                } while (IsOpaque == false);

                // Wait for scene to load, if it is loading
                if (loadOperation != null)
                {
                    do
                    {
                        yield return null;
                    } while (loadOperation.progress < 0.9f);
                }

                // begin turning transparent
                ToTransparent();

                // Allow next scene to fully load, if it is loaded
                if (loadOperation != null) loadOperation.allowSceneActivation = true;
            }
            else
                Debug.LogError("Could not load scene with index " + levelIndex + ". Scene is not in build.");
        }
    }

    public IEnumerator SceneLoadingOperation(string levelName)
    {
        if (isTransitioning == false)
        {
            isTransitioning = true;

            AsyncOperation loadOperation = null;
            GameObject[] partyMembers = GameObject.FindGameObjectsWithTag("Player");

            // Check if the desired scene is valid
            if (SceneManager.GetSceneByName(levelName) != null)
            {
                // Only load the next scene if it is not the same as the current scene
                if (!SceneManager.GetSceneByName(levelName).Equals(SceneManager.GetActiveScene().name))
                {
                    loadOperation = SceneManager.LoadSceneAsync(levelName, LoadSceneMode.Single);
                    loadOperation.allowSceneActivation = false;
                }

                // Ensure time is not stopped
                Time.timeScale = 1;

                // wait for screen transition to complete
                do
                {
                    yield return null;
                } while (IsOpaque == false);

                // Wait for scene to load, if it is loading
                if (loadOperation != null)
                {
                    do
                    {
                        yield return null;
                    } while (loadOperation.progress < 0.9f);
                }

                // begin turning transparent
                ToTransparent();

                // Allow next scene to fully load, if it is loaded
                if (loadOperation != null) loadOperation.allowSceneActivation = true;
            }
            else
                Debug.LogError("Could not load scene with name " + levelName + ". Scene is not in build.");
        }
    }

    public IEnumerator SceneLoadingOperation(int levelIndex, bool deletePersistantObjects)
    {
        if (isTransitioning == false)
        {
            isTransitioning = true;

            AsyncOperation loadOperation = null;
            GameObject[] partyMembers = GameObject.FindGameObjectsWithTag("Player");

            // Check if the desired scene is valid
            if (SceneManager.GetSceneByBuildIndex(levelIndex) != null)
            {
                // Only load the next scene if it is not the same as the current scene
                if (!SceneManager.GetSceneByBuildIndex(levelIndex).Equals(SceneManager.GetActiveScene()))
                {
                    loadOperation = SceneManager.LoadSceneAsync(levelIndex, LoadSceneMode.Single);
                    loadOperation.allowSceneActivation = false;
                }

                // Ensure time is not stopped
                Time.timeScale = 1;

                // wait for screen transition to complete
                do
                {
                    yield return null;
                } while (IsOpaque == false);

                // Wait for scene to load, if it is loading
                if (loadOperation != null)
                {
                    do
                    {
                        yield return null;
                    } while (loadOperation.progress < 0.9f);
                }

                // begin turning transparent
                ToTransparent();

                // Allow next scene to fully load, if it is loaded
                if (loadOperation != null) loadOperation.allowSceneActivation = true;

                yield return null;

                // Deletes objects marked as DontDestroyOnLoad, if desired. These are tagged as "Player" or "Screen".
                if (deletePersistantObjects == true)
                {
                    GameObject[] players = GameObject.FindGameObjectsWithTag("Player");
                    GameObject[] screens = GameObject.FindGameObjectsWithTag("Screen");
                    PartyMemberActor.partyMembers = null;
                    foreach (GameObject p in players)
                    {
                        Destroy(p);
                    }
                    foreach (GameObject s in screens)
                    {
                        Destroy(s);
                    }
                }
            }
            else
                Debug.LogError("Could not load scene with index " + levelIndex + ". Scene is not in build.");
        }
    }

    public IEnumerator SceneLoadingOperation(string levelName, bool deletePersistantObjects)
    {
        if (isTransitioning == false)
        {
            isTransitioning = true;

            AsyncOperation loadOperation = null;
            GameObject[] partyMembers = GameObject.FindGameObjectsWithTag("Player");

            // Check if the desired scene is valid
            if (SceneManager.GetSceneByName(levelName) != null)
            {
                // Only load the next scene if it is not the same as the current scene
                if (!SceneManager.GetSceneByName(levelName).Equals(SceneManager.GetActiveScene().name))
                {
                    loadOperation = SceneManager.LoadSceneAsync(levelName, LoadSceneMode.Single);
                    loadOperation.allowSceneActivation = false;
                }

                // Ensure time is not stopped
                Time.timeScale = 1;

                // wait for screen transition to complete
                do
                {
                    yield return null;
                } while (IsOpaque == false);

                // Wait for scene to load, if it is loading
                if (loadOperation != null)
                {
                    do
                    {
                        yield return null;
                    } while (loadOperation.progress < 0.9f);
                }

                // begin turning transparent
                ToTransparent();

                // Allow next scene to fully load, if it is loaded
                if (loadOperation != null) loadOperation.allowSceneActivation = true;

                yield return null;

                // Deletes objects marked as DontDestroyOnLoad, if desired. These are tagged as "Player" or "Screen".
                if (deletePersistantObjects == true)
                {
                    GameObject[] players = GameObject.FindGameObjectsWithTag("Player");
                    GameObject[] screens = GameObject.FindGameObjectsWithTag("Screen");
                    PartyMemberActor.partyMembers = null;
                    foreach (GameObject p in players)
                    {
                        Destroy(p);
                    }
                    foreach (GameObject s in screens)
                    {
                        Destroy(s);
                    }
                }
            }
            else
                Debug.LogError("Could not load scene with name " + levelName + ". Scene is not in build.");
        }
    }

    public IEnumerator SceneLoadingOperation(int levelIndex, bool deletePersistantObjects, bool IsOverworldCamNowMainCamera)
    {
        if (isTransitioning == false)
        {
            isTransitioning = true;

            AsyncOperation loadOperation = null;
            GameObject[] partyMembers = GameObject.FindGameObjectsWithTag("Player");

            // Check if the desired scene is valid
            if (SceneManager.GetSceneByBuildIndex(levelIndex) != null)
            {
                // Only load the next scene if it is not the same as the current scene
                if (!SceneManager.GetSceneByBuildIndex(levelIndex).Equals(SceneManager.GetActiveScene()))
                {
                    loadOperation = SceneManager.LoadSceneAsync(levelIndex, LoadSceneMode.Single);
                    loadOperation.allowSceneActivation = false;
                }

                // Ensure time is not stopped
                Time.timeScale = 1;

                // wait for screen transition to complete
                do
                {
                    yield return null;
                } while (IsOpaque == false);

                // Wait for scene to load, if it is loading
                if (loadOperation != null)
                {
                    do
                    {
                        yield return null;
                    } while (loadOperation.progress < 0.9f);
                }

                GameObject[] screens = GameObject.FindGameObjectsWithTag("Screen");

                // Sets the overworld camera as either MainCamera or Untagged.
                if (screens != null)
                {
                    foreach (GameObject screen in screens)
                    {
                        GameObject cam = screen.GetComponentInChildren<Camera>().gameObject;
                        cam.tag = IsOverworldCamNowMainCamera ? "MainCamera" : "Untagged";
                        cam.GetComponent<AudioListener>().enabled = IsOverworldCamNowMainCamera;
                    }
                }

                // begin turning transparent
                ToTransparent();

                // Allow next scene to fully load, if it is loaded
                if (loadOperation != null) loadOperation.allowSceneActivation = true;

                yield return null;

                // Deletes objects marked as DontDestroyOnLoad, if desired. These are tagged as "Player" or "Screen".
                if (deletePersistantObjects == true)
                {
                    GameObject[] players = GameObject.FindGameObjectsWithTag("Player");

                    PartyMemberActor.partyMembers = null;
                    if (players != null)
                    {
                        foreach (GameObject p in players)
                        {
                            Destroy(p);
                        }
                    }

                    if (screens != null)
                    {
                        foreach (GameObject s in screens)
                        {
                            Destroy(s);
                        }
                    }
                }
            }
            else
                Debug.LogError("Could not load scene with index " + levelIndex + ". Scene is not in build.");
        }
    }

    public IEnumerator SceneLoadingOperation(string levelName, bool deletePersistantObjects, bool IsOverworldCamNowMainCamera)
    {
        if (isTransitioning == false)
        {
            isTransitioning = true;

            AsyncOperation loadOperation = null;
            GameObject[] partyMembers = GameObject.FindGameObjectsWithTag("Player");

            // Check if the desired scene is valid
            if (SceneManager.GetSceneByName(levelName) != null)
            {
                // Only load the next scene if it is not the same as the current scene
                if (!SceneManager.GetSceneByName(levelName).Equals(SceneManager.GetActiveScene().name))
                {
                    loadOperation = SceneManager.LoadSceneAsync(levelName, LoadSceneMode.Single);
                    loadOperation.allowSceneActivation = false;
                }

                // Ensure time is not stopped
                Time.timeScale = 1;

                // wait for screen transition to complete
                do
                {
                    yield return null;
                } while (IsOpaque == false);

                // Wait for scene to load, if it is loading
                if (loadOperation != null)
                {
                    do
                    {
                        yield return null;
                    } while (loadOperation.progress < 0.9f);
                }
                GameObject[] screens = GameObject.FindGameObjectsWithTag("Screen");

                // Sets the overworld camera as either MainCamera or Untagged.
                if (screens != null)
                {
                    foreach (GameObject screen in screens)
                    {
                        GameObject cam = screen.GetComponentInChildren<Camera>().gameObject;
                        cam.tag = IsOverworldCamNowMainCamera ? "MainCamera" : "Untagged";
                        cam.GetComponent<AudioListener>().enabled = IsOverworldCamNowMainCamera;
                    }
                }

                // begin turning transparent
                ToTransparent();

                // Allow next scene to fully load, if it is loaded
                if (loadOperation != null) loadOperation.allowSceneActivation = true;

                yield return null;

                // Deletes objects marked as DontDestroyOnLoad, if desired. These are tagged as "Player" or "Screen".
                if (deletePersistantObjects == true)
                {
                    GameObject[] players = GameObject.FindGameObjectsWithTag("Player");

                    PartyMemberActor.partyMembers = null;
                    if (players != null)
                    {
                        foreach (GameObject p in players)
                        {
                            Destroy(p);
                        }
                    }

                    if (screens != null)
                    {
                        foreach (GameObject s in screens)
                        {
                            Destroy(s);
                        }
                    }
                }
            }
            else
                Debug.LogError("Could not load scene with name " + levelName + ". Scene is not in build.");
        }
    }

    private void OnDestroy()
    {
        Teleporter.DimScreen -= ToOpaque;
        Teleporter.UndimScreen -= ToTransparent;
        Enemy.StartBattle -= FightTransition;
        RunAbility.EndBattle -= EscapeTransition;
    }
}
