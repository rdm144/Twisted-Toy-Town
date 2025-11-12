using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;
using UnityEngine.UI;
using System;
using TMPro;
using UnityEngine.EventSystems;
using Unity.VisualScripting;

/// <summary>
/// A NOTE ON BUTTON DEBUGGING!
/// - Button listeners do not show up if created during runtime, as these are considered "non-persistent"!
/// - The button will still function, however, so simply not seeing it does not imply it's not working.
/// </summary>

public class SelectionMenu : MonoBehaviour
{
    public GameObject ButtonPrefab;
    public GameObject RedButtonPrefab;
    //public GameObject GreenButtonPrefab;

    public GameObject MainMenu;
    GameObject MainMenuContent;
    public GameObject SubMenu;
    public GameObject SubMenuContent { get; private set; }
    public GameObject CancelMenu;

    public Player_Battle_Input pbi;
    public Button AttackButton;
    public Button RunButton;

    public static event Action CancelAbility;
    private string prevMenu; // Used for cancel button operation

    // Start is called before the first frame update
    void Start()
    {
        SetRunButton();
        GetContentFromMenu();
        OpenMainMenu();
        prevMenu = "main";
    }

    void GetContentFromMenu()
    {
        MainMenuContent = MainMenu.transform.Find("Viewport/Content").gameObject;
        SubMenuContent = SubMenu.transform.Find("Viewport/Content").gameObject;
    }

    public void OpenSubMenu()
    {
        CloseAllMenus();
        SubMenu.SetActive(true);
        prevMenu = "sub";
    }

    public void OpenMainMenu()
    {
        CloseAllMenus();
        MainMenu.SetActive(true);
        prevMenu = "main";
    }

    public void CloseAllMenus()
    {
        MainMenu.SetActive(false);
        SubMenu.SetActive(false);
        CancelMenu.SetActive(false);
    }

    public void OpenCancelMenu()
    {
        CloseAllMenus();
        CancelMenu.SetActive(true);
    }

    public void OpenPrevMenu()
    {
        CloseAllMenus();
        switch(prevMenu)
        {
            case "main":
                OpenMainMenu();
                break;
            case "sub":
                OpenSubMenu();
                break;
            default:
                OpenMainMenu();
                break;
        }

        //Debug.Log("Return to " + prevMenu);
    }

    public void ClearMenu(Transform contentObject)
    {
        foreach (Transform item in contentObject)
            Destroy(item.gameObject);
    }

    // Alerts the player battle actor to restart the ability decision operation
    public void CancelCurrentAbility()
    {
        CancelAbility.Invoke();
    }

    public void PopulateMenu(Transform contentObject, List<string> abilityNames)
    {
        // Clear the menu before populating it
        ClearMenu(contentObject);

        foreach (string name in abilityNames)
        {
            // Skip the first item on the list, since it is the default attack
            if (abilityNames.IndexOf(name) == 0)
                continue;

            Button newButton = Instantiate(ButtonPrefab, contentObject).GetComponent<Button>();
            newButton.onClick.AddListener(() => { pbi.GetAbility(name); });
            newButton.onClick.AddListener(() => { OpenCancelMenu(); });
            newButton.transform.GetChild(0).GetComponent<TextMeshProUGUI>().text = name;

            // Either get the button's event trigger, or add one if it doesn't exist
            EventTrigger newButtonTrigger;
            if (!newButton.TryGetComponent(out newButtonTrigger))
                newButtonTrigger = newButton.AddComponent<EventTrigger>();

            // Add a mouse-over event to the event trigger component
            EventTrigger.Entry hoverEvent = new EventTrigger.Entry();
            hoverEvent.eventID = EventTriggerType.PointerEnter;

            // Try and get the ability's description, if possible
            string description = "";
            AbilitiesTable.table.TryGetValue(name, out Ability ability);
            if (ability != null)
                description = ability.GetDescription();

            // Invoke the description box display event on mouse-over
            hoverEvent.callback.AddListener((data) => { DescriptionBox.displayDescription.Invoke(description); });
            newButtonTrigger.triggers.Add(hoverEvent);

            // Add a mouse-exit event to the event trigger component
            EventTrigger.Entry exitEvent = new EventTrigger.Entry();
            exitEvent.eventID = EventTriggerType.PointerExit;

            // Invoke the description box hide event on mouse-exit
            exitEvent.callback.AddListener((data) => { DescriptionBox.hideDescription.Invoke(); });
            newButtonTrigger.triggers.Add(exitEvent);
        }

        // Add a close button to the list for returning to the main menu
        Button closeButton = Instantiate(RedButtonPrefab, contentObject).GetComponent<Button>();
        closeButton.onClick.AddListener(() => { OpenMainMenu(); });
        closeButton.transform.GetChild(0).GetComponent<TextMeshProUGUI>().text = "Close";
    }

    public void SetAttackButton(string attackName)
    {
        AttackButton.onClick.RemoveAllListeners();

        AttackButton.onClick.AddListener(() => { pbi.GetAbility(attackName); });
        AttackButton.onClick.AddListener(() => { OpenCancelMenu(); });
        AttackButton.transform.GetChild(0).GetComponent<TextMeshProUGUI>().text = attackName;

        if(AttackButton.interactable == true)
        {
            // Either get the button's event trigger, or add one if it doesn't exist
            EventTrigger attackButtonTrigger;
            if (!AttackButton.TryGetComponent(out attackButtonTrigger))
                attackButtonTrigger = AttackButton.AddComponent<EventTrigger>();

            // Add a mouse-over event to the event trigger component
            EventTrigger.Entry hoverEvent = new EventTrigger.Entry();
            hoverEvent.eventID = EventTriggerType.PointerEnter;

            // Try and get the ability's description, if possible
            string description = "";
            AbilitiesTable.table.TryGetValue(attackName, out Ability ability);
            if (ability != null)
                description = ability.GetDescription();

            // Invoke the description box display event on mouse-over
            hoverEvent.callback.AddListener((data) => { DescriptionBox.displayDescription.Invoke(description); });
            attackButtonTrigger.triggers.Add(hoverEvent);

            // Add a mouse-exit event to the event trigger component
            EventTrigger.Entry exitEvent = new EventTrigger.Entry();
            exitEvent.eventID = EventTriggerType.PointerExit;

            // Invoke the description box hide event on mouse-exit
            exitEvent.callback.AddListener((data) => { DescriptionBox.hideDescription.Invoke(); });
            attackButtonTrigger.triggers.Add(exitEvent);
        }
    }

    public void SetRunButton()
    {
        RunButton.onClick.RemoveAllListeners();

        RunButton.onClick.AddListener(() => { pbi.GetAbility("Run"); });
        RunButton.onClick.AddListener(() => { OpenCancelMenu(); });
        RunButton.transform.GetChild(0).GetComponent<TextMeshProUGUI>().text = "Run";

        if (RunButton.interactable == true)
        {
            // Either get the button's event trigger, or add one if it doesn't exist
            EventTrigger runButtonTrigger;
            if (!RunButton.TryGetComponent(out runButtonTrigger))
                runButtonTrigger = RunButton.AddComponent<EventTrigger>();

            // Add a mouse-over event to the event trigger component
            EventTrigger.Entry hoverEvent = new EventTrigger.Entry();
            hoverEvent.eventID = EventTriggerType.PointerEnter;

            // Try and get the ability's description, if possible
            string description = "";
            AbilitiesTable.table.TryGetValue("Run", out Ability ability);
            if (ability != null)
                description = ability.GetDescription();

            // Invoke the description box display event on mouse-over
            hoverEvent.callback.AddListener((data) => { DescriptionBox.displayDescription.Invoke(description); });
            runButtonTrigger.triggers.Add(hoverEvent);

            // Add a mouse-exit event to the event trigger component
            EventTrigger.Entry exitEvent = new EventTrigger.Entry();
            exitEvent.eventID = EventTriggerType.PointerExit;

            // Invoke the description box hide event on mouse-exit
            exitEvent.callback.AddListener((data) => { DescriptionBox.hideDescription.Invoke(); });
            runButtonTrigger.triggers.Add(exitEvent);
        }
    }
}
