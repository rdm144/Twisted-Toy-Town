using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Battle_Actor : MonoBehaviour
{
    public int maxHealth;
    public int health;
    public int partyIndex;
    public bool isDead;
    public bool isTakingTurn;
    public List<string> abilities;
    public static Turn_Manager tm;
    public static GameObject battleUI;
    public Battle_Animator battleAnim;

    // Start is called before the first frame update
    void Start()
    {
        InitializeActor();
    }

    public virtual void InitializeActor()
    {
        isDead = isTakingTurn = false;

        if (tm == null)
            tm = GameObject.FindGameObjectWithTag("Turn Manager").GetComponent<Turn_Manager>();

        if(battleUI == null)
            battleUI = GameObject.FindGameObjectWithTag("Battle UI");

        TryGetComponent(out battleAnim);

        SetAbilitiesList();
    }

    protected virtual void SetAbilitiesList()
    {
        // Load abilities from a save file, if available
        abilities = new List<string>
        {
            "Attack",
            /*"Heal",*/
            "WideAttack"/*,
            "PartyHeal"*/
        };
    }

    public virtual void TakeDamage(int damage)
    {
        health = Mathf.Clamp(health - damage, 0, maxHealth);

        DamageNumberSpawner.createDamageNumber?.Invoke(transform.position, damage);

        if(health == 0) 
        {
            isDead = true;
            Debug.Log(name + " has died!");
            GetComponent<BoxCollider>().enabled = false;

            // Play death animation

            // Remove self from the turn order
            tm.RemoveFromTurnOrder(this);
            foreach(Transform t in transform)
            {
                t.gameObject.SetActive(false);
            }
            if(TryGetComponent(out MeshRenderer mesh))
                mesh.enabled = false;
        }
    }

    // Called from the Turn_Manager script to start this actor's turn.
    public virtual void StartTurn()
    {
        if(isTakingTurn == false)
        {
            isTakingTurn = true;
            StartCoroutine("ActorTurn");
        }
    }

    protected virtual IEnumerator ActorTurn()
    {
        yield return new WaitForFixedUpdate();

        Ability decision = GetDecision();

        List<Battle_Actor> targetList = GetTargets(decision);

        decision.SetTarget(targetList);

        Debug.Log(name + " uses " + decision.GetName() + " on " + PrintList(targetList));

        // Assign this object as the caster of this ability
        decision.SetCaster(this);

        // Play the ability's animation. Abilities cannot run their own coroutines.
        decision.PlayAnimation();
        StartCoroutine(decision.AbilityAnimation());

        yield return new WaitUntil(() => decision.IsPlayingAnimation() == false);

        yield return new WaitForFixedUpdate();
        //Destroy(decision);

        EndingPhase();
    }

    protected virtual Ability GetDecision()
    {
        // pick attack, skill, or run
        // If player, tell the UI what options to be filled with, and wait for a response.

        // picks an ability at random
        string key = "Attack";
        if (abilities.Count > 0)
        {
            int r = UnityEngine.Random.Range(0, abilities.Count);
            key = abilities[r];
        }

        // Get ability using the string key
        Ability chosenAbility;

        if (!AbilitiesTable.table.TryGetValue(key, out chosenAbility))
            chosenAbility = new AttackAbility();

        return chosenAbility;
    }

    protected virtual Ability GetDecision(string key)
    {
        // pick attack, skill, or run
        // If player, tell the UI what options to be filled with, and wait for a response.

        // picks an ability at random
        key = "Attack";
        if (abilities.Count > 0)
        {
            int r = UnityEngine.Random.Range(0, abilities.Count);
            key = abilities[r];
        }

        // Get ability using the string key
        Ability chosenAbility;

        if (!AbilitiesTable.table.TryGetValue(key, out chosenAbility))
            chosenAbility = new AttackAbility();

        return chosenAbility;
    }

    protected virtual List<Battle_Actor> GetTargets(Ability a)
    {
        // pick a target
        // If player, tell the UI the number of targets and the target type, then wait for a response.

        List<Battle_Actor> partyList = tm.GetAliveTargetableParty(this, a.GetTargetType());

        if (a.GetTargetType() == TargetType.FriendlyParty || a.GetTargetType() == TargetType.OpponentParty)
            return partyList;
        else
        {
            int rand = UnityEngine.Random.Range(0, partyList.Count);
            return new List<Battle_Actor> { partyList[rand] };
        }
    }

    protected virtual void EndingPhase()
    {
        // counter attacks, poison, status effects expiring, etc.
        
        tm.CheckVictory();

        if(tm.IsBattleFinished() == false)
        {
            isTakingTurn = false;
            tm.AdvanceTurn();
        }
        else
        {
            // Battle is over
        }
    }

    protected string PrintList(List<Battle_Actor> list)
    {
        foreach(Battle_Actor actor in list)
        {
            return actor.name + " ";
        }
        return "";
    }
}
