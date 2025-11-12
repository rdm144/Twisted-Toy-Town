using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WideAttackAbility : Ability
{
    public WideAttackAbility()
    {
        abilityName = "WideAttack";
        deltaHP = 1;
        description = "Deals " + deltaHP + " damage to the enemy party.";
        targetType = TargetType.OpponentParty;
    }

    public override IEnumerator AbilityAnimation()
    {
        // Make the attacker shoot
        if (caster.battleAnim ?? false)
            caster.battleAnim.PlayShootAnimation();

        foreach (Battle_Actor target in currentTargets)
        {
            // Spawn a hitspark on the target
            SpawnHitSpark(target);

            // Deal damage to the target
            target.TakeDamage(deltaHP);
        }

        // Wait
        yield return new WaitForSeconds(0.25f);

        // End the animation
        isPlayingAnimation = false;
    }
}
