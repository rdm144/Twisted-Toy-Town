using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CombatUI_Container : MonoBehaviour
{
    public GameObject FailsafeUI;
    public GameObject JimmyUI;
    public GameObject MeddyUI;

    private Dictionary<string, GameObject> dict = new Dictionary<string, GameObject>();

    void Start()
    {
        dict.Add("Jimmy", JimmyUI);
        dict.Add("Meddy", MeddyUI);
    }

    public GameObject GetCombatUI(string key)
    {
        if (dict.ContainsKey(key))
            return dict[key];
        else
            return FailsafeUI;
    }

    public void DisableAllCombatUI()
    {
        foreach(Transform child in transform)
        {
            child.gameObject.SetActive(false);
        }
    }
}
