using System.Collections;
using System.Collections.Generic;
using Unity.Burst.CompilerServices;
using UnityEngine;
using System;

public class Target_Reticle : MonoBehaviour
{
    public int index;
    public List<Battle_Actor> currentList;
    public bool On;
    public bool isSingleTarget;
    bool changeToNewTarget;
    const float Offset = 2;
    static Transform Cam;
    public Transform child;
    public string targetTag; // Used to speed up mouse-over only. Do not use for ANYTHING else!
    private List<GameObject> extraReticles; // holds extra reticles for multi-target abilities

    public static event Action<Battle_Actor> SetSingleTarget;
    public static event Action<List<Battle_Actor>> SetMultiTarget;

    // Start is called before the first frame update
    void Start()
    {
        //ListsRecieved = false;
        currentList = new List<Battle_Actor>();
        extraReticles = new List<GameObject>();
        if(child == null)
            child = transform.GetChild(0);
        Cam = Camera.main.transform;
        index = 0;
        isSingleTarget = true;
        changeToNewTarget = false;
        SelectionMenu.CancelAbility += TurnOff;
        //StartCoroutine(ChangeList("EnemyList"));
        TurnOff();
    }

    // Update is called once per frame
    void Update()
    {
        if (On == true)
        {
            // Update reticle placement if used for a single target
            if(changeToNewTarget && isSingleTarget)
            {
                PlaceReticle();
                transform.LookAt(Cam);
            }
         
            if(IsMouseOverTarget() == true && GetTargetFromMouse().gameObject.tag == targetTag)
            {
                Battle_Actor target = GetTargetFromMouse();
                ChangeTargetIndex(target); // Move reticle to the hovered target, if needed

                if (Input.GetMouseButtonDown(0) == true)
                {
                    // Shoot
                    if (isSingleTarget)
                        SetSingleTarget?.Invoke(currentList[index]);
                    else
                        SetMultiTarget?.Invoke(currentList);
                }
            }
            else if(Input.GetKeyDown(KeyCode.A))
            {
                ChangeTargetIndex(1);
            }
            else if (Input.GetKeyDown(KeyCode.D))
            {
                ChangeTargetIndex(-1);
            }

            if (Input.GetKeyDown(KeyCode.Space))
            {
                // Shoot
                if (isSingleTarget)
                    SetSingleTarget?.Invoke(currentList[index]);
                else
                    SetMultiTarget?.Invoke(currentList);
            }
        }
    }

    public void TurnOn()
    {
        On = true;
        index = 0;
        
        changeToNewTarget = true;
        targetTag = currentList[0].tag;

        if(isSingleTarget == false)
            PlaceMultipleReticlesOnParty();
    }

    public void TurnOff()
    {
        On = false;
        if (child == null)
            child = transform.GetChild(0);
        child.GetComponent<SpriteRenderer>().enabled = false;
        
        // Disable all extra reticles
        foreach(GameObject extra in extraReticles)
        {
            extra.SetActive(false);
        }

        DescriptionBox.hideDescription?.Invoke();
    }

    // Called by player input to move the reticle either up or down in the target list by 1
    public void ChangeTargetIndex(int offset)
    {
        if (offset > 1 || offset < -1)
        {
            Debug.LogError("Target Reticle will not skip over " + offset + " targets. Offset must be -1 thru 1.");
        }
        else
        {
            index += offset;
            if (index >= currentList.Count)
                index = 0;
            else if (index < 0)
                index = currentList.Count - 1;

            changeToNewTarget = true;
        }
    }

    // Jumps reticle to a specific actor
    public void ChangeTargetIndex(Battle_Actor target)
    {
        if (currentList.Contains(target))
        {
            if(index != currentList.IndexOf(target))
            {
                changeToNewTarget = true;
                index = currentList.IndexOf(target);
            }
        }
        else
        {
            Debug.Log("Method 'Target_Reticle.ChangeTarget()' cannot find target within the reticle's target list.");
        }
    }

    // Aligns reticle on current target
    void PlaceReticle()
    {
        Vector3 targetLoc;

        // Get the position of the target at a specific index
        if (index > -1 && currentList.Count > index)
            targetLoc = currentList[index].gameObject.transform.position;
        else
        {
            targetLoc = Vector3.zero;
            Debug.LogError("The current list does not contain an element at " + index);
        }

        // Place object 1 unit in front of the target, relative to the camera
        Vector3 CamLoc = Cam.position;
        Vector3 direction = (CamLoc - targetLoc).normalized;
        //Debug.DrawLine(CamLoc, enemyLoc);
        transform.position = targetLoc + direction * Offset;
        if (child == null)
            child = transform.GetChild(0);
        child.GetComponent<SpriteRenderer>().enabled = true;

        changeToNewTarget = false;

        // Update the descripton box with the target's name
        DescriptionBox.displayDescription?.Invoke(currentList[index].transform.name);
    }

    // Aligns an object on current target
    void PlaceReticle(Transform objectTransform, int listIndex)
    {
        Vector3 targetLoc = Vector3.zero;

        // Get the position of the target at a specific index
        if (listIndex > -1 && currentList.Count > listIndex)
            targetLoc = currentList[listIndex].gameObject.transform.position;
        else
            Debug.LogError("The current list does not contain an element at " + listIndex);

        // Place object 1 unit in front of the target, relative to the camera
        Vector3 CamLoc = Cam.position;
        Vector3 direction = (CamLoc - targetLoc).normalized;
        //Debug.DrawLine(CamLoc, enemyLoc);
        objectTransform.position = targetLoc + direction * Offset;
    }

    void PlaceMultipleReticlesOnParty()
    {
        if (child == null)
            child = transform.GetChild(0);
        child.GetComponent<SpriteRenderer>().enabled = true;

        if (child == null)
        {
            Debug.LogError("Target Reticle does not have a child sprite!");
            return;
        }

        // Spawn targetlist.count - 1 reticle sprites, if needed
        int extraReticlesNeeded = currentList.Count - 1;
        while(extraReticles.Count < extraReticlesNeeded)
        {
            GameObject extraReticle = Instantiate(child.gameObject, transform);
            extraReticles.Add(extraReticle);
        }

        // place this reticle on index 0
        PlaceReticle(transform, 0);
        transform.LookAt(Cam);
        
        // place subsequent reticles on index 1 through targetlist.count-1
        for (int i = 1; i < currentList.Count; i++) 
        {
            // Get the current reticle
            Transform extraReticle = extraReticles[i - 1].transform;
            
            // Make the reticle visible
            extraReticle.gameObject.SetActive(true);
            
            // Place the reticle in front of a target
            PlaceReticle(extraReticle, i);

            // set the reticle to look at the camera
            extraReticle.LookAt(Cam);
        }

        DescriptionBox.displayDescription?.Invoke("Entire Party");
    }

    bool IsMouseOverTarget()
    {
        Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
        RaycastHit hitInfo;

        if (Physics.Raycast(ray, out hitInfo, Mathf.Infinity, LayerMask.GetMask("BattleActor")))
            return true;
        else
            return false;
    }

    Battle_Actor GetTargetFromMouse()
    {
        Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
        RaycastHit hit;
        Physics.Raycast(ray, out hit, Mathf.Infinity, LayerMask.GetMask("BattleActor"));
        return hit.transform.GetComponent<Battle_Actor>();
    }

    private void OnDestroy()
    {
        SelectionMenu.CancelAbility -= TurnOff;
    }
}
