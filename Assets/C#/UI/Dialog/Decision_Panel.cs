using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Decision_Panel : MonoBehaviour
{
    public Pause_UI pui;
    public Transform buttons;
    [SerializeField] List<GameObject> buttonList;
    public int cursorIndex;

    // Start is called before the first frame update
    void Start()
    {
        buttonList = new List<GameObject>();
        foreach (Transform child in buttons)
            buttonList.Add(child.gameObject);
        cursorIndex = 0;
        if (buttonList.Count > 0)
            HighlightButton(buttonList[cursorIndex]);
    }

    // Update is called once per frame
    void Update()
    {
        // Ensure button is highlighted during edge cases
        if (buttonList[cursorIndex].GetComponent<Animator>().GetBool("Highlight") != true)
        {
            HighlightButton(buttonList[cursorIndex]);
        }

        // Highlight new button upon player input
        if (pui.moveVertical != 0 && buttonList.Count > 0 && !pui.isPaused)
        {
            int newIndex = cursorIndex - pui.moveVertical;
            if (newIndex < 0)
                newIndex = buttonList.Count - 1;
            else if (newIndex >= buttonList.Count)
                newIndex = 0;

            if (buttonList[cursorIndex] != buttonList[newIndex])
            {
                UnhighlightButton(buttonList[cursorIndex]);
                HighlightButton(buttonList[newIndex]);
                cursorIndex = newIndex;
            }
        }
    }

    void HighlightButton(GameObject button)
    {
        button.GetComponent<Animator>().SetBool("Highlight", true);
        button.GetComponent<Animator>().SetBool("Unhighlight", false);
    }

    void UnhighlightButton(GameObject button)
    {
        button.GetComponent<Animator>().SetBool("Highlight", false);
        button.GetComponent<Animator>().SetBool("Unhighlight", true);
    }

    void ClickButton(GameObject button)
    {
        button.GetComponent<Button>().onClick.Invoke();
    }

    public void Yes()
    {

    }

    public void No()
    {

    }
}
