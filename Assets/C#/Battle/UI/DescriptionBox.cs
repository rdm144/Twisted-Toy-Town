using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class DescriptionBox : MonoBehaviour
{
    public delegate void DisplayDescription(string text);
    public static DisplayDescription displayDescription;

    public delegate void HideDescription();
    public static HideDescription hideDescription;

    private TextMeshProUGUI textBox;

    // Start is called before the first frame update
    void Start()
    {
        displayDescription += ActivateDescriptionBox;
        hideDescription += DeactivateDescriptionBox;

        DeactivateDescriptionBox();
    }

    private void ActivateDescriptionBox(string text)
    {
        foreach(Transform t in transform) 
        {
            t.gameObject.SetActive(true);
        }

        if(textBox == null)
            textBox = GetComponentInChildren<TextMeshProUGUI>();

        if (textBox != null)
            textBox.text = text;
    }

    private void DeactivateDescriptionBox()
    {
        if (textBox == null)
            textBox = GetComponentInChildren<TextMeshProUGUI>();

        if (textBox != null)
            textBox.text = "";

        foreach (Transform t in transform)
        {
            t.gameObject.SetActive(false);
        }
    }

    private void OnDestroy()
    {
        displayDescription -= ActivateDescriptionBox;
        hideDescription -= DeactivateDescriptionBox;
    }
}
