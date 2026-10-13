using System;
using System.Collections.Generic;
using Unity.VectorGraphics;
using UnityEngine;
using UnityEngine.SceneManagement;

public class Battle_Pause_UI : Pause_UI
{
    List<Battle_Actor> battleActorsToBeUnpaused;

    // Start is called before the first frame update
    void Start()
    {
        BaseMenu.GetComponent<Base_Menu>().pui = this;
        LeftKey = KeyCode.A; // Hard-coded keybinds. Remove later.
        RightKey = KeyCode.D;
        UpKey = KeyCode.W;
        DownKey = KeyCode.S;
        PauseKey = KeyCode.Escape;
        battleActorsToBeUnpaused = new List<Battle_Actor>();
        UnpauseGame();

        GameObject stageInfoGameObject = GameObject.FindGameObjectWithTag("Stage Info");
        if (stageInfoGameObject != null)
            stageInfoGameObject.TryGetComponent(out stageInfo);
    }

    // Update is called once per frame
    void Update()
    {
        GetDirectionalInput();

        if (GetPauseInput() == true && Camera.main == myCam)
        {
            ToggleMenus();
        }
    }

    protected override void PauseGame()
    {
        if (BaseMenu != null)
            BaseMenu.SetActive(true);

        currentMenu = MenuType.Base;
        isPaused = true;

        // Stop time
        Time.timeScale = 0;

        foreach (Battle_Actor act in FindObjectsByType<Battle_Actor>(FindObjectsSortMode.None))
        {
            if (act.isDead == false)
                battleActorsToBeUnpaused.Add(act);
            act.isDead = true;
        }
    }

    public override void UnpauseGame()
    {
        if (BaseMenu != null)
            BaseMenu.SetActive(false);

        currentMenu = MenuType.Base;
        isPaused = false;

        foreach (Battle_Actor act in battleActorsToBeUnpaused)
        {
            act.isDead = false;
        }

        // Resume time
        Time.timeScale = 1;

        battleActorsToBeUnpaused.Clear();
    }

    public void RestartBattle()
    {
        // Start screen transition
        //devToolRestartBattle?.Invoke(SceneManager.GetActiveScene().name, false, false);
        SceneManager.LoadScene(SceneManager.GetActiveScene().name, LoadSceneMode.Single);
    }

    public void AutoWin()
    {
        foreach (Battle_Actor actor in Battle_Actor.tm.enemyParty)
        {
            actor.TakeDamage(int.MaxValue);
        }
        UnpauseGame();
        Battle_Actor.tm.CheckVictory();
    }

    public void AutoLose()
    {
        foreach (Battle_Actor actor in Battle_Actor.tm.playerParty)
        {
            actor.TakeDamage(int.MaxValue);
        }
        UnpauseGame();
        Battle_Actor.tm.CheckVictory();
    }
}
