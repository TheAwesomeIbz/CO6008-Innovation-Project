using Dialogue;
using System;
using System.Collections;
using System.Collections.Generic;
using Entities.Player;
using UnityEngine;
using UnityEngine.UI;

namespace Entities
{
    public class SCR_ChoiceDialogueNPC : MonoBehaviour, iInteractable, IQuizInterface
    {
        [Header("CHOICE DIALOGUE PROPERTIES")]
        [SerializeField] protected string characterName;
        [SerializeField] protected AudioClip audioSFX;
        [SerializeField] protected ChoiceDialogueObject[] choiceDialogue;

        [Header("SAVABLE CHOICE PROPERTIES")]
        [SerializeField] RecordingFormat recordingFormat;
        [SerializeField] protected SavableChoice savableChoice;


        [Header("ADDITIONAL DIALOGUE PROPERTIES")]
        [Tooltip("If this is populated, after the question asked, the NPC will automatically display these dialogue instructions")]
        [SerializeField] protected DialogueObject[] resultantDialogueObjects;
        
        [SerializeField] protected GameObject invisibleBarrier;
        [SerializeField] protected WarpProperties warpProperties;

        [Header("INTERACTION PROPERTIES")] 
        [SerializeField] private Transform interactionObject;
        private SCR_PlayerMovement playerMovement;
        private SpriteRenderer spriteRenderer;
        private bool interactedWithOnce;
        Action IQuizInterface.OnCorrectChoiceMade => OnCorrectChoiceMade;
        Action IQuizInterface.OnIncorrectChoiceMade => OnIncorrectChoiceMade;

        public bool Interactable => true;

        public DialogueObject[] QuizDialogueObjects => choiceDialogue;
        
        public SavableChoice SavableChoice => savableChoice;
        
        public RecordingFormat RecordingFormat => recordingFormat;

        
        void Start()
        {
            playerMovement = SCR_GeneralManager.LevelManager.playerMovement;
            spriteRenderer = GetComponent<SpriteRenderer>();
            invisibleBarrier?.SetActive(true);
            InitialiseDialogue();
        }

        private void Update()
        {
            interactionObject?.gameObject.SetActive(!interactedWithOnce);
            NPCSpriteUpdate();
        }

        /// <summary>
        /// Method called in Update() that's responsible for updating the NPC direction and sorting value
        /// </summary>
        private void NPCSpriteUpdate()
        {
            if (playerMovement)
            {
                transform.localScale = transform.position.x < playerMovement.transform.position.x ?
                    Vector3.one : new Vector3(-1,1);
            }

            if (spriteRenderer)
            {
                spriteRenderer.sortingOrder = transform.position.y > playerMovement.transform.position.y ?
                        -20 : 20;
            }
        }

        
        /// <summary>
        /// Initialises the dialogue instructions within the object, to be parsed through the dialogue manager.
        /// It sets default values for choice options if they don't exist
        /// </summary>
        private void InitialiseDialogue()
        {
            foreach (ChoiceDialogueObject choiceDialogueObject in choiceDialogue)
            {
                choiceDialogueObject.SetAudioEffect(audioSFX);
                foreach (ChoiceDialogueObject.ChoiceOption choiceOption in choiceDialogueObject.choiceOptions)
                {
                    if (choiceOption.ResultingDialogue.Length == 0)
                    {
                        choiceOption.ResultingDialogue = DialogueObject.CreateDialogue(
                            "Unfortunately that answer is incorrect.",
                            "Please make sure you explore to interact with everyone around you to collect the correct information.");

                        foreach (DialogueObject dialogueObject in choiceOption.ResultingDialogue)
                        {
                            dialogueObject.SetAudioEffect(audioSFX);
                        }
                    }
                }
            }

        }

        public void Interact(object playerObject)
        {
            //if there is a required NPC object, then check if it's been interacted with once
            //if it hasn't, then this will warp the player back to the NPC so they obtain the right information
            if (!warpProperties.requiredNPC?.InteractedOnce ?? false)
            {
                DialogueObject[] requiredDialogue = DialogueObject.CreateDialogue(
                    "It seems you haven't obtained the correct information yet.",
                    "Make sure you explore to interact with everyone around you.");
                
                SCR_GeneralManager.UIManager.FindUIObject<SCR_DialogueManager>().DisplayDialogue(
                    requiredDialogue, OnDialogueEnd: () =>
                    {
                        SCR_GeneralManager.UIManager.FindUIObject<UI_LoadScene>().LoadTransition(warpProperties.playerWarpPosition);
                    });
                
                return;
            }
            
            if (!interactedWithOnce) {
                interactedWithOnce = true;
            }
            
            OnQuestionRightToProceed();
        }

        private DialogueObject[] GetCorrectDialogue()
        {
            ChoiceDialogueObject choiceDialogueObject = Array.Find(choiceDialogue, ch => ch.choiceOptions.Length > 0);
            ChoiceDialogueObject.ChoiceOption correctChoice =
                Array.Find(choiceDialogueObject.choiceOptions, chOption => chOption.CorrectAnswer);
            
            return correctChoice.ResultingDialogue ?? null;
        }

        
        private void OnQuestionRightToProceed()
        {
            if (savableChoice.CorrectAnswer)
            {
                DialogueObject[] resultantDialogue = resultantDialogueObjects.Length > 0 ? resultantDialogueObjects : GetCorrectDialogue();
                SCR_GeneralManager.UIManager.FindUIObject<SCR_DialogueManager>().DisplayDialogue(resultantDialogue);
            }
            else
            {
                SCR_GeneralManager.UIManager.FindUIObject<SCR_DialogueManager>().DisplayDialogue(this, OnDialogueEnd: OnDialogueFinish);
            }
        }



        
        private void OnCorrectChoiceMade()
        {
            warpProperties.correctAnswerSelected = true;
            invisibleBarrier?.SetActive(false);
        }

        private void OnIncorrectChoiceMade()
        {
            warpProperties.requiredNPC?.ResetInteraction();
            interactedWithOnce = false;
            warpProperties.correctAnswerSelected = false;
        }

        private void OnDialogueFinish()
        {
            if (!warpProperties.correctAnswerSelected && warpProperties.shouldWarpPlayer)
            {
                SCR_GeneralManager.UIManager.FindUIObject<UI_LoadScene>().LoadTransition(warpProperties.playerWarpPosition);
            }
        }

        [Serializable] protected struct WarpProperties
        {
            [Header("REQUIRED DIALOGUE NPC PROPERTIES")]
            public SCR_DialogueNPC requiredNPC;
            
            [Header("ADDITIONAL WARP PROPERTIES")]
            public bool correctAnswerSelected;
            public bool shouldWarpPlayer;
            public Vector2 playerWarpPosition;
        }
        
    }

}