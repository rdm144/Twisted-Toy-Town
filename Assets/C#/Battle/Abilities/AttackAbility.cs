using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class AttackAbility : Ability
{
    public AttackAbility()
    {
        abilityName = "Attack";
        deltaHP = 2;
        description = "Deals " + deltaHP + " damage to a single target.";
        targetType = TargetType.OpponentSingle;
    }

    public override IEnumerator AbilityAnimation()
    {
        // Make the attacker shoot
        if (caster.battleAnim??false)
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
