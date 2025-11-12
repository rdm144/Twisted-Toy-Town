using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public static class AbilitiesTable
{
    public static Dictionary<string, Ability> table = new()
    {
        {"Attack", new AttackAbility()},
        {"WideAttack", new WideAttackAbility()},
        {"Heal", new HealAbility()},
        {"PartyHeal", new PartyHealAbility()},
        {"Run", new RunAbility()},
    };
}
