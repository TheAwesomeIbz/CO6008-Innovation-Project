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

        [SerializeField] GameObject[] examPanels;
        [SerializeField] float examTimeRemaining;
        [SerializeField] int examDuration = 900;
        [SerializeField] GameObject backgroundPanel;
        
        bool examStarted = false;
        public bool ExamStarted => examStarted;

        [Header("UI PROPERTIES")]
        [SerializeField] TextMeshProUGUI timerDisplay;
        [SerializeField] GameObject navigationDisplay;

        [SerializeField] GameObject saveDataObject;

        private void Start()
        {
            backgroundPanel.SetActive(false);
            saveDataObject.SetActive(false);
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
            string formURl = "https://docs.google.com/forms/u/0/d/e/1FAIpQLSeERGsKZcJua20tXoz7GYX-DF57v0op2Lt07b2qp7UY9Ri-pA/formResponse";
            string JSONString = JsonUtility.ToJson(exam);

            StartCoroutine(PostData(JSONString));

           
            IEnumerator PostData(string jsonData)
            {
                WWWForm form = new WWWForm();
                form.AddField("entry.1448748991", jsonData);
                UnityWebRequest webRequest = UnityWebRequest.Post(formURl, form);

                saveDataObject.SetActive(true);
                yield return webRequest.SendWebRequest();
                saveDataObject.SetActive(false);

                DialogueObject[] resultingDialogue;
                Action resultingAction = null;

                switch (webRequest.result)
                {
                    case UnityWebRequest.Result.Success:
                        File.WriteAllText(Application.persistentDataPath + "/ExamCompleted.txt", JsonUtility.ToJson(""));
                        resultingDialogue = DialogueObject.CreateDialogue("Successfully saved results to external storage!", "Thank you for your patience!");
                        resultingAction = () =>
                        {
                            SCR_GeneralManager.UIManager.FindUIObject<UI_LoadScene>().LoadScene(new UI_LoadScene.TransitionProperties
                            {
                                SceneName = "Splash Scene",
                                EnablePlayerControls = true
                            });
                        };
                        break;
                    default:

                        ChoiceDialogueObject.ChoiceOption yesOption = new ChoiceDialogueObject.ChoiceOption("Yes", null,
                                onChoiceMade: () =>
                                {
                                    SubmitResults();
                                });
                        ChoiceDialogueObject.ChoiceOption noOption = new ChoiceDialogueObject.ChoiceOption("No", null,
                            onChoiceMade: () =>
                            {
                                SCR_GeneralManager.UIManager.FindUIObject<UI_LoadScene>().LoadScene(new UI_LoadScene.TransitionProperties
                                {
                                    SceneName = "Splash Scene",
                                    EnablePlayerControls = true
                                });
                            });

                        ChoiceDialogueObject choiceDialogue = new ChoiceDialogueObject(
                            choiceOptions: new ChoiceDialogueObject.ChoiceOption[] { yesOption, noOption },
                            nonImpactingChoice: true,
                            _speakingCharacter: "",
                            _dialogueText: "Would you like to try submitting your answers again?");

                        resultingDialogue = new DialogueObject[]
                        {
                            new DialogueObject("An error occured with submitting your results."),
                            new DialogueObject(webRequest.error + "."),
                            choiceDialogue,
                        };

                        break;
                }

                SCR_GeneralManager.UIManager.FindUIObject<SCR_DialogueManager>().DisplayDialogue(resultingDialogue, resultingAction);
            }
        }

        public enum ExamUIInterface
        {
            BRIEF_PANEL = 0,
            EXAM_PANEL = 1,
            FINAL_PANEL = 2
        }
    }
}
