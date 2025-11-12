using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Events;

public class Dialog_UI : MonoBehaviour
{
    EnemyManager em;

    public GameObject name_tag_panel;
    public TextMeshProUGUI name_tag_text;
    public GameObject dialog_panel;
    public TextMeshProUGUI dialog_text;
    public GameObject downArrow;
    public GameObject sideArrow;
    public Image leftImage;
    public Image rightImage;
    public GameObject decision_panel;
    List<Actor> actorsToBeUnpaused;

    public char sideArrowDelimiter;
    public int charDelay;
    public bool isTalking;

    bool isRolling;
    bool skipTextRoll;

    private void Awake()
    {
        gameObject.tag = "Dialog_UI";
    }

    // Start is called before the first frame update
    private void Start()
    {
        Deactivate_UI();
        ClearDialog();
        ClearNameTag();
        actorsToBeUnpaused = new List<Actor>();
        isTalking = false;
        skipTextRoll = false;
        isRolling = false;
    }

    private void Update()
    {
        // If the player presses a button while talking, skip the text roll
        if (isRolling == true && Input.anyKeyDown == true && Input.GetKeyDown(KeyCode.Escape) == false)
        {
            skipTextRoll = true;
        } 
    }

#region Private Cutscene Tools
    private void Activate_UI()
    {
        dialog_panel.SetActive(true);
        name_tag_panel.SetActive(true);
        skipTextRoll = false;
    }

    private void Deactivate_UI()
    {
        dialog_panel.SetActive(false);
        name_tag_panel.SetActive(false);
        downArrow.SetActive(false);
        //sideArrow.SetActive(false);
        //decision_panel.SetActive(false);
        skipTextRoll = false;
        isTalking = false;
    }

    private void ClearDialog()
    {
        dialog_text.text = "";
    }

    private void ClearNameTag()
    {
        name_tag_text.text = "";
    }
    
    private void SetDialogText(string newText)
    {
        dialog_text.text = newText;
    }

    private void DisableAllActiveActors()
    {
        Actor[] allActors = GameObject.FindObjectsByType<Actor>(FindObjectsSortMode.None);
        foreach (Actor actor in allActors)
        {
            if(actor.canOperate == true)
            {
                actorsToBeUnpaused.Add(actor);
                actor.canOperate = false;
                actor.isMoving = false;
            }
        }
    }

    private void ReenableAllPreviouslyActiveActors()
    {
        foreach(Actor actor in actorsToBeUnpaused)
        {
            actor.canOperate = true;
        }

        actorsToBeUnpaused.Clear();
    }

    private IEnumerator PauseOnDelimiter()
    {
        // Get world position of character just before the delimiter
        TMP_TextInfo textInfo = dialog_text.textInfo;
        Vector2 sideArrowPosition = textInfo.characterInfo[textInfo.characterCount - 1].bottomLeft;

        // Place side arrow at that position
        sideArrow.GetComponent<RectTransform>().anchoredPosition = sideArrowPosition;

        // Activate side arrow
        sideArrow.SetActive(true);

        // Wait a frame to update values
        yield return new WaitForFixedUpdate();

        // Reusing skipTextRoll variable to look for player input
        yield return new WaitUntil(() => skipTextRoll == true);

        // Wait a frame to update values
        yield return new WaitForFixedUpdate();

        // Deactivate side arrow
        sideArrow.SetActive(false);

        skipTextRoll = false;
    }

    private int GetIndexOfNextDelimiter(int startIndex, string text)
    {
        for (int i = startIndex; i < text.Length; i++)
            if (text[i].Equals(sideArrowDelimiter))
                return i;
        return -1;
    }
#endregion

#region Public Cutscene Tools
    /// <summary>
    /// Sets the Dialog UI's name tag text to a given string
    /// </summary>
    /// <param name="newText">New string to set the name tag to.</param>
    public void SetNameTagText(string newText)
    {
        name_tag_text.text = newText;
    }

    /// <summary>
    /// Opens the Dialog UI, while disabling all enemy and player actions.
    /// </summary>
    public void StartDialogUI()
    {
        DisableAllActiveActors();
        ClearDialog();
        Activate_UI();
        isTalking = true;
    }

    /// <summary>
    /// Updates dialog text in a "rolling" manner.
    /// </summary>
    /// <param name="fullText">String to roll.</param>
    /// <returns></returns>
    public IEnumerator RollText(string fullText)
    {
        char currentChar;
        ClearDialog();
        yield return new WaitForFixedUpdate();

        skipTextRoll = false;
        isRolling = true;
        downArrow.SetActive(false);

        while (dialog_text.textInfo.characterCount < fullText.Length)
        {
            yield return null;

            // Get next character
            currentChar = fullText[dialog_text.textInfo.characterCount];
            
            // if delimiter, activate side arrow
            if(currentChar.Equals(sideArrowDelimiter))
            {
                yield return PauseOnDelimiter();

                // remove delimiter from full text
                fullText = fullText.Remove(dialog_text.textInfo.characterCount, 1);

                // Wait a frame to update values
                yield return new WaitForFixedUpdate();

                continue;
            }
            
            // Add next char to current text
            dialog_text.text += currentChar;

            // Update text on screen
            SetDialogText(dialog_text.text);

            // Delay
            for(int i = 0; i < charDelay; i++)
            {
                // break if skipping text
                if (skipTextRoll == true)
                {
                    // Skip to the next delimiter, if it exists
                    int nextDelimiter = GetIndexOfNextDelimiter(dialog_text.textInfo.characterCount, fullText);
                    if(nextDelimiter != -1)
                    {
                        // show text up to that point
                        dialog_text.text = fullText.Remove(nextDelimiter);
                        SetDialogText(dialog_text.text);
                    }
                    // Else skip to end of text
                    else
                    {
                        dialog_text.text = fullText;
                        SetDialogText(fullText);
                    }
                    
                    skipTextRoll = false;
                    yield return null;
                    break;
                }
                else
                    yield return new WaitForFixedUpdate(); // Wait for 1 frame
            }
        }

        // Activate arrow
        downArrow.SetActive(true);

        // Reusing skipTextRoll variable to look for player input
        yield return new WaitUntil(() => skipTextRoll == true);
    }

    /// <summary>
    /// Updates dialog text in a "rolling" manner.
    /// </summary>
    /// <param name="fullText">String to roll.</param>
    /// <returns></returns>
    public IEnumerator RollTextWithDecision(string fullText)
    {
        char currentChar;
        ClearDialog();
        yield return new WaitForFixedUpdate();

        skipTextRoll = false;
        isRolling = true;
        downArrow.SetActive(false);

        while (dialog_text.textInfo.characterCount < fullText.Length)
        {
            yield return null;

            // Get next character
            currentChar = fullText[dialog_text.textInfo.characterCount];

            // if delimiter, activate side arrow
            if (currentChar.Equals(sideArrowDelimiter))
            {
                yield return PauseOnDelimiter();

                // remove delimiter from full text
                fullText = fullText.Remove(dialog_text.textInfo.characterCount, 1);

                // Wait a frame to update values
                yield return new WaitForFixedUpdate();

                continue;
            }

            // Add next char to current text
            dialog_text.text += currentChar;

            // Update text on screen
            SetDialogText(dialog_text.text);

            // Delay
            for (int i = 0; i < charDelay; i++)
            {
                // break if skipping text
                if (skipTextRoll == true)
                {
                    // Skip to the next delimiter, if it exists
                    int nextDelimiter = GetIndexOfNextDelimiter(dialog_text.textInfo.characterCount, fullText);
                    if (nextDelimiter != -1)
                    {
                        // show text up to that point
                        dialog_text.text = fullText.Remove(nextDelimiter);
                        SetDialogText(dialog_text.text);
                    }
                    // Else skip to end of text
                    else
                    {
                        dialog_text.text = fullText;
                        SetDialogText(fullText);
                    }

                    skipTextRoll = false;
                    yield return null;
                    break;
                }
                else
                    yield return new WaitForFixedUpdate(); // Wait for 1 frame
            }
        }

        // Open decision panel
        decision_panel.SetActive(true);

        yield return new WaitForFixedUpdate();

        // wait for player input

        // Activate arrow
        //downArrow.SetActive(true);

        // Reusing skipTextRoll variable to look for player input
        //yield return new WaitUntil(() => skipTextRoll == true);
    }

    /// <summary>
    /// Closes the Dialog UI, while also enabling all enemy and player actions.
    /// </summary>
    public void EndDialogUI()
    {
        Deactivate_UI();
        isTalking = false;
        isRolling = false;
        skipTextRoll = false;

        ReenableAllPreviouslyActiveActors();
    }

    /// <summary>
    /// Fades an image's color either from transparent to opaque, or vice versa.
    /// </summary>
    /// <param name="time">Total fade duration in seconds.</param>
    /// <param name="sr">The image to fade in/out.</param>
    /// <param name="fadeDirection">-1 for transparent fade out, 1 for opaque fade in.</param>
    /// <returns></returns>
    public IEnumerator FadeImageAlpha(float time, Image sr, int fadeDirection)
    {
        // Recieves either 1 or -1 for fade direction (fade in or fade out)
        //Application.targetFrameRate = 60;
        float targetTime = (float)Screen.currentResolution.refreshRateRatio.value * time;
        float increment = (1f / targetTime);
        float currentTransparency;
        if (fadeDirection == 1) // become opaque
        {
            currentTransparency = 0f;
            while (sr.color.a < 1)
            {
                sr.color = new Color(sr.color.r, sr.color.g, sr.color.b, currentTransparency);
                currentTransparency += increment;
                yield return new WaitForEndOfFrame();
            }
        }
        else // become transparent
        {
            currentTransparency = 1f;
            while (sr.color.a > 0)
            {
                sr.color = new Color(sr.color.r, sr.color.g, sr.color.b, currentTransparency);
                currentTransparency -= increment;
                yield return new WaitForEndOfFrame();
            }
        }
    }

    /// <summary>
    /// Sets an image to Color.gray (0.5f, 0.5f, 0.5f, alpha).
    /// </summary>
    /// <param name="image">Image to dim.</param>
    public void DimImage(Image image)
    {
        image.color = Color.gray;
    }

    /// <summary>
    /// Sets an image to Color.white (1, 1, 1, alpha).
    /// </summary>
    /// <param name="image">Image to lighten.</param>
    public void LightenImage(Image image)
    {
        image.color = Color.white;
    }

    /// <summary>
    /// Colors the name tag's text
    /// </summary>
    /// <param name="newColor">Color to set the name tag to.</param>
    public void SetNameTagColor(Color newColor)
    {
        name_tag_text.color = newColor;
    }
#endregion
}
