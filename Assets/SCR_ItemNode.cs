using System;
using System.Collections;
using System.Collections.Generic;
using Dialogue;
using UnityEngine;

namespace Overworld
{
    public class SCR_ItemNode : SCR_GraphNode, iInteractable
    {
        [SerializeField] private SO_Item overworldItem;
        private bool ItemAlreadyExists => SCR_GeneralManager.Instance.PlayerData.CollectedItems.Contains(name);
        public bool Interactable => !ItemAlreadyExists;
        public void Interact(object playerObject)
        {
            SCR_GeneralManager.Instance.PlayerData.CollectedItems.Add(name);
            SCR_GeneralManager.InventoryManager.AddItem(overworldItem);

            bool startsWithVowel = "aeiou".Contains(overworldItem.name.ToLower()[0]);
            DialogueObject[] dialogueObjects = DialogueObject.CreateDialogue(
                $"{SCR_GeneralManager.Instance.PlayerData.PlayerName} found {(startsWithVowel ? "an" : "a")} {overworldItem.name}!",
                $"The {overworldItem.name} was stored into your inventory."
                );
            SCR_GeneralManager.UIManager.FindUIObject<SCR_DialogueManager>().DisplayDialogue(dialogueObjects);
        }
    }
}
