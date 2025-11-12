using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.SceneManagement;

public class RunAbility : Ability
{
    public string OverworldName;
    static ScreenTransition st;
    static StageInfo si;

    public RunAbility()
    {
        OverworldName = "Hospital_1F";
        abilityName = "Run";
        description = "Escapes the fight.";
        targetType = TargetType.FriendlyParty;
    }

    public override IEnumerator AbilityAnimation()
    {
        AsyncOperation loadOperation = null;
        if (SceneManager.GetSceneByName(OverworldName) != null)
        {
            // Only load the next scene if it is not the same as the current scene
            if (!SceneManager.GetSceneByName(OverworldName).Equals(SceneManager.GetActiveScene()))
            {
                loadOperation = SceneManager.LoadSceneAsync(OverworldName, LoadSceneMode.Single);
                loadOperation.allowSceneActivation = false;
            }

            // Ensure time is not stopped
            Time.timeScale = 1;

            // Inform the enemy manager of a stun, if applicable
            GameObject stageInfoGameObject = GameObject.FindGameObjectWithTag("Stage Info");
            if (stageInfoGameObject != null)
                stageInfoGameObject.TryGetComponent(out si);

            if (si != null)
            {
                if (si.battleIndex != -1)
                {
                    si.stunIndex = si.battleIndex;
                }
                else
                    Debug.LogWarning("No instance id to send to label for stunning!");
            }

            // Go through the player party and save all hp values
            Player_Battle_Actor[] actors = Object.FindObjectsByType<Player_Battle_Actor>(FindObjectsSortMode.None);
            foreach (Player_Battle_Actor actor in actors)
            {
                if (si != null)
                {
                    foreach (PartyMember stageInfoPartyMember in si.partyMembers)
                    {
                        if (actor.partyIndex == stageInfoPartyMember.partyIndex)
                        {
                            stageInfoPartyMember.health = actor.health;
                            yield return new WaitForEndOfFrame();
                            continue;
                        }
                    }
                }

                if(actor.isDead == false)
                {
                    Debug.Log(actor.name + " is running!");
                    // Rotate entire targetable player party 180
                    actor.transform.rotation = Quaternion.Euler(actor.transform.rotation.x, actor.transform.rotation.y + 180, actor.transform.rotation.z);

                    // Play their runaway animation
                    actor.battleAnim.PlayRunAwayAnimation();

                    // Move players forward off-camera
                    float runSpeed = 300;
                    Rigidbody rb = actor.AddComponent<Rigidbody>();
                    rb.useGravity = false;
                    rb.interpolation = RigidbodyInterpolation.Interpolate;
                    rb.AddRelativeForce(Vector3.right * runSpeed);
                }

            }
            

            yield return new WaitForSeconds(1);

            // Update description

            // Wait for a click or button


            // Activate screen transition, if available
            if (st == null)
                st = Object.FindAnyObjectByType<ScreenTransition>();

            if (st != null)
            {
                st.StartTransition();

                // wait for screen transition
                yield return new WaitUntil(() => st.IsOpaque == true);

                // Wait for scene to load, if it is loading
                if (loadOperation != null) yield return WaitToLoad(loadOperation);
            }

            // Allow next scene to fully load, if it is loaded
            if (loadOperation != null) loadOperation.allowSceneActivation = true;
        }

        yield return null;
        isPlayingAnimation = false;
    }

    IEnumerator WaitToLoad(AsyncOperation loadOperation)
    {
        do
        {
            yield return new WaitForFixedUpdate();
        } while (loadOperation.progress < 0.9f);
    }
}
