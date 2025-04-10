using System;
using System.Collections;
using System.Collections.Generic;
using Dialogue;
using TMPro;
using UnityEngine;
using UnityEngine.EventSystems;

namespace UnityEngine.UI
{
    public class UI_SolveCubicBossUI : MonoBehaviour
    {
        [Header("CUBIC BOSS UI PROPERTIES")] 
        [SerializeField] private TextMeshProUGUI titleText;
        [SerializeField] private TextMeshProUGUI descriptionText;
        [SerializeField] private RawImage image;
        [SerializeField] private TMP_InputField inputField;
        [SerializeField] private Transform parentAnswerObjects;
        [SerializeField] private Button submitButton;
        
        [Header("QUESTION INDEX PROPERTIES")]
        [SerializeField] private List<Questions> questions;
        [SerializeField] private List<Answers> answers;
        [SerializeField] private List<string> inputAnswers;
        [SerializeField] private int currentQuestionIndex;
        
        private List<TextMeshProUGUI> answerObjects = new List<TextMeshProUGUI>();

        private const string bossID = "BOSS3";
        private float currentTime;
        
        [Header("UPLOAD INDEX PROPERTIES")]
        [SerializeField] SCR_UploadDataModule uploadDataModule;
        
        void Start()
        {
            foreach (Transform child in parentAnswerObjects)
            {
                answerObjects.Add(child.GetComponentInChildren<TextMeshProUGUI>());
                child.gameObject.SetActive(false);
            }

            currentQuestionIndex = 0;
            inputAnswers = new List<string>();
            currentTime = Time.time;
            UpdateQuestion();

        }
        
        void Update()
        {
            
            if (SCR_GeneralManager.PlayerInputManager.Submit.PressedThisFrame() && 
                currentQuestionIndex < questions.Count) {
                OnContinueButtonPressed();
            }

        }
        
        /// <returns>Whether the input is empty or not</returns>
        private bool IsEmptyInput()
        {
            if (string.IsNullOrEmpty(inputField.text))
            {
                SCR_GeneralManager.UIManager.FindUIObject<SCR_DialogueManager>().DisplayDialogue(DialogueObject.CreateDialogue(
                    "Please enter in an answer within this field before you continue."), OnDialogueEnd: () =>
                {
                    submitButton.interactable = true;
                    inputField.interactable = true;
                    EventSystem.current.SetSelectedGameObject(inputField.gameObject);
                });
                return true;
            }
            return false;
        }

        /// <returns>Whether the answer provided is valid or not</returns>
        private bool IsValidAnswer(string formattedAnswer)
        {
            bool validAnswer = answers[currentQuestionIndex].ValidAnswer(formattedAnswer) &&
                               !inputAnswers.Contains(formattedAnswer);
            ;
            if (!validAnswer)
            {
                SCR_GeneralManager.UIManager.FindUIObject<SCR_DialogueManager>().DisplayDialogue(DialogueObject.CreateDialogue(
                    $"Unfortunately {inputField.text} is incorrect.", "Please recheck or redo your calculations again."), OnDialogueEnd: () =>
                {
                    submitButton.interactable = true;
                    inputField.interactable = true;
                    EventSystem.current.SetSelectedGameObject(inputField.gameObject);
                });
            }
            return validAnswer;
        }
        
        /// <summary>
        /// Called to continue on with answering the question
        /// </summary>
        public void OnContinueButtonPressed()
        {
            submitButton.interactable = false;
            inputField.interactable = false;
            
            if (IsEmptyInput()) { return;}
            
            string formattedAnswer = inputField.text
                .ToLower()
                .Replace(" ", "");
            
            if (!IsValidAnswer(formattedAnswer)) { return;}
            OnAnsweredSuccessfully(formattedAnswer);
        }

        
        private void OnAnsweredSuccessfully(string formattedAnswer)
        {
            inputAnswers.Add(formattedAnswer);
            answerObjects[currentQuestionIndex].text += formattedAnswer;
            inputField.text = "";
            currentQuestionIndex++;
            
            foreach (TextMeshProUGUI answerObject in answerObjects)
            {
                Transform answerObjectParent = answerObject.transform.parent;
                answerObjectParent.gameObject.SetActive(answerObjectParent.GetSiblingIndex() < currentQuestionIndex);
            }

            UpdateQuestion();
        }

        private void UpdateQuestion()
        {
            if (currentQuestionIndex >= questions.Count)
            {
                OnCubicBossCompleted();
                return;
            }
            
            titleText.text = questions[currentQuestionIndex].Title;
            descriptionText.text = questions[currentQuestionIndex].Description;
            image.texture = questions[currentQuestionIndex].ImageTexture;
            submitButton.interactable = true;
            inputField.interactable = true;
            EventSystem.current.SetSelectedGameObject(inputField.gameObject);
        }

        /// <summary>
        /// Called when the boss is completed and the cubic equation is solved.
        /// Adds the boss to the current 
        /// </summary>
        private void OnCubicBossCompleted()
        {
            LevelData existingLevel =
                SCR_GeneralManager.LevelManager.GetLevelInformation.Find(lvl => lvl.LevelID == bossID);

            //remove the existing level if the time achieved is less than the current time
            float elapsedTime = Time.time - currentTime;
            if (existingLevel != null && elapsedTime < existingLevel.LevelCompletedTime) {
                SCR_GeneralManager.LevelManager.GetLevelInformation.Remove(existingLevel);
            }
            
            //add it to the level information and save progress
            LevelData levelData = new LevelData(bossID, elapsedTime);
            SCR_GeneralManager.LevelManager.GetLevelInformation.Add(levelData);
            
            SCR_GeneralManager.UIManager.FindUIObject<SCR_DialogueManager>().DisplayDialogue(DialogueObject.CreateDialogue(
                "Congratulations on solving the final boss! Your journey is over!",
                $"You have successfully finished PROJECT POLYNOMIAL VERSION {Application.unityVersion}!",
                "Your save data will be analysed and recorded for future testing. Thank you very much for your time and feedback!"), 
                OnDialogueEnd: () =>
                {
                    StartCoroutine(OnCompletionDialogueFinish());
                });


            
            IEnumerator OnCompletionDialogueFinish()
            {
                //save the player's information with the GameCompletionPlayerData() function
                //done to save the player's progress to the original map and starting position
                SavingOperations.SaveInformation(SCR_GeneralManager.Instance.PlayerData.GameCompletionPlayerData);
                string jsonData = JsonUtility.ToJson(SavingOperations.LoadInformation());
                
                //upload the data to an external database
                yield return uploadDataModule.PostData(jsonData);
            }
            
            
        }
        
        
        /// <summary>
        /// Struct that contains the properties for a question for the cubic equation
        /// </summary>
        [Serializable] struct Questions
        {
            [SerializeField] private string title;
            [SerializeField] [TextArea(4,4)] private string description;
            [SerializeField] private Texture2D imageTexture;
            
            public string Title => title;
            public string Description => description;
            public Texture2D ImageTexture => imageTexture;
        }

        
        /// <summary>
        /// Struct that contains the list of possible answers acceptable for a given question
        /// </summary>
        [Serializable] struct Answers
        {
            [SerializeField] private List<string> possibleAnswers;

            public bool ValidAnswer(string answer)
            {
                return possibleAnswers.Contains(answer);
            }
        }
        
    }
}
