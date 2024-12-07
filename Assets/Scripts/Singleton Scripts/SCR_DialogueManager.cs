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
        //Keeps track of the index of DialogueObject[] locally
        [SerializeField] int dialogueObjectIndex;

        void Start()
        {
            _dialogueBox.SetActive(false);
            _nameBox.SetActive(false);
            _continueIcon.SetActive(false);
        }


        /// <summary>
        /// DIsplays the dialogue objects within the dialogue manager UI
        /// </summary>
        /// <param name="dialogueObjects"></param>
        public void DisplayDialogue(DialogueObject[] dialogueObjects, Action OnDialogueEnd = null)
        {
            
            dialogueObjectIndex = 0;

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
            _continueIcon.SetActive(false);
            if (dialogueObjectIndex >= dialogueObjects.Length)
            {
                EndDialogueSequence();
                return;
            }
            else
            {
                SetDialogueActivity(true, dialogueObjects[dialogueObjectIndex]);
                StartCoroutine(TypeSentence(dialogueObjects));
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
            if (dialogueObject.SpeakingCharacter != "")
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
            SCR_PlayerInputManager.PlayerControlsEnabled = true;
        }

        /// <summary>
        /// Enumaration called to type sentence in dialogue text box
        /// </summary>
        /// <param name="dialogueObjects"></param>
        /// <returns></returns>
        private IEnumerator TypeSentence(DialogueObject[] dialogueObjects)
        {
            string tempString = "";
            foreach (char character in dialogueObjects[dialogueObjectIndex].DialogueText)
            {
                tempString += character;
                _dialogueText.text = tempString;
                yield return new WaitForSeconds(0.005f);
            }

            yield return new WaitForSeconds(0.5f);
            _continueIcon.SetActive(true);

            yield return new WaitUntil(() => { return Input.GetButtonDown("Submit"); });
            dialogueObjectIndex++;
            DisplayNextDialogue(dialogueObjects);

        }
    }
}


