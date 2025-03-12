using Dialogue;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

namespace UnityEngine.UI
{
    public class UI_ExamBriefUI : MonoBehaviour
    {
        UI_ExamUI examUI;

        [Header("EXAM BRIEF")]
        [SerializeField] TextMeshProUGUI examBriefDisplay;
        [SerializeField] [TextArea(5, 5)] string examBrief;
        void Start()
        {
            examBriefDisplay.text = examBrief;
            examUI = GetComponentInParent<UI_ExamUI>();
        }

        /// <summary>
        /// Method called on begin exam button press
        /// </summary>
        public void BeginExam()
        {
            examUI.StartExam();
        }

        /// <summary>
        /// Method called on Return button press
        /// </summary>
        public void Return()
        {
            SCR_GeneralManager.UIManager.FindUIObject<UI_LoadScene>().LoadScene(new UI_LoadScene.TransitionProperties
            {
                SceneName = "Splash Scene",
                EnablePlayerControls = true
            });
        }

    }
}
