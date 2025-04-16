using Dialogue;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace Entities.Player
{
    public class SCR_PlayerInteraction : MonoBehaviour
    {
        [Header("INTERACTION PROPERTIES")]
        [SerializeField] SpriteRenderer interactObject;
        bool dialogueManagerEnabled;

        SCR_PlayerInputManager _inputManager;
        UI_LoadScene _loadScene;

        iInteractable _interactableObject;
        SCR_PlayerMovement _playerMovement;
        CircleCollider2D _circleCollider;
        private void Start()
        {
            _inputManager = SCR_GeneralManager.PlayerInputManager;
            _playerMovement = GetComponentInParent<SCR_PlayerMovement>();
            _circleCollider = GetComponent<CircleCollider2D>();

            SCR_DialogueManager.OnDialogueStartEvent += SCR_DialogueManager_OnDialogueStartEvent;
            SCR_DialogueManager.OnDialogueEndEvent += SCR_DialogueManager_OnDialogueEnd;

            interactObject.transform.parent = null;
            interactObject.transform.localScale = Vector3.one;
            interactObject.enabled = false;
            _loadScene = SCR_GeneralManager.UIManager.FindUIObject<UI_LoadScene>();
        }

        private void SCR_DialogueManager_OnDialogueStartEvent(DialogueObject[] obj)
        {
            dialogueManagerEnabled = true;
        }

        private void SCR_DialogueManager_OnDialogueEnd()
        {
            dialogueManagerEnabled = false;

            StopAllCoroutines();
            StartCoroutine(DisableCollider());

            IEnumerator DisableCollider()
            {
                _circleCollider.enabled = false;
                yield return new WaitForSeconds(0.55f);
                _circleCollider.enabled = true;
            }
        }

        public void UpdateCollider()
        {
            Collider2D[] allColliders = new Collider2D[0];
            _circleCollider.OverlapCollider(new ContactFilter2D(), allColliders);

            foreach (var collision in allColliders)
            {
                OnTriggerEnter2D(collision);
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
            if (_loadScene.Loading) { return; }

            interactObject.enabled = _interactableObject != null && !dialogueManagerEnabled;

            if (_inputManager.Submit.PressedThisFrame() && _interactableObject != null)
            {
                _interactableObject.Interact(_playerMovement);
            }
        }

        private void OnDisable()
        {
            SCR_DialogueManager.OnDialogueStartEvent -= SCR_DialogueManager_OnDialogueStartEvent;
            SCR_DialogueManager.OnDialogueEndEvent -= SCR_DialogueManager_OnDialogueEnd;
        }

    }
}
