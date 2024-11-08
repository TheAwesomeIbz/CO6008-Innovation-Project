using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using System;


namespace Dialogue
{
    public class SCR_DialogueManager : MonoBehaviour
    {
        public static event Action<DialogueObject[]> OnDialogueStart;
        public static event Action OnDialogueEnd;

        [Header("DIALOGUE MANAGER PROPERTIES")]
        [SerializeField] GameObject _dialogueBox;
        [SerializeField] GameObject _nameBox, _continueIcon;
        [SerializeField] TextMeshProUGUI _dialogueText, _nameText;

        [Header("DIALOGUE OBJECTS PROPERTIES")]
        [SerializeField] int dialogueObjectIndex;

        
        // Start is called before the first frame update
        void Start()
        {
            _dialogueBox.SetActive(false);
            _nameBox.SetActive(false);
            _continueIcon.SetActive(false);
        }

        // Update is called once per frame
        void Update()
        {

        }

        public void DisplayDialogue(DialogueObject[] dialogueObjects)
        {
            dialogueObjectIndex = 0;
            SetDialogueActivity(true, dialogueObjects[0]);
            SCR_PlayerInputManager.PlayerControlsEnabled = false;
            OnDialogueStart?.Invoke(dialogueObjects);

            DisplayNextDialogue(dialogueObjects);

            foreach (DialogueObject dialogueObject in dialogueObjects)
            {
                print(dialogueObject.DialogueText + "\n");
            }
        }

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

        private void SetDialogueActivity(bool dialogueActivity, DialogueObject dialogueObject)
        {
            _dialogueBox.gameObject.SetActive(dialogueActivity);
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

        private void EndDialogueSequence()
        {
            SetDialogueActivity(false, null);
            OnDialogueEnd?.Invoke();
            SCR_PlayerInputManager.PlayerControlsEnabled = true;
        }

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


