using System.Collections;
using System.Collections.Generic;
using UnityEngine;

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
        Debug.Log(caster.transform.name + ": Heal called!");
        // Play the caster's heal animation
        if (caster.battleAnim??false)
            caster.battleAnim.PlayHealAnimation();

        // Place a heal-casting effect over the caster
        SpawnObjectFromResources("Prefabs/Battle/Effects/medic heal fx", caster.transform.position);

        yield return new WaitForSeconds(1f);

        foreach (Battle_Actor target in currentTargets)
        {
            // Place a heal effect over the target
            SpawnObjectFromResources("Prefabs/Battle/Effects/medic heal receive", target.transform.position);

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
