using Models;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

namespace UnityEngine.UI
{
    public class UI_ExamFinalPanel : MonoBehaviour
    {
        UI_ExamUI examUI;
        [SerializeField] TextMeshProUGUI questionPrefab;
        List<TextMeshProUGUI> allQuestions;
        void Start()
        {
            examUI = GetComponentInParent<UI_ExamUI>();
            examUI.OnInterfaceChanged += ExamUI_OnInterfaceChanged;

            Transform parentObject = questionPrefab.transform.parent;


            questionPrefab.text = $"Q1 | {(examUI.exam[0].QuestionAnswered() ? "<color=green>ANSWERED</color>" : "<color=red>NOT ANSWERED</color>")}";
            allQuestions = new List<TextMeshProUGUI>() { questionPrefab };
            for (int i = 1; i < examUI.exam.examQuestions.Length; i++)
            {
                TextMeshProUGUI obj = Instantiate(questionPrefab, parentObject);
                obj.text = $"Q{i + 1} | {(examUI.exam[0].QuestionAnswered() ? "<color=green>ANSWERED</color>" : "<color=red>NOT ANSWERED</color>")}";
                allQuestions.Add(obj);
            }
        }

        private void ExamUI_OnInterfaceChanged(UI_ExamUI.ExamUIInterface obj)
        {
            if (obj.Equals(UI_ExamUI.ExamUIInterface.FINAL_PANEL)){
                UpdateAllQuestion();
            }
            
        }

        private void UpdateAllQuestion()
        {
            for (int i = 0; i < allQuestions.Count; i++)
            {
                allQuestions[i].text = $"Q{i + 1} | {(examUI.exam[i].QuestionAnswered() ? "<color=green>ANSWERED</color>" : "<color=red>NOT ANSWERED</color>")}";
            }
        }

        private void OnDestroy()
        {
            examUI.OnInterfaceChanged -= ExamUI_OnInterfaceChanged;
        }
    }
}
