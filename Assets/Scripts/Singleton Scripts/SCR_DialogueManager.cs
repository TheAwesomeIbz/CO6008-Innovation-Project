using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using System;

namespace Dialogue
{
    /// <summary>
    /// UI class used to control and display the dialogue on screen
    /// </summary>
    public class SCR_DialogueManager : MonoBehaviour
    {
        /// <summary>
        /// Event called when the dialogue manager begins an interaction
        /// </summary>
        public static event Action<DialogueObject[]> OnDialogueStartEvent;

        /// <summary>
        /// Event called when the dialogue manager finishes an interaction
        /// </summary>
        public static event Action OnDialogueEndEvent;
        private Action OnDialogueEnd;

        [Header("DIALOGUE MANAGER PROPERTIES")]
        [SerializeField] GameObject _dialogueBox;
        [SerializeField] GameObject _nameBox, _continueIcon;
        [SerializeField] TextMeshProUGUI _dialogueText, _nameText;

        [Header("DIALOGUE OBJECTS PROPERTIES")]
        [SerializeField] int dialogueObjectIndex;
        [SerializeField] int choiceDialogueObjectIndex;

        [Header("CHOICE UI PROPERTIES")]
        [SerializeField] Transform parentChoiceObject;
        [SerializeField] ChoiceUIObject[] choiceUIObjects;

        [Header("CACHED DIALOGUE OBJECTS")]
        [SerializeReference] DialogueObject[] cachedDialogueObjects;
        [SerializeField] ChoiceDialogueObject.ChoiceOption[] cachedChoiceObjects;
        ISavableChoice savableChoice;
        IQuizInterface quizInterface;
        bool selectedCorrectChoice;
        bool enablePlayerControlsOnFinish;
        /// <summary>
        /// The current index of either the choice dialogue objects or default dialogue objects. 
        /// This depends on whether the dialogue manager is running through any resultant dialogue within the choice dialogue object.
        /// </summary>
        int GetCurrentIndex => choiceDialogueObjectIndex > -1 ? choiceDialogueObjectIndex : dialogueObjectIndex;
        
        float questionedTime, answeredTime;
        void Start()
        {
            _dialogueBox.SetActive(false);
            _nameBox.SetActive(false);
            _continueIcon.SetActive(false);

            choiceUIObjects = new ChoiceUIObject[]
            {
                new ChoiceUIObject(parentChoiceObject.GetChild(0).gameObject),
                new ChoiceUIObject(parentChoiceObject.GetChild(1).gameObject),
                new ChoiceUIObject(parentChoiceObject.GetChild(2).gameObject)
            };

        }

        /// <summary>
        /// DIsplays the dialogue objects within the dialogue manager UI. Flexibly display the instructions of any object that implements this interface.
        /// </summary>
        /// <param name="dialogueObjects"></param>
        public void DisplayDialogue(IDialogueInterface dialogueInterface, Action OnDialogueEnd = null, bool enablePlayerControlsOnFinish = true)
        {
            if (dialogueInterface is ISavableChoice){
                savableChoice = dialogueInterface as ISavableChoice;
            }
            if (dialogueInterface is IQuizInterface){
                quizInterface = dialogueInterface as IQuizInterface;
            }
            DisplayDialogue(dialogueInterface.DialogueObjects, OnDialogueEnd, enablePlayerControlsOnFinish);
        }

        /// <summary>
        /// DIsplays the dialogue objects within the dialogue manager UI
        /// </summary>
        /// <param name="dialogueObjects"></param>
        public void DisplayDialogue(DialogueObject[] dialogueObjects, Action OnDialogueEnd = null, bool enablePlayerControlsOnFinish = true)
        {
            bool dialogueAlreadyActive = _dialogueBox.activeInHierarchy;
            if (dialogueAlreadyActive && choiceDialogueObjectIndex > -1)
            {
                DialogueObject[] combinedArray = new DialogueObject[cachedDialogueObjects.Length + dialogueObjects.Length];
                cachedDialogueObjects.CopyTo(combinedArray, 0);
                dialogueObjects.CopyTo(combinedArray, cachedDialogueObjects.Length);
                cachedDialogueObjects = combinedArray;
                return;
            }

            this.enablePlayerControlsOnFinish = enablePlayerControlsOnFinish;
            cachedDialogueObjects = dialogueObjects;
            dialogueObjectIndex = 0;
            choiceDialogueObjectIndex = -1;

            DialogueObject[] validDialogue = dialogueObjects.Length == 0 ?
                DialogueObject.NullDialogueObject : dialogueObjects;
            this.OnDialogueEnd = OnDialogueEnd;

            SetDialogueActivity(true, validDialogue[0]);
            SCR_PlayerInputManager.PlayerControlsEnabled = false;
            OnDialogueStartEvent?.Invoke(validDialogue);

            DisplayNextDialogue(validDialogue);
        }


        /// <summary>
        /// DIsplay the next dialogue object within the sequence, until the index is out of bounds for the dialogue object
        /// </summary>
        /// <param name="dialogueObjects"></param>
        private void DisplayNextDialogue(DialogueObject[] dialogueObjects)
        {
            StopAllCoroutines();
            questionedTime = 0;
            answeredTime = 0;
            _continueIcon.SetActive(false);
            DialogueObject[] currentDialogueObjects = dialogueObjects;

            if (currentDialogueObjects == null)
            {
                EndDialogueSequence();
                return;
            }
            
            //If the dialogue manager has run into the last object in the resultant dialogue in the choice dialogue object
            //then resume back to displaying the parent dialogue objects (cached locally)
            if (choiceDialogueObjectIndex != -1 && choiceDialogueObjectIndex >= currentDialogueObjects.Length)
            {
                choiceDialogueObjectIndex = -1;
                dialogueObjectIndex++;
                currentDialogueObjects = cachedDialogueObjects;
            }

            if (GetCurrentIndex >= currentDialogueObjects.Length)
            {
                EndDialogueSequence();
                return;
            }
            else
            {
                SetDialogueActivity(true, currentDialogueObjects[GetCurrentIndex]);
                StartCoroutine(TypeSentence(currentDialogueObjects));
            }
        }

        /// <summary>
        /// Sets all dialogue objects active or inactive, alongside the name box
        /// </summary>
        /// <remarks>Name box is dependant on whether the dialogue object has a speaking character name</remarks>
        /// <param name="dialogueActivity"></param>
        /// <param name="dialogueObject"></param>
        private void SetDialogueActivity(bool dialogueActivity, DialogueObject dialogueObject)
        {
            _dialogueBox.SetActive(dialogueActivity);
            _continueIcon.SetActive(false);

            if (dialogueObject == null) {
                _nameBox.SetActive(false);
                return;
            }
            if (!string.IsNullOrWhiteSpace(dialogueObject.SpeakingCharacter))
            {
                _nameBox.SetActive(dialogueActivity);
                _nameText.text = dialogueObject.SpeakingCharacter;
            }
            else
            {
                _nameBox.SetActive(false);
            }

        }

        /// <summary>
        /// Called when dialogue sequence is over when the last dialogue object has been reached.
        /// </summary>
        private void EndDialogueSequence()
        {
            SetDialogueActivity(false, null);
            OnDialogueEndEvent?.Invoke();
            OnDialogueEnd?.Invoke();

            
            OnDialogueEnd = null;
            quizInterface = null;
            choiceDialogueObjectIndex = -1;
            dialogueObjectIndex = 0;
            cachedDialogueObjects = null;
            cachedChoiceObjects = null;

            if (enablePlayerControlsOnFinish)
            {
                SCR_PlayerInputManager.PlayerControlsEnabled = true;
            }
            

        }



        /// <summary>
        /// Enumaration called to type sentence in dialogue text box
        /// </summary>
        /// <param name="dialogueObjects"></param>
        /// <returns></returns>
        private IEnumerator TypeSentence(DialogueObject[] dialogueObjects)
        {
            
            DialogueObject currentDialogueObject = dialogueObjects[GetCurrentIndex];
            string tempString = "";
            foreach (char character in currentDialogueObject.DialogueText)
            {
                tempString += character;
                _dialogueText.text = tempString;
                yield return new WaitForSeconds(0.005f);
            }
            

            //If any choices exist, then display choices and relevant UI, else default to normal text settings.
            ChoiceDialogueObject choiceDialogueObject = currentDialogueObject as ChoiceDialogueObject;
            
            if (choiceDialogueObject?.choiceOptions.Length > 0)
            {
                DisplayChoices(choiceDialogueObject);
            }
            else
            {
                yield return new WaitForSeconds(0.5f);
                _continueIcon.SetActive(true);
                yield return new WaitUntil(() => { return Input.GetButtonDown("Submit"); });

                if (choiceDialogueObjectIndex > -1){
                    choiceDialogueObjectIndex++;
                }
                else{
                    dialogueObjectIndex++;
                }

                currentDialogueObject.OnSentenceFinished?.Invoke();
                DisplayNextDialogue(dialogueObjects);
            } 

        }

        /// <summary>
        /// Display the choices and update each choiceUI object to display unique text.
        /// </summary>
        /// <param name="choiceDialogue"></param>
        private void DisplayChoices(ChoiceDialogueObject choiceDialogue)
        {
            //Cache the starting time for asking the question
            questionedTime = Time.time;
            foreach (ChoiceUIObject choiceUIObject in choiceUIObjects){
                choiceUIObject.SetObjectActivity(false);
            }

            //Set the amount of questions in the UI
            for (int i = 0; i < choiceDialogue.choiceOptions.Length; i++){
                choiceUIObjects[i].SetObjectActivity(true);
                choiceUIObjects[i].SetChoiceText(choiceDialogue.choiceOptions[i].ChoiceText);
            }

            cachedChoiceObjects = choiceDialogue.choiceOptions;
        }

        /// <summary>
        /// Called whenever a choice UI object is selected, with its respective index
        /// </summary>
        /// <param name="index">The index of the button</param>
        public void OnChoiceSelectedIndex(int index)
        {
            answeredTime = Time.time;
            foreach (ChoiceUIObject choiceUIObject in choiceUIObjects){
                choiceUIObject.SetObjectActivity(false);
            }

            ChoiceDialogueObject currentDialogueObject = cachedDialogueObjects[GetCurrentIndex] as ChoiceDialogueObject;
            if (!currentDialogueObject.NonImpactingChoice)
            {
                bool choiceAlreadyExists = SCR_GeneralManager.Instance.PlayerData.SavableChoices.Find(ch => ch.ChoiceID == savableChoice.SavableChoice.ChoiceID) != null;

                //if there is a savable choice that doesnt exist that can be saved to disk, then add it to the player data
                bool canSaveToDisk = savableChoice != null && !choiceAlreadyExists;
                selectedCorrectChoice = currentDialogueObject.CorrectChoice == index;



                if (canSaveToDisk && (selectedCorrectChoice || (quizInterface?.OnlyOneChance ?? false)))
                {
                    savableChoice.SavableChoice.SetChoice(index, (float)Math.Round(answeredTime - questionedTime, 2));
                    SCR_GeneralManager.Instance.PlayerData.SavableChoices.Add(savableChoice.SavableChoice);
                }

                
            }


            choiceDialogueObjectIndex++;
            cachedChoiceObjects[index].OnChoiceMade?.Invoke();
            if (selectedCorrectChoice)
            {
                quizInterface?.OnCorrectChoiceMade();
            }
            else
            {
                quizInterface?.OnIncorrectChoiceMade();
            }
            DisplayNextDialogue(cachedChoiceObjects[index].ResultingDialogue);
        }

        
        /// <summary>
        /// Local class used for flexibly displaying and setting properties of choice UI
        /// </summary>
        [Serializable] class ChoiceUIObject
        {
            [SerializeField] private GameObject choiceGameObject;
            [SerializeField] private TextMeshProUGUI choiceText;

            /// <summary>
            /// Enable or disable each choice button
            /// </summary>
            /// <param name="activity"></param>
            public void SetObjectActivity(bool activity) => choiceGameObject.SetActive(activity);

            /// <summary>
            /// Set the text of each choice button
            /// </summary>
            /// <param name="text"></param>
            public void SetChoiceText(string text) => choiceText.text = text;

            public ChoiceUIObject(GameObject choiceGameObject)
            {
                this.choiceGameObject = choiceGameObject;
                choiceText = choiceGameObject.GetComponentInChildren<TextMeshProUGUI>();

                this.choiceGameObject.SetActive(false);
            }
        }
    }
}


