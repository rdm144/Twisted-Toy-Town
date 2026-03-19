using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Shield_Enemy_Battle_Actor : Battle_Actor
{
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        InitializeActor();
    }

    protected override void SetAbilitiesList()
    {
        abilities = new List<string>
        {
            "Attack", // First ability will be assigned to the default "Attack" button
            //"ShieldCharge"
        };
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
