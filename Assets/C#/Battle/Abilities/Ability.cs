using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum TargetType { FriendlySingle, FriendlyParty, OpponentSingle, OpponentParty };

public class Ability
{
    protected string abilityName;
    protected int deltaHP;
    protected TargetType targetType;
    protected string description;

    protected bool isPlayingAnimation = false;
    protected List<Battle_Actor> currentTargets;
    protected Battle_Actor caster;

    public Ability()
    {
        abilityName = "Ability";
        deltaHP = 0;
        description = "Deals " + deltaHP + " damage to a single target.";
        targetType = TargetType.OpponentSingle;
    }

    public string GetName()
    {
        return abilityName;
    }

    public int GetDeltaHP()
    {
        return deltaHP;
    }

    public string GetDescription()
    {
        return description;
    }

    public bool IsPlayingAnimation()
    {
        return isPlayingAnimation;
    }

    public TargetType GetTargetType()
    {
        return targetType;
    }

    public virtual void SetTarget(List<Battle_Actor> target)
    {
        currentTargets = target;
    }

    public virtual void SetCaster(Battle_Actor newCaster)
    {
        caster = newCaster;
    }

    public virtual void PlayAnimation()
    {
        if(!isPlayingAnimation)
        {
            isPlayingAnimation = true;
            //StartCoroutine(nameof(this.AbilityAnimation));
        }
    }

    public virtual IEnumerator AbilityAnimation()
    {
        // move characters, move camera, spawn flares, play sounds, etc

        foreach(Battle_Actor target in currentTargets)
        {
            target.TakeDamage(deltaHP);
        }

        yield return null;
        isPlayingAnimation = false;
    }

    protected void SpawnObjectFromResources(string path, Vector3 targetPosition)
    {
        // Get prefab
        Object obj = Resources.Load(path);

        // Instantiate the object
        GameObject newObject = GameObject.Instantiate((GameObject)obj);

        // Place the object at the desired world-space location
        newObject.transform.position = targetPosition;
    }

    protected void SpawnHitSpark(Battle_Actor target)
    {
        // Get the hitspark prefab
        Object obj = Resources.Load("Prefabs/Battle/Effects/hit_effect");

        // Instantiate hitspark
        GameObject hitSpark = GameObject.Instantiate((GameObject)obj);

        // Place hitspark in front of the enemy
        Vector3 CamLoc = Camera.main.transform.position;
        Vector3 direction = (CamLoc - target.transform.position).normalized;
        hitSpark.transform.position = target.transform.position + direction;

        // Aim the hitspark at the camera
        hitSpark.transform.LookAt(CamLoc);

        // Rotate the hitspark by 90 degrees on the z axis
        hitSpark.transform.localRotation = Quaternion.Euler(hitSpark.transform.localRotation.x, hitSpark.transform.localRotation.y, hitSpark.transform.localRotation.z + 90);
    }
}
