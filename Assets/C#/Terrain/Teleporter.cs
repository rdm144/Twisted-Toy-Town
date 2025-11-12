using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using System;
using static UnityEngine.Rendering.DebugUI;
using Unity.VisualScripting;

public class Teleporter : MonoBehaviour
{
    public string LevelName;
    public Vector2 SpawnPoint;
    public static event Action DimScreen;
    public static event Action UndimScreen;
    bool isLoading;
    public Vector3Int SpawnDirection;
    // 0:up, 90:left, 180:down, 270:right

    // Start is called before the first frame update
    void Start()
    {
        isLoading = false;
        SpawnDirection = CalculateSpawnDirection();
    }

    Vector3Int CalculateSpawnDirection()
    {
        // Get current Z rotation
        float zRot = transform.parent.rotation.eulerAngles.z;

        // Normalize value between 0 and 360
        zRot = zRot % 360;
        if (zRot < 0)
            zRot += 360;

        // Round to nearest multiple of 90
        zRot = RoundToNearest(zRot, 90);

        // Determine direction vector
        switch (zRot)
        {
            case 0:
                return Vector3Int.forward;
            case 90:
                return Vector3Int.left;
            case 180:
                return Vector3Int.back;
            case 270:
                return Vector3Int.right;
            default:
                return Vector3Int.forward;
        }
    }

    IEnumerator LoadLevel()
    {
        // Start screen transition
        if (DimScreen != null) DimScreen.Invoke();

        AsyncOperation loadOperation = null;
        GameObject[] partyMembers = GameObject.FindGameObjectsWithTag("Player");

        // Check if the desired scene is valid
        if (SceneManager.GetSceneByName(LevelName) != null)
        {
            // Only load the next scene if it is not the same as the current scene
            if (!SceneManager.GetSceneByName(LevelName).Equals(SceneManager.GetActiveScene()))
            {
                loadOperation = SceneManager.LoadSceneAsync(LevelName, LoadSceneMode.Single);
                loadOperation.allowSceneActivation = false;
            }

            // Ensure time is not stopped
            Time.timeScale = 1;

            // Stop all party members from operating
            yield return FreezePartyMembers(partyMembers);

            // wait for screen transition
            yield return new WaitForSeconds(0.25F);

            // Wait for scene to load, if it is loading
            if (loadOperation != null) yield return WaitToLoad(loadOperation);

            // Reposition all party members
            yield return RepositionPartyMembers(partyMembers);

            // Unfreeze all party members
            yield return UnfreezePartyMembers(partyMembers);

            // Start screen transition
            if (UndimScreen != null) UndimScreen.Invoke();

            // Allow next scene to fully load, if it is loaded
            if (loadOperation != null) loadOperation.allowSceneActivation = true;
        }

        // Allow this teleporter to operate again
        isLoading = false;
    }

    IEnumerator FreezePartyMembers(GameObject[] partyMembers)
    {
        foreach (GameObject partyMember in partyMembers)
        {
            partyMember.GetComponent<Actor>().canOperate = false;
            partyMember.GetComponent<Actor>().isMoving = false;
            yield return new WaitForFixedUpdate();
        }
    }

    IEnumerator WaitToLoad(AsyncOperation loadOperation)
    {
        do
        {
            yield return new WaitForFixedUpdate();
        } while (loadOperation.progress < 0.9f);
    }

    IEnumerator RepositionPartyMembers(GameObject[] partyMembers)
    {
        foreach (GameObject partyMember in partyMembers)
        {
            partyMember.transform.position = SpawnPoint;
            partyMember.GetComponent<Actor>().Direction = SpawnDirection;
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

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.transform.parent.tag.Equals("Player"))
        {
            if(SceneManager.GetSceneByName(LevelName) != null)
            {
                if(isLoading == false)
                {
                    isLoading = true;
                    StartCoroutine("LoadLevel");
                }
            }
        }
    }

    float RoundToNearest(float value, float multiple)
    {
        return Mathf.Round(value / multiple) * multiple;
    }
}
