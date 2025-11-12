using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Mr_Meaty : NPC
{
    private void Start()
    {
        NPCStart();
    }

    public override void Interact()
    {
        if(!isInteracting)
        {
            isInteracting = true;
            StartCoroutine("CutsceneScript");
        }
    }

    IEnumerator CutsceneScript()
    {
        // Initialize the name tag
        dialogUI.SetNameTagText("Mr. Meaty");

        // Start the dialog ui
        dialogUI.StartDialogUI();

        // Roll text
        yield return dialogUI.RollText("All god's creatures, *fresh off the grill! *So come on down to Mr. Meaty, *where friends meet to eat, *MEAT!");

        dialogUI.SetNameTagText("doofus");
        yield return dialogUI.RollText("Save you game?");

        // End dialog
        dialogUI.EndDialogUI();

        isInteracting = false;
    }
}
