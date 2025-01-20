using Dialogue;
using Overworld;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Overworld
{
    /// <summary>
    /// Overworld node that contains information about NPCs to answer questions and answers
    /// </summary>
    public class SCR_ChoiceDialogueNode : SCR_GraphNode, iInteractable, ISavableChoice, IDialogueInterface
    {
        [Header("CHOICE NODE PROPERTIES")]
        [Tooltip("Depending on whether this is toggled or not, the question will be saved to disk.")] 
        [SerializeField] protected bool saveQuestion;
        [SerializeField] protected ChoiceDialogueObject[] choiceDialogue;
        [SerializeField] protected SavableChoice savableChoice;

        [Header("ADDITIONAL DIALOGUE PROPERTIES")]
        [SerializeField] protected DialogueObject[] resultantDialogueObjects;

        public bool Interactable => true;

        public SavableChoice SavableChoice => savableChoice;

        public DialogueObject[] DialogueObjects => choiceDialogue;

        public bool SaveQuestionToDisk => saveQuestion;

        protected void Start()
        {
            if (savableChoice == null || string.IsNullOrEmpty(savableChoice.ChoiceID))
            {
                savableChoice = new SavableChoice(name);
            }
        }

        public void Interact(object playerObject)
        {
            //Determine whether this ChoiceID already exists within the existing playerdata
            bool choiceAlreadyMade = SCR_GeneralManager.Instance.PlayerData.SavableChoices.Find(ch => ch.ChoiceID == savableChoice.ChoiceID) != null;
            if (!choiceAlreadyMade)
            {
                SCR_GeneralManager.UIManager.FindUIObject<SCR_DialogueManager>().DisplayDialogue(this);
                return;
            }
            
            //If resulting dialogue exists after a choice has been made, display that dialogue
            //Otherwise, find the first occuring question, and play the resulting dialogue from the choice dialogue object
            if (resultantDialogueObjects != null && resultantDialogueObjects.Length > 0)
            {
                SCR_GeneralManager.UIManager.FindUIObject<SCR_DialogueManager>().DisplayDialogue(resultantDialogueObjects);
            }
            else
            {
                ChoiceDialogueObject firstChoiceDialogue = Array.Find(choiceDialogue, ch => ch.choiceOptions.Length > 0);
                DialogueObject[] dialogueObjects = firstChoiceDialogue.choiceOptions[savableChoice.SelectedChoice].ResultingDialogue;
                SCR_GeneralManager.UIManager.FindUIObject<SCR_DialogueManager>().DisplayDialogue(dialogueObjects);
            }

        }
    }

}