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
        public static event Action<SCR_DialogueNPC> OnDialogueStart;
        [SerializeField] string NPCName;
        [SerializeField] DialogueObject[] dialogueObjects;
        public DialogueObject[] DialogueObjects => dialogueObjects;

        private Action onDialogueFinish;
        public bool Interactable => true;

        [Header("ADDITIONAL INTERACTION PROPERTIES")] 
        [SerializeField] private GameObject interactedGraphic;
        
        private bool interactedOnce;
        public bool InteractedOnce => interactedOnce;

        private void Start()
        {
            if (!string.IsNullOrEmpty(NPCName)) {
                dialogueObjects.InitialiseCharacterNames(NPCName);
            }
            
            interactedOnce = false;
            interactedGraphic.SetActive(true);
        }


        public void ResetInteraction() => interactedGraphic.SetActive(true);
        public void SetDialogueFinishAction(Action action) => onDialogueFinish = action;

        public void Interact(object playerObject)
        {
            if (playerObject is SCR_PlayerMovement)
            {
                SCR_PlayerMovement playerMovement = playerObject as SCR_PlayerMovement;
                playerMovement.Rigidbody2D.velocity = Vector3.zero;
            }
            
            interactedOnce = true;
            interactedGraphic.SetActive(false);

            if (dialogueObjects == null) {
                Debug.LogWarning("<color=yellow>THERE IS NO DIALOGUE OBJECTS ATTACHED TO THIS GAME OBJECT</color>");
                return;
            }
            SCR_GeneralManager.UIManager.FindUIObject<SCR_DialogueManager>().DisplayDialogue(dialogueObjects, onDialogueFinish);
            OnDialogueStart?.Invoke(this);
        }

        private void Update()
        {
            if (SCR_GeneralManager.LevelManager.playerMovement)
            {
                transform.localScale = transform.position.x < SCR_GeneralManager.LevelManager.playerMovement.transform.position.x ?
                        Vector3.one : new Vector3(-1,1);
            }
        }
    }
}

