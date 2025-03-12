using Dialogue;
using Models;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.EventSystems;


namespace UnityEngine.UI
{
    public class UI_ExamQuestions : MonoBehaviour
    {
        UI_ExamUI examUI;
        [SerializeField] int currentQuestionIndex = 0;

        [Header("SUBTITLE FIELDS")]
        [SerializeField] TextMeshProUGUI subtitleDisplay;

        [Header("QUESTION FIELDS")]
        [SerializeField] TextMeshProUGUI singularTextDisplay;
        [SerializeField] TextMeshProUGUI jointTextDisplay;
        [SerializeField] RawImage jointImageDisplay;

        [Header("ANSWER FIELDS")]
        [SerializeField] TMP_InputField inputField;
        [SerializeField] TextMeshProUGUI[] buttonInputFields;

        [Header("PROGRESS FIELDS")]
        [SerializeField] GameObject previousButton;
        [SerializeField] GameObject nextButton;
        [SerializeField] Transform navigationParent;
        [SerializeField] UI_NavigationButton navigationButtonPrefab;
        [SerializeField] List<UI_NavigationButton> navigationButtons;




        void Start()
        {
            examUI = GetComponentInParent<UI_ExamUI>();
            UI_NavigationButton.OnButtonPressed += UI_NavigationButton_OnButtonPressed;
            navigationButtons = new List<UI_NavigationButton>() { navigationButtonPrefab };


            for (int i = 0; i < examUI.exam.examQuestions.Length - 1; i++)
            {
                UI_NavigationButton obj = Instantiate(navigationButtonPrefab, navigationParent);
                obj.InitialiseButton((i + 2).ToString("00"));
                navigationButtons.Add(obj);
            }
            nextButton.SetActive(true);
            previousButton.SetActive(false);
            currentQuestionIndex = 0;


            foreach (ExamQuestion examQuestion in examUI.exam.examQuestions)
            {
                examQuestion.multipleChoiceQuestion.selectedAnswerIndex = -1;
                examQuestion.textInput.inputAnswer = "";
            }

            SetQuestion(currentQuestionIndex);
            UpdateNavigation();
        }

        private void SetQuestion(int index)
        {
            currentQuestionIndex = index;
            UpdateNavigation();
            DisplayQuestion();
        }

        private void DisplayQuestion()
        {

            ExamQuestion examQuestion = examUI.exam[currentQuestionIndex];
            subtitleDisplay.text = "QUESTION " + (currentQuestionIndex + 1).ToString("00");

            singularTextDisplay.gameObject.SetActive(!examQuestion.questionTexture);

            singularTextDisplay.text = examQuestion.question;
            jointTextDisplay.text = examQuestion.question;

            jointTextDisplay.gameObject.SetActive(examQuestion.questionTexture);
            jointImageDisplay.gameObject.SetActive(examQuestion.questionTexture);
            jointImageDisplay.texture = examQuestion.questionTexture;

            inputField.gameObject.SetActive(!examQuestion.IsMultipleChoiceInput());
            for (int i = 0; i < buttonInputFields.Length; i++)
            {
                buttonInputFields[i].transform.parent.gameObject.SetActive(examQuestion.IsMultipleChoiceInput());
                if (!examQuestion.IsMultipleChoiceInput()) { continue; }
                buttonInputFields[i].text = examQuestion.multipleChoiceQuestion.answers[i];
            }

            Transform answerButtonParent = buttonInputFields[0].transform.parent.parent;
            foreach (Transform child in answerButtonParent)
            {
                child.GetComponent<Button>().image.color = child.GetSiblingIndex() == examQuestion.multipleChoiceQuestion.selectedAnswerIndex ? Color.black : Color.white;
                child.GetComponentInChildren<TextMeshProUGUI>().color = child.GetSiblingIndex() == examQuestion.multipleChoiceQuestion.selectedAnswerIndex ? Color.white : Color.black;
            }
        }

        private void UI_NavigationButton_OnButtonPressed(int obj)
        {
            if (examUI.GetUIInterface() == UI_ExamUI.ExamUIInterface.FINAL_PANEL)
            {
                examUI.SetExamUIInterface(UI_ExamUI.ExamUIInterface.EXAM_PANEL);
            }
            SetQuestion(obj);
        }



        public void OnNextButtonPressed()
        {
            if (currentQuestionIndex < examUI.exam.examQuestions.Length)
            {
                currentQuestionIndex++;
            }

            if (currentQuestionIndex == examUI.exam.examQuestions.Length)
            {
                examUI.SetExamUIInterface(UI_ExamUI.ExamUIInterface.FINAL_PANEL);
                return;
            }

            DisplayQuestion();
            nextButton.SetActive(currentQuestionIndex < examUI.exam.examQuestions.Length);
            UpdateNavigation();
            previousButton.SetActive(true);
        }

        public void OnPreviousButtonPressed()
        {
            if (currentQuestionIndex > 0)
            {
                currentQuestionIndex--;
            }

            if (examUI.GetUIInterface() == UI_ExamUI.ExamUIInterface.FINAL_PANEL){
                examUI.SetExamUIInterface(UI_ExamUI.ExamUIInterface.EXAM_PANEL);
            }

            DisplayQuestion();
            previousButton.SetActive(currentQuestionIndex > 0);
            UpdateNavigation();
            nextButton.SetActive(true);
        }

        public void OnAnswerSelected(Transform transform)
        {
            foreach (Transform child in transform.parent)
            {
                child.GetComponent<Button>().image.color = child == transform ? Color.black : Color.white;
                child.GetComponentInChildren<TextMeshProUGUI>().color = child == transform ? Color.white : Color.black;
            }
            examUI.exam[currentQuestionIndex].multipleChoiceQuestion.selectedAnswerIndex = transform.GetSiblingIndex();
        }

        private void UpdateNavigation()
        {
            foreach (var navigationButton in navigationButtons)
            {
                bool questionAnswered = examUI.exam[navigationButton.transform.GetSiblingIndex()].multipleChoiceQuestion.selectedAnswerIndex != -1
                    || !string.IsNullOrEmpty(examUI.exam[navigationButton.transform.GetSiblingIndex()].textInput.inputAnswer);
                navigationButton.SetButtonSelected(navigationButton.transform.GetSiblingIndex() == currentQuestionIndex, questionAnswered);
            }
        }

        void Update()
        {
            if (!examUI?.ExamStarted ?? true) { return; }

            examUI.exam[currentQuestionIndex].timeTakenToAnswer += Time.deltaTime;
        }


        private void OnDestroy()
        {
            UI_NavigationButton.OnButtonPressed -= UI_NavigationButton_OnButtonPressed;
        }
    }

}