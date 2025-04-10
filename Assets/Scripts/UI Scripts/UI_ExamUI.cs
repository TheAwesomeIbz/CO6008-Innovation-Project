using Dialogue;
using Models;
using System;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using System.IO;
using System.Threading.Tasks;
using TMPro;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.Networking;
using UnityEngine.Rendering;
using UnityEngine.SceneManagement;
using static UnityEngine.UI.UI_ExamUI;
using static UnityEngine.UI.UI_LoadScene;

namespace UnityEngine.UI
{
    public class UI_ExamUI : MonoBehaviour
    {
        public event System.Action OnExamEnd;
        public event System.Action<ExamUIInterface> OnInterfaceChanged;

        [Header("EXAM PROPERTIES")]
        public Exam exam;
        public bool PlayedProjectPolynomial = false;

        [SerializeField] GameObject[] examPanels;
        [SerializeField] float examTimeRemaining;
        [SerializeField] int examDuration = 900;
        [SerializeField] GameObject backgroundPanel;
        
        bool examStarted = false;
        public bool ExamStarted => examStarted;

        [Header("UI PROPERTIES")]
        [SerializeField] TextMeshProUGUI timerDisplay;
        [SerializeField] GameObject navigationDisplay;

        [SerializeField] SCR_UploadDataModule uploadDataModule;

        private void Start()
        {
            backgroundPanel.SetActive(false);
            SetExamUIInterface(ExamUIInterface.BRIEF_PANEL);
            SetTimerAndNavigationState(false);
            
            
            
        }
        public void StartExam()
        {
            SetExamUIInterface(ExamUIInterface.EXAM_PANEL);
            SetTimerAndNavigationState(true);
            examStarted = true;
            examTimeRemaining = examDuration;
        }

        public void SetTimerAndNavigationState(bool state)
        {
            timerDisplay.gameObject.SetActive(state);
            navigationDisplay.SetActive(state);
        }

        public void SetExamUIInterface(ExamUIInterface examUIInterface)
        {
            for (int i = 0; i < examPanels.Length; i++)
            {
                examPanels[i].SetActive(i == (int)examUIInterface);
            }
            OnInterfaceChanged?.Invoke(examUIInterface);
        }

        public ExamUIInterface GetUIInterface()
        {
            for (int i = 0; i < examPanels.Length; i++)
            {
                if (examPanels[i].activeInHierarchy)
                    return (ExamUIInterface)i;
            }
            return default;
        }

        private string FormatUserData(Exam exam)
        {
            UserData userData = new UserData();
            bool saveDataExists = SavingOperations.LoadInformation() != null;
            userData.playedProjectPolynomial = saveDataExists;

            for (int i = 0; i < exam.examQuestions.Length; i++)
            {
                string questionKey = $"Q{i + 1}";
                userData.questionData.Add(new QuestionData
                {
                    QuestionID = questionKey,
                    CorrectAnswer = exam[i].IsCorrectAnswer(),
                    TimeTakenToAnswer = exam[i].timeTakenToAnswer
                });
            }

            return JsonUtility.ToJson(userData);
        }

        public void OnSubmitButtonPressed()
        {
            
            ChoiceDialogueObject.ChoiceOption yesOption = new ChoiceDialogueObject.ChoiceOption("Yes", null,
                onChoiceMade: () =>
                {
                    SubmitResults();
                });
            ChoiceDialogueObject.ChoiceOption noOption = new ChoiceDialogueObject.ChoiceOption("No", null, 
                onChoiceMade: () =>
                {
                    backgroundPanel.SetActive(false);
                });
            ChoiceDialogueObject choiceDialogue = new ChoiceDialogueObject(
                choiceOptions: new ChoiceDialogueObject.ChoiceOption[] { yesOption, noOption },
                nonImpactingChoice: true,
                _speakingCharacter: "CONFIRMATION",
                _dialogueText: "Are you sure you want to submit your final answers? You will NOT be able to attempt this exam again.");


            backgroundPanel.SetActive(true);
            EventSystem.current.SetSelectedGameObject(null);
            SCR_GeneralManager.UIManager.FindUIObject<SCR_DialogueManager>().DisplayDialogue(new ChoiceDialogueObject[] { choiceDialogue });
        }
        void Update()
        {
            if (!examStarted) { return; }

            examTimeRemaining -= examTimeRemaining > 0 ? Time.deltaTime : 0;
            timerDisplay.text = string.Format("{01:00}:{00:00}", Mathf.Floor(examTimeRemaining % 60), Mathf.Floor(examTimeRemaining / 60));

            if (examTimeRemaining <= 0)
            {
                //exam has finished, and flip exam started case so it does not run again
                OnExamEnd?.Invoke();
                examStarted = false;
                timerDisplay.text = "00:00";

                EventSystem.current.SetSelectedGameObject(null);
                backgroundPanel.gameObject.SetActive(true);
                SCR_DialogueManager dialogueManager = SCR_GeneralManager.UIManager.FindUIObject<SCR_DialogueManager>();
                dialogueManager.EndDialogue();

                dialogueManager.DisplayDialogue(DialogueObject.CreateDialogue($"The {examDuration % 60} minutes available for this exam are over.", "Your response will be recorded automatically.", "Thank you for taking part in this research."),
                    OnDialogueEnd: () =>
                    {
                        SubmitResults();
                    });
            }
        }


        private void SubmitResults()
        {
            string JSONString = FormatUserData(exam);
            StartCoroutine(uploadDataModule.PostData(JSONString));
            
        }

        public enum ExamUIInterface
        {
            BRIEF_PANEL = 0,
            EXAM_PANEL = 1,
            FINAL_PANEL = 2
        }
    }
}
