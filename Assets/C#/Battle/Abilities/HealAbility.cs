using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using static UnityEngine.GraphicsBuffer;

public class HealAbility : Ability
{
    public static int HealPercentage = 45;

    public HealAbility()
    {
        abilityName = "Heal";
        description = "Heals " + HealPercentage + "% HP to a single friendly target.";
        targetType = TargetType.FriendlySingle;
    }

    private int CalculateHealAmount(Battle_Actor target)
    {
        return Mathf.RoundToInt((target.maxHealth * HealPercentage) / 100);
    }

    public override IEnumerator AbilityAnimation()
    {
        //Debug.Log(caster.transform.name + ": Heal called!");
        // Play the caster's heal animation
        if (caster.battleAnim??false)
            caster.battleAnim.PlayHealAnimation();

        // Place a heal-casting effect over the caster
        //SpawnObjectFromResources("Prefabs/Effects/Heal FX/medic heal fx", caster.transform.position);
        SpawnObjectFromResourcesAtTargetBottom("Prefabs/Effects/Heal FX/medic heal fx", caster);

        yield return new WaitForSeconds(1f);

        foreach (Battle_Actor target in currentTargets)
        {
            // Place a heal effect over the target. If the target's bottom face coordinate was not found, use the target's world position instead.
            SpawnObjectFromResourcesAtTargetBottom("Prefabs/Effects/Heal FX/medic heal receive", target);

            /*
            // Attempt to find the bottom of the target's collider
            Vector3 bottomWorldCoordinate = FindBottomOfTargetsCollider(target);

            // Place a heal effect over the target. If the target's bottom face coordinate was not found, use the target's world position instead.
            SpawnObjectFromResources("Prefabs/Effects/Heal FX/medic heal receive", (bottomWorldCoordinate == Vector3.zero)? target.transform.position : bottomWorldCoordinate);
            */

            yield return new WaitForSeconds(1f);

            // Heal the target
            deltaHP = CalculateHealAmount(target);
            target.TakeDamage(-deltaHP);
        }

        // Wait
        yield return new WaitForSeconds(0.5f);

        // End the animation
        isPlayingAnimation = false;
    }
}
