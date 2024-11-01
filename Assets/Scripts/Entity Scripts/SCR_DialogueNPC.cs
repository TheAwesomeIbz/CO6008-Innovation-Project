using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using Dialogue;

namespace Entities
{
    public class SCR_DialogueNPC : MonoBehaviour, iInteractable
    {
        [SerializeField] DialogueObject[] dialogueObjects;
        public DialogueObject[] DialogueObjects => dialogueObjects;

        public bool Interactable => true;

        public void Interact(object playerObject)
        {
            if (dialogueObjects == null) {
                Debug.LogWarning("<color=yellow>THERE IS NO DIALOGUE OBJECTS ATTACHED TO THIS GAME OBJECT</color>");
                return;
            }
            SCR_GeneralManager.UIManager.FindUIObject<SCR_DialogueManager>().DisplayDialogue(dialogueObjects);
        }


    }
}

