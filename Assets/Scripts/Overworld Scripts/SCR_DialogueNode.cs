using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Dialogue;

namespace Overworld
{
    public class SCR_DialogueNode : SCR_GraphNode, iInteractable
    {
        [Header("DIALOGUE PROPERTIES")]
        [SerializeField] DialogueObject[] dialogueObjects;

        public bool Interactable => true;

        public DialogueObject[] DialogueObject => dialogueObjects;

        public void Interact(object playerObject)
        {
            SCR_GeneralManager.UIManager.FindUIObject<SCR_DialogueManager>().DisplayDialogue(dialogueObjects);
        }
    }
}
