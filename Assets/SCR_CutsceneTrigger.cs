using Entities.Player;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SCR_CutsceneTrigger : SCR_AbstractCutsceneTrigger
{
    protected override void TriggerCutscene(SCR_PlayerMovement playerMovement)
    {
        StartCoroutine(waitCoroutine());

        IEnumerator waitCoroutine()
        {
            yield return new WaitForSeconds(2);
            playerMovementReference.enabled = true;
        }

    }
}

public abstract class SCR_AbstractCutsceneTrigger : MonoBehaviour
{
    [SerializeField] protected bool alreadyTriggered = false;
    protected SCR_PlayerMovement playerMovementReference;

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.GetType(out SCR_PlayerMovement playerMovement) && !alreadyTriggered)
        {
            playerMovementReference = playerMovement;
            playerMovementReference.enabled = false;
            TriggerCutscene(playerMovement);
            alreadyTriggered = true;
        }
    }

    protected abstract void TriggerCutscene(SCR_PlayerMovement playerMovement);
}

