using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Dialogue;
using UnityEngine.UI;

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

        public override void OnPlayerLanded(SCR_PlayerOverworldMovement playerOverworldMovement)
        {
            SCR_GeneralManager.UIManager.FindUIObject<UI_PlayerInputDisplay>().DisplayUI("<color=green>[ENTER]</color> INTERACT WITH NPC");
        }
        
    }
}
