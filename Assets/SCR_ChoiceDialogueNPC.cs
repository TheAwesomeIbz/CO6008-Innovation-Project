using Dialogue;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Entities
{
    public class SCR_ChoiceDialogueNPC : MonoBehaviour, iInteractable, IQuizInterface
    {
        [Header("CHOICE DIALOGUE PROPERTIES")]
        [SerializeField] private InteractionType NPCInteractionType;
        [SerializeField] protected string characterName;
        [SerializeField] protected ChoiceDialogueObject[] choiceDialogue;

        [Header("SAVABLE CHOICE PROPERTIES")]
        [SerializeField] bool interactedWithOnce;
        [SerializeField] bool saveQuestionToDisk;
        [SerializeField] protected SavableChoice savableChoice;


        [Header("ADDITIONAL DIALOGUE PROPERTIES")]
        [Tooltip("If this is populated, after the question asked, the NPC will automatically display these dialogue instructions")]
        [SerializeField] protected DialogueObject[] resultantDialogueObjects;

        public bool Interactable => true;

        public DialogueObject[] QuizDialogueObjects => choiceDialogue;

        public int CorrectChoice => 1;

        public bool OnlyOneChance => true;

        public SavableChoice SavableChoice => savableChoice;

        public bool SaveQuestionToDisk => saveQuestionToDisk;

        private Action onCorrectChoiceMade, onIncorrectChoiceMade;

        public void SetChoiceActions(Action onCorrectChoiceMade = null, Action onIncorrectChoiceMade = null)
        {
            this.onCorrectChoiceMade = onCorrectChoiceMade;
            this.onIncorrectChoiceMade = onIncorrectChoiceMade;
        }

        Action IQuizInterface.OnCorrectChoiceMade => onCorrectChoiceMade;

        Action IQuizInterface.OnIncorrectChoiceMade => onIncorrectChoiceMade;

        public void Interact(object playerObject)
        {
            switch (NPCInteractionType)
            {
                case InteractionType.QUESTION_RIGHT_TO_PROCEED:
                    OnQuestionRightToProceed();
                    break;
            }

            //if (saveQuestionToDisk)
            //{
            //    //Determine whether this ChoiceID already exists within the existing playerData
            //    bool choiceAlreadyMade = SCR_GeneralManager.Instance.Choices.Find(ch => ch.ChoiceID == savableChoice.ChoiceID) != null;
            //    if (!choiceAlreadyMade)
            //    {
            //        SCR_GeneralManager.UIManager.FindUIObject<SCR_DialogueManager>().DisplayDialogue(this);
            //        return;
            //    }

            //    //If resulting dialogue exists after a choice has been made, display that dialogue
            //    //Otherwise, find the first occuring question, and play the resulting dialogue from the choice dialogue object
            //    if (resultantDialogueObjects?.Length > 0)
            //    {
            //        SCR_GeneralManager.UIManager.FindUIObject<SCR_DialogueManager>().DisplayDialogue(resultantDialogueObjects);
            //    }
            //    else
            //    {
            //        ChoiceDialogueObject firstChoiceDialogue = Array.Find(choiceDialogue, ch => ch.choiceOptions.Length > 0);
            //        DialogueObject[] dialogueObjects = firstChoiceDialogue.choiceOptions[savableChoice.SelectedChoice].ResultingDialogue;
            //        SCR_GeneralManager.UIManager.FindUIObject<SCR_DialogueManager>().DisplayDialogue(dialogueObjects);
            //    }

            //}
            //else
            //{
            //    if (!interactedWithOnce)
            //    {
            //        SCR_GeneralManager.UIManager.FindUIObject<SCR_DialogueManager>().DisplayDialogue(this);
            //        interactedWithOnce = true;
            //    }
            //    else
            //    {
            //        SCR_GeneralManager.UIManager.FindUIObject<SCR_DialogueManager>().DisplayDialogue(resultantDialogueObjects);
            //    }
                
            //}


            
        }

        private void OnQuestionRightToProceed()
        {
            if (savableChoice.CorrectAnswer)
            {
                SCR_GeneralManager.UIManager.FindUIObject<SCR_DialogueManager>().DisplayDialogue(resultantDialogueObjects);
            }
            else
            {
                SCR_GeneralManager.UIManager.FindUIObject<SCR_DialogueManager>().DisplayDialogue(this);
            }
           

        }

        void Start()
        {

        }

        // Update is called once per frame
        void Update()
        {

        }

        public void OnCorrectChoiceMade()
        {
            
        }

        public void OnIncorrectChoiceMade()
        {
            
        }

        enum InteractionType
        {
            QUESTION_RIGHT_TO_PROCEED,
            
        }
    }

}