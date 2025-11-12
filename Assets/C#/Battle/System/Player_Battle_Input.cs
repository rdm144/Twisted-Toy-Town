using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Player_Battle_Input : MonoBehaviour
{
    public Ability chosenAbility;
    public List<Battle_Actor> chosenTarget;
    Target_Reticle tr;

    // Start is called before the first frame update
    void Start()
    {
        if(tr == null)
            tr = GameObject.FindGameObjectWithTag("Target Reticle").GetComponent<Target_Reticle>();

        Target_Reticle.SetSingleTarget += this.SetSingleTarget;
        Target_Reticle.SetMultiTarget += this.SetMultiTarget;

        chosenAbility = null;
        chosenTarget = new List<Battle_Actor>(); ;
    }

    // Called by buttons to get an ability object from the ability table using a key
    public void GetAbility(string key)
    {
        if (!AbilitiesTable.table.TryGetValue(key, out chosenAbility))
            chosenAbility = new AttackAbility();
    }

    public void CreateTargets(List<Battle_Actor> targets)
    {
        // Send actor list to target reticle
        tr.currentList = targets;

        // Tell reticle to copy itself for every target, if needed
        if (chosenAbility.GetTargetType() == TargetType.FriendlyParty ||
            chosenAbility.GetTargetType() == TargetType.OpponentParty)
        {
            tr.isSingleTarget = false;
        }
        else
            tr.isSingleTarget = true;

        // activate target reticle
        tr.TurnOn();
    }

    public void SetSingleTarget(Battle_Actor target)
    {
        tr.TurnOff();
        chosenTarget.Add(target);
    }

    void SetMultiTarget(List<Battle_Actor> targets)
    {
        tr.TurnOff();
        chosenTarget = targets;
    }

    private void OnDestroy()
    {
        Target_Reticle.SetSingleTarget -= this.SetSingleTarget;
        Target_Reticle.SetMultiTarget -= this.SetMultiTarget;
    }
}
