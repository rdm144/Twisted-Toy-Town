using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Player_Battle_Actor : Battle_Actor
{
    public static Player_Battle_Input pbi;
    public static Target_Reticle tr;
    private bool resetTurn;
    
    // Start is called before the first frame update
    void Start()
    {
        InitializeActor();
        if (pbi == null)
            pbi = GameObject.FindGameObjectWithTag("Player Battle Input").GetComponent<Player_Battle_Input>();
        if (tr == null)
            tr = GameObject.FindGameObjectWithTag("Target Reticle").GetComponent<Target_Reticle>();

        resetTurn = false;
        SelectionMenu.CancelAbility += () => resetTurn = true;
    }

    protected override void SetAbilitiesList()
    {
        abilities = new List<string>
        {
            "Attack", // First ability will be assigned to the default "Attack" button
            "WideAttack",
            "Heal",
            "PartyHeal"
        };
    }

    public override void TakeDamage(int damage)
    {
        health = Mathf.Clamp(health - damage, 0, maxHealth);

        // Invoke UI to update
        PlayerStatus.updateBattleUI?.Invoke(this);

        // Create a damage number
        DamageNumberSpawner.createDamageNumber?.Invoke(transform.position, damage);

        if (health == 0)
        {
            isDead = true;
            Debug.Log(name + " has died!");
            GetComponent<BoxCollider>().enabled = false;

            // Play death animation
            battleAnim.PlayDeathAnimation();

            // Remove self from the turn order
            tm.RemoveFromTurnOrder(this);

            /*
            foreach (Transform t in transform)
            {
                t.gameObject.SetActive(false);
            }
            */
        }
        else if(Math.Sign(damage) > 0 && health > 0) // play hit reel if the incoming damage was not a heal
            battleAnim.PlayHitReelAnimation();
    }

    protected override IEnumerator ActorTurn()
    {
        SelectionMenu select = battleUI.GetComponentInChildren<SelectionMenu>();
        Ability decision;

        // Wait a frame
        yield return new WaitForFixedUpdate();

        // Set the attack button
        select.SetAttackButton(abilities[0]);

        // Fill the skills menu
        select.PopulateMenu(select.SubMenuContent.transform, abilities);

        // Wait a frame
        yield return new WaitForFixedUpdate();

        // Open the main menu
        select.OpenMainMenu();

        do
        {
            resetTurn = false;

            // Reset the current ability (just in case)
            pbi.chosenAbility = null;

            // Wait for player to select an ability from the UI. UI will alert pbi of the selection.
            yield return new WaitUntil(() => pbi.chosenAbility != null);

            // Get the selected ability object
            decision = pbi.chosenAbility;

            // Get list of potential targets from the turn manager
            List<Battle_Actor> potentialTargets = GetTargets(decision);

            // Send potential targets to player_battle_input to spawn target reticle(s)
            pbi.CreateTargets(potentialTargets);

            // Reset the current target (just in case)
            pbi.chosenTarget.Clear();

            yield return new WaitForFixedUpdate();

            // Wait for player to select a target
            yield return new WaitUntil(() => pbi.chosenTarget.Count > 0 || resetTurn == true);

        } while (resetTurn == true);

        select.CloseAllMenus();

        // Get the selected target list
        List<Battle_Actor> targetList = pbi.chosenTarget;

        // Mark the list as the ability's target
        decision.SetTarget(targetList);

        // Assign this object as the caster of this ability
        decision.SetCaster(this);

        Debug.Log(name + " uses " + decision.GetName() + " on " + PrintList(targetList));

        // Play the ability's animation. Abilities cannot run their own coroutines.
        decision.PlayAnimation();
        StartCoroutine(decision.AbilityAnimation());

        // Wait until the animation has finished
        yield return new WaitUntil(() => decision.IsPlayingAnimation() == false);

        // Check ammunition count

        yield return new WaitForFixedUpdate();

        EndingPhase();
    }

    protected override List<Battle_Actor> GetTargets(Ability a)
    {
        return tm.GetAliveTargetableParty(this, a.GetTargetType());
    }
}
