using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ScreenTransition : MonoBehaviour
{
    Animator anim;
    
    public bool IsOpaque;

    // Start is called before the first frame update
    void Start()
    {
        anim = GetComponent<Animator>();
        Teleporter.DimScreen += StartTransition;
        Teleporter.UndimScreen += EndTransition;
        Enemy.StartBattle += FightTransition;
        
        IsOpaque = false;
    }

    // Used by Fade in animation
    public void SetOpaqueState()
    {
        IsOpaque = true;
    }

    // Used by fade out animation
    public void SetTransparentState()
    {
        IsOpaque = false;
    }

    public void StartTransition()
    {
        anim.SetTrigger("Start");
    }

    public void EndTransition()
    {
        anim.SetTrigger("End");
    }

    public void FightTransition()
    {
        anim.SetTrigger("Fight");
    }

    private void OnDestroy()
    {
        Teleporter.DimScreen -= StartTransition;
        Teleporter.UndimScreen -= EndTransition;
        Enemy.StartBattle -= FightTransition;
    }
}
