using Dialogue;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Overworld
{
    /// <summary>
    /// Overworld node that can quiz the player, and may reqard them or grant access to other nodes
    /// </summary>
    public class SCR_QuizDialogueNode : SCR_GraphNode, IQuizInterface, iInteractable, ISavableChoice, IDialogueInterface
    {
        [Header("CHOICE NODE PROPERTIES")]
        [SerializeField] protected string characterName;
        [SerializeField] protected ChoiceDialogueObject[] choiceDialogue;
        [SerializeField] protected SavableChoice savableChoice;

        [Header("ADDITIONAL DIALOGUE PROPERTIES")]
        [Tooltip("If this is populated, after the question asked, the NPC will automatically display these dialogue instructions")]
        [SerializeField] protected DialogueObject[] resultantDialogueObjects;

        [Header("QUIZ DIALOGUE PROPERTIES")]
        [Tooltip("Depending on whether this is toggled or not, the question can only be answered once")]
        [SerializeField] bool onlyOneChance;

        int correctChoice;

        [Header("REWARD PROPERTIES")]
        [Tooltip("Item rewarded to player if they got the question right")]
        [SerializeField] SO_Item rewardItem;

        public bool Interactable => true;

        public SavableChoice SavableChoice => savableChoice;

        public DialogueObject[] DialogueObjects => choiceDialogue;
        
        protected void Start()
        {
            if (savableChoice == null || string.IsNullOrEmpty(savableChoice.ChoiceID))
            {
                savableChoice = new SavableChoice(name);
            }

            foreach (ChoiceDialogueObject choice in choiceDialogue)
            {
                if (choice.choiceOptions?.Length > 0)
                {
                    correctChoice = Array.FindIndex(choice.choiceOptions, choiceOption => choiceOption.CorrectAnswer);
                }
            }

            if (!string.IsNullOrEmpty(characterName)) { 
                choiceDialogue.InitialiseCharacterNames(characterName);
            }
        }

        public override bool ConditionalNode()
        {
            return correctChoice == savableChoice.SelectedChoice;
        }
        public int CorrectChoice => correctChoice - 1;
        public bool OnlyOneChance => onlyOneChance;

        public void OnCorrectChoiceMade()
        {
            if (rewardItem){
                SCR_GeneralManager.InventoryManager.AddItem(rewardItem);
            }
            print("ON RIGHT");
        }

        public void OnIncorrectChoiceMade()
        {
            print("ON WRONG");
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
