using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using Dialogue;
using System;
using Entities.Player;

namespace Entities
{
    public class SCR_DialogueNPC : MonoBehaviour, iInteractable
    {
        public const int C_OffsetPosition = 2;
        public static event Action<SCR_DialogueNPC> OnDialogueStart;
        [SerializeField] DialogueObject[] dialogueObjects;
        public DialogueObject[] DialogueObjects => dialogueObjects;

        public bool Interactable => true;

        public void Interact(object playerObject)
        {
            if (playerObject is SCR_PlayerMovement)
            {
                SCR_PlayerMovement playerMovement = playerObject as SCR_PlayerMovement;
                playerMovement.Rigidbody2D.velocity = Vector3.zero;
                Vector3 talkingPosition = playerMovement.transform.position.x < transform.position.x ?
                    new Vector3(transform.position.x - C_OffsetPosition, playerMovement.transform.position.y) :
                    new Vector3(transform.position.x + C_OffsetPosition, playerMovement.transform.position.y);
                playerMovement.transform.position = talkingPosition;
            }

            if (dialogueObjects == null) {
                Debug.LogWarning("<color=yellow>THERE IS NO DIALOGUE OBJECTS ATTACHED TO THIS GAME OBJECT</color>");
                return;
            }
            SCR_GeneralManager.UIManager.FindUIObject<SCR_DialogueManager>().DisplayDialogue(dialogueObjects);
            OnDialogueStart?.Invoke(this);
        }


    }
}

