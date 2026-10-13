using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class Pause_UI : MonoBehaviour
{
    protected enum MenuType { Base, Dev, Option, Controls, Audio}

    protected MenuType currentMenu;
    protected KeyCode PauseKey;
    public bool isPaused;

    public GameObject BaseMenu;
    protected Dialog_UI dui;
    protected List<Actor> actorsToBeUnpaused;
    public StageInfo stageInfo;
    public Camera myCam;
    public int moveHorizontal { get; protected set; }
    public int moveVertical { get; protected set; }
    public KeyCode LeftKey, RightKey, UpKey, DownKey;

    // Start is called before the first frame update
    void Start()
    {
        BaseMenu.GetComponent<Base_Menu>().pui = this;
        LeftKey = KeyCode.A; // Hard-coded keybinds. Remove later.
        RightKey = KeyCode.D;
        UpKey = KeyCode.W;
        DownKey = KeyCode.S;
        PauseKey = KeyCode.Escape;
        dui = GameObject.FindGameObjectWithTag("Dialog_UI").GetComponent<Dialog_UI>();
        actorsToBeUnpaused = new List<Actor>();
        UnpauseGame();

        GameObject stageInfoGameObject = GameObject.FindGameObjectWithTag("Stage Info");
        if (stageInfoGameObject != null)
            stageInfoGameObject.TryGetComponent(out stageInfo);
    }

    // Update is called once per frame
    void Update()
    {
        GetDirectionalInput();

        if(GetPauseInput() == true && Camera.main == myCam)
        {
            ToggleMenus();
        }
    }

    protected virtual void GetDirectionalInput()
    {
        // Left or Right input
        if (Input.GetKeyDown(LeftKey) && !Input.GetKeyDown(RightKey))
            moveHorizontal = -1;
        else if (Input.GetKeyDown(RightKey) && !Input.GetKey(LeftKey))
            moveHorizontal = 1;
        else
            moveHorizontal = 0;

        // Up or Down input
        if (Input.GetKeyDown(DownKey) && !Input.GetKeyDown(UpKey))
            moveVertical = -1;
        else if (Input.GetKeyDown(UpKey) && !Input.GetKeyDown(DownKey))
            moveVertical = 1;
        else
            moveVertical = 0;

        // Prevent diagonals
        if (moveHorizontal != 0 && moveVertical != 0)
            moveVertical = 0;
    }

    protected virtual bool GetPauseInput()
    {
        return Input.GetKeyDown(PauseKey);
    }

    protected virtual void PauseGame()
    {
        if(BaseMenu != null)
            BaseMenu.SetActive(true);
        
        currentMenu = MenuType.Base;
        isPaused = true;

        // Stop time
        Time.timeScale = 0;
        
        foreach(Actor act in FindObjectsByType<Actor>(FindObjectsSortMode.None))
        {
            if (act.canOperate == true)
                actorsToBeUnpaused.Add(act);
            act.canOperate = false;
        }
        
    }

    public virtual void UnpauseGame()
    {
        if (BaseMenu != null)
            BaseMenu.SetActive(false);

        currentMenu = MenuType.Base;
        isPaused = false;

        // Resume time
        Time.timeScale = 1;
        
        if(dui != null)
        {
            if (!dui.isTalking)
            {
                foreach (Actor act in actorsToBeUnpaused)
                {
                    act.canOperate = true;
                }
            }
        }
        else
        {
            foreach (Actor act in actorsToBeUnpaused)
            {
                act.canOperate = true;
            }
        }

        actorsToBeUnpaused.Clear();
    }

    protected virtual void ToggleMenus()
    {
        if (isPaused == false)
        {
            PauseGame();
        }
        else
        {
            switch (currentMenu)
            {
                case MenuType.Base:
                    UnpauseGame();
                    break;
                case MenuType.Dev:
                    // Close dev menu
                    // open base menu
                    break;
            }
        }
    }
}
