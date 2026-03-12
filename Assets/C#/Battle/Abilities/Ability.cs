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
        Object obj = Resources.Load("Prefabs/Effects/Hit Spark/hit_effect");

        // Instantiate hitspark
        GameObject hitSpark = GameObject.Instantiate((GameObject)obj);

        // Place hitspark in front of the enemy
        Vector3 CamLoc = Camera.main.transform.position;
        Vector3 direction = (CamLoc - target.transform.position).normalized;
        hitSpark.transform.position = target.transform.position + direction;

        // Aim the hitspark at the camera
        hitSpark.transform.LookAt(CamLoc);

        // Rotate the hitspark by 90 degrees on the z axis
        //hitSpark.transform.localRotation = Quaternion.Euler(hitSpark.transform.localRotation.x, hitSpark.transform.localRotation.y, hitSpark.transform.localRotation.z + 90);
    }

    /// <summary>
    /// Attempts to find the bottom-center world coordinate of an actor's BoxCollider
    /// </summary>
    /// <param name="target">Target's battle actor</param>
    /// <returns>BoxCollider's bottom-center face coordinate in world-space, or the target's position if not found.</returns>
    protected Vector3 FindBottomOfTargetsCollider(Battle_Actor target)
    {
        Vector3 colliderBottomWorldCoordinate = target.transform.position;
        BoxCollider targetCollider;
        target.gameObject.TryGetComponent<BoxCollider>(out targetCollider);
        if (targetCollider != null)
        {
            // target collider's center - half the collider's vertical size
            Vector3 colliderBottomLocalCoordinate = targetCollider.center - Vector3.up * (targetCollider.size.y / 2);

            // Since we do not have monobehavior functions, we use the target's Transform to convert from local space to global space
            colliderBottomWorldCoordinate = target.transform.TransformPoint(colliderBottomLocalCoordinate);
        }

        return colliderBottomWorldCoordinate;
    }

    /// <summary>
    /// Spawns a prefab at the bottom-center of a Battle Actor's BoxCollider.
    /// </summary>
    /// <param name="path">Path of the desired prefab to spawn from the Resources folder.</param>
    /// <param name="target">The Battle Actor to find the bottom of.</param>
    protected void SpawnObjectFromResourcesAtTargetBottom(string path, Battle_Actor target)
    {
        // Get prefab
        Object obj = Resources.Load(path);

        // Instantiate the object
        GameObject newObject = GameObject.Instantiate((GameObject)obj);

        // Place the object at the bottom of the target's BoxCollider in world-space.
        newObject.transform.position = FindBottomOfTargetsCollider(target);
    }
}
