using Dialogue;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Entities.Player
{
    public class SCR_PlayerInteraction : MonoBehaviour
    {
        SCR_PlayerInputManager _inputManager;

        iInteractable _interactableObject;
        SCR_PlayerMovement _playerMovement;
        CircleCollider2D _circleCollider;
        private void Start()
        {
            _inputManager = SCR_GeneralManager.PlayerInputManager;
            _playerMovement = GetComponentInParent<SCR_PlayerMovement>();
            _circleCollider = GetComponent<CircleCollider2D>();
            SCR_DialogueManager.OnDialogueEnd += SCR_DialogueManager_OnDialogueEnd;
        }

        private void SCR_DialogueManager_OnDialogueEnd()
        {
            StartCoroutine(DisableCollider());

            IEnumerator DisableCollider()
            {
                _circleCollider.enabled = false;
                yield return new WaitForSeconds(0.125f);
                _circleCollider.enabled = true;
            }
        }


        /// <summary>
        /// Trigger Interaction used to determine whether the player has interacted with an interactable object
        /// </summary>
        /// <param name="collision"></param>
        private void OnTriggerEnter2D(Collider2D collision)
        {
            if (collision.GetType(out iInteractable interactable) == null) { return; }

            if (interactable.Interactable){
                _interactableObject = interactable;
            }
            
        }

        private void OnTriggerExit2D(Collider2D collision)
        {
            if (collision.GetType(out iInteractable interactable) == null) { return; }

            if (interactable.Interactable){
                _interactableObject = null;
            }
        }

        private void Update()
        {
            if (_inputManager.Submit.IsPressed() && _interactableObject != null)
            {
                _interactableObject.Interact(_playerMovement);
            }
        }

        private void OnDisable()
        {
            SCR_DialogueManager.OnDialogueEnd -= SCR_DialogueManager_OnDialogueEnd;
        }

    }
}
