using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using UnityEngine.UI;
using System;

/// <summary>
/// At round start, each PlayerStatus object makes itself invisible. Later, the object is made visible via an invoke by TurnManager.cs
/// </summary>
public class PlayerStatus : MonoBehaviour
{
    private static CombatUI_Container cuic;
    private GameObject combatUI;

    //private TextMeshProUGUI nameTag;
    private Slider hpBar;
    private TextMeshProUGUI hpText;

    public int partyMemberNumber;
    private const float ACTIVE_ALPHA = 0.9568627f;
    private const float INACTIVE_ALPHA = 0.5529412f;

    public delegate void UpdateBattleUI(Player_Battle_Actor pba);
    public delegate void UpdatePauseUI(PartyMember pba);
    public static UpdateBattleUI updateBattleUI;
    public static UpdatePauseUI updatePauseUI;

    // Start is called before the first frame update
    void Start()
    {
        combatUI = null;
        //nameTag = null;
        hpBar = null;
        hpText = null;
        
        cuic = transform.parent.GetComponent<CombatUI_Container>();

        updateBattleUI += UpdateUI;
        //updatePauseUI += UpdateUI;
    }

    void UpdateUI(Player_Battle_Actor playerActor)
    {
        if(playerActor.partyIndex == partyMemberNumber)
        {
            if (combatUI == null)
            {
                // Create a combat ui instance based on the actor's name
                GameObject combatUI_Prefab = cuic.GetCombatUI(playerActor.gameObject.name);

                if (combatUI_Prefab != null)
                {
                    combatUI = GameObject.Instantiate(combatUI_Prefab, transform);

                    // Get necessary objects
                    hpBar = combatUI.transform.Find("HP Bar").GetComponent<Slider>();
                    hpText = hpBar.transform.Find("Text").GetComponent<TextMeshProUGUI>();
                    //nameTag = combatUI.transform.Find("Name Tag/Text").GetComponent<TextMeshProUGUI>();
                }
                else 
                {
                    Debug.LogError("Could not find combat ui prefab for party member number " + partyMemberNumber);
                }
            }

            // Update the combat ui
            if(hpBar == null)
                hpBar = combatUI.transform.Find("HP Bar").GetComponent<Slider>();

            hpBar.value = (float)((float)playerActor.health / (float)playerActor.maxHealth);

            if(hpText == null)
                hpText = hpBar.transform.Find("Text").GetComponent<TextMeshProUGUI>();

            //Debug.Log("Setting value to " + playerActor.health + " / " + playerActor.maxHealth + " = " + hpBar.value);
            hpText.text = playerActor.health + "/" + playerActor.maxHealth;
            
            //nameTag.text = playerActor.gameObject.name;
        }
    }

    void UpdateUI(PartyMember partyMember)
    {
        //Debug.Log("Updating ui for " + partyMember.name);
        if (partyMember.partyIndex == partyMemberNumber)
        {
            if (combatUI == null)
            {
                // Create a combat ui instance based on the actor's name
                GameObject combatUI_Prefab = cuic.GetCombatUI(partyMember.name);

                if (combatUI_Prefab != null)
                {
                    combatUI = GameObject.Instantiate(combatUI_Prefab, transform);

                    // Get necessary objects
                    hpBar = combatUI.transform.Find("HP Bar").GetComponent<Slider>();
                    hpText = hpBar.transform.Find("Text").GetComponent<TextMeshProUGUI>();
                    //nameTag = combatUI.transform.Find("Name Tag/Text").GetComponent<TextMeshProUGUI>();
                }
                else
                {
                    Debug.LogError("Could not find combat ui prefab for party member number " + partyMemberNumber);
                }
            }

            // Update the combat ui
            if (hpBar == null)
                hpBar = combatUI.transform.Find("HP Bar").GetComponent<Slider>();

            hpBar.value = (float)((float)partyMember.health / (float)partyMember.maxHealth);

            if (hpText == null)
                hpText = hpBar.transform.Find("Text").GetComponent<TextMeshProUGUI>();

            //Debug.Log("Setting value to " + partyMember.health + " / " + partyMember.maxHealth + " = " + hpBar.value);
            hpText.text = partyMember.health + "/" + partyMember.maxHealth;

            //nameTag.text = partyMember.gameObject.name;
        }
    }

    

    void DeactivateUIElement(int index)
    {
        if(index == partyMemberNumber && transform.childCount > 0)
        {
            transform.GetChild(0).gameObject.SetActive(false);
            /*
            //Debug.Log("Disabling UI for index " + partyMemberNumber);
            backgroundPanel.color = new Color(backgroundPanel.color.r, backgroundPanel.color.g, backgroundPanel.color.b, INACTIVE_ALPHA);
            nameTag.text = "";
            nameTag.transform.parent.gameObject.SetActive(false);
            hpBar.value = 0;
            hpText.text =  "00/00";
            hpBar.gameObject.SetActive(false);
            */
        }
    }

    private void OnDestroy()
    {
        updateBattleUI -= UpdateUI;
        //updatePauseUI -= UpdateUI;
    }
}
