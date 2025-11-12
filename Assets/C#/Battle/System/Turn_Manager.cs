using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// Keeps track of player and enemy turns.
/// </summary>

public class Turn_Manager : MonoBehaviour
{
    public List<Battle_Actor> turnOrder;
    public List<Battle_Actor> playerParty;
    public List<Battle_Actor> enemyParty;

    private Battle_Actor currentTurnActor;

    private int playersAlive;
    private int enemiesAlive;

    private bool isBattleFinished;

    public static event Action PlayerVictoryEvent;
    public static event Action PlayerDefeatEvent;

    private void Awake()
    {
        gameObject.tag = "Turn Manager"; // Preemptively set the gameobject's tag, just in case
    }

    private void Start()
    {
        turnOrder = new List<Battle_Actor>();
        playerParty = new List<Battle_Actor>();
        enemyParty = new List<Battle_Actor>();
        playersAlive = enemiesAlive = 0;
        isBattleFinished = false;

        // Wait for all actors to join the fight
        StartCoroutine("PrepareBattleScene");
    }

    // Get all battle actors and prepare the turn order
    IEnumerator PrepareBattleScene()
    {
        // Wait a frame for the Actor Loader to load in all actors
        yield return new WaitForFixedUpdate();

        // Find all battle actors
        Battle_Actor[] actors = FindObjectsByType<Battle_Actor>(FindObjectsSortMode.None);
        foreach(Battle_Actor actor in actors)
        {
            if (actor.gameObject.TryGetComponent(out Player_Battle_Actor pba))
            {
                AddToPlayerParty(pba);

                // Update the UI with said player's info
                PlayerStatus.updateBattleUI?.Invoke(pba);
            }
            else
                AddToEnemyParty(actor);
        }

        // Sort the parties by their party index
        playerParty = SortBattleActors.QuickSort(playerParty);
        enemyParty = SortBattleActors.QuickSort(enemyParty);

        // Initialize the turn order
        InitializeTurnOrder();
        Debug.Log("Turns initialized.");
        yield return null;

        // Begin the fight
        Debug.Log("Battle Start.");
        currentTurnActor.StartTurn();
        yield return null;
    }

    // Used by Actors to determine the end of the battle
    public bool IsBattleFinished()
    {
        return isBattleFinished;
    }

    // Actors should use this to check if it is currently their turn
    public bool IsMyTurn(Battle_Actor Me)
    {
        return currentTurnActor == Me;
    }

    // Sets the current actor to the next actor in the turn order list, or resets to the first one if at the end of the list 
    public void AdvanceTurn()
    {
        int index = turnOrder.IndexOf(currentTurnActor);

        if (index + 1 < turnOrder.Count)
        {
            currentTurnActor = turnOrder[index + 1];
        }
        else
        {
            currentTurnActor = turnOrder[0];
        }

        currentTurnActor.StartTurn();
    }

    // Fill the turn order list with players and enemies, with players going first
    private void InitializeTurnOrder()
    {
        turnOrder.Clear();

        // Add all players to top of the turn order
        foreach (Battle_Actor actor in playerParty)
        {
            if (actor != null)
            {
                if (actor.isDead == false) turnOrder.Add(actor);
                playersAlive++;
            }
        }

        // Add all enemy actors afterwards
        foreach (Battle_Actor actor in enemyParty)
        {
            if (actor != null)
            {
                if (actor.isDead == false) turnOrder.Add(actor);
                enemiesAlive++;
            }
        }

        // Set the first actor as the currently acting actor
        if (turnOrder.Count > 0)
            currentTurnActor = turnOrder[0];
    }

    // Actors should call this mid-battle when summoning or reviving friendlies
    public void RefreshTurnOrder()
    {
        turnOrder.Clear();
        playersAlive = enemiesAlive = 0;

        // Add all players to top of the turn order
        foreach (Battle_Actor actor in playerParty)
        {
            if (actor != null)
            {
                if (actor.isDead == false) turnOrder.Add(actor);
                playersAlive++;
            }
        }

        // Add all enemy actors afterwards
        foreach (Battle_Actor actor in enemyParty)
        {
            if (actor != null)
            {
                if (actor.isDead == false) turnOrder.Add(actor);
                enemiesAlive++;
            }
        }

        // Set the first actor as the currently acting actor
        if (turnOrder.Count > 0)
        {
            // Reset the current actor to the top-most one if said actor is not on the turn order list
            if (!turnOrder.Contains(currentTurnActor))
                currentTurnActor = turnOrder[0];
        }
    }

    // Player actors should call this to enter the party
    public void AddToPlayerParty(Battle_Actor newActor)
    {
        playerParty.Add(newActor);
    }

    // Enemy actors should call this to enter the party
    public void AddToEnemyParty(Battle_Actor newActor)
    {
        enemyParty.Add(newActor);
    }

    // Removes an actor from the current turn order. Called by newly-killed actors
    public void RemoveFromTurnOrder(Battle_Actor deadActor)
    {
        if (turnOrder.Contains(deadActor))
        {
            turnOrder.Remove(deadActor);
        }

        if (enemyParty.Contains(deadActor))
            enemiesAlive--;
        else if (playerParty.Contains(deadActor))
            playersAlive--;
    }

    // Checks the number of currently alive actors for each party. Called at the end of an actor's turn
    public void CheckVictory()
    {
        if (enemiesAlive <= 0)
        {
            isBattleFinished = true;
            PlayerVictoryEvent?.Invoke();
            Debug.Log("Player victory!");
        }
        else if (playersAlive <= 0)
        {
            isBattleFinished = true;
            PlayerDefeatEvent?.Invoke();
            Debug.Log("Player defeat!");
        }
    }

    // Gets a list of live party members for ability targeting. Called by actors when requesting a target
    public List<Battle_Actor> GetAliveTargetableParty(Battle_Actor actor, TargetType targets)
    {
        List<Battle_Actor> aliveTargetableList = new List<Battle_Actor>();

        if (playerParty.Contains(actor))
        {
            if (targets == TargetType.FriendlyParty || targets == TargetType.FriendlySingle)
            {
                foreach (Battle_Actor a in playerParty)
                {
                    if (!a.isDead)
                        aliveTargetableList.Add(a);
                }
            }
            else
            {
                foreach (Battle_Actor a in enemyParty)
                {
                    if (!a.isDead)
                        aliveTargetableList.Add(a);
                }
            }
        }
        else if (enemyParty.Contains(actor))
        {
            if (targets == TargetType.FriendlyParty || targets == TargetType.FriendlySingle)
            {
                foreach (Battle_Actor a in enemyParty)
                {
                    if (!a.isDead)
                        aliveTargetableList.Add(a);
                }
            }
            else
            {
                foreach (Battle_Actor a in playerParty)
                {
                    if (!a.isDead)
                        aliveTargetableList.Add(a);
                }
            }
        }

        return aliveTargetableList;
    }

    // Gets a list of dead party members for ability targeting. Called by actors when requesting a target
    public List<Battle_Actor> GetDeadTargetableParty(Battle_Actor actor, TargetType targets)
    {
        List<Battle_Actor> deadTargetableList = new List<Battle_Actor>();

        if (playerParty.Contains(actor))
        {
            if (targets == TargetType.FriendlyParty || targets == TargetType.FriendlySingle)
            {
                foreach (Battle_Actor a in playerParty)
                {
                    if (a.isDead)
                        deadTargetableList.Add(a);
                }
            }
            else
            {
                foreach (Battle_Actor a in enemyParty)
                {
                    if (a.isDead)
                        deadTargetableList.Add(a);
                }
            }
        }
        else if (enemyParty.Contains(actor))
        {
            if (targets == TargetType.FriendlyParty || targets == TargetType.FriendlySingle)
            {
                foreach (Battle_Actor a in enemyParty)
                {
                    if (a.isDead)
                        deadTargetableList.Add(a);
                }
            }
            else
            {
                foreach (Battle_Actor a in playerParty)
                {
                    if (a.isDead)
                        deadTargetableList.Add(a);
                }
            }
        }

        return deadTargetableList;
    }

    // Gets the index of a player party member from the party list
    public int GetPlayerIndexFromTurnOrder(Player_Battle_Actor playerActor)
    {
        for (int i = 0; i < playerParty.Count; i++)
        {
            if (playerParty[i] == playerActor)
                return i;
        }

        Debug.LogError("Could not find " + playerActor.transform.name + " within the Turn Manager's player party list!");
        return -1;
    }
}
