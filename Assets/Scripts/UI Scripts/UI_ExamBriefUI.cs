using Dialogue;
using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

namespace UnityEngine.UI
{
    public class UI_ExamBriefUI : MonoBehaviour
    {
        UI_ExamUI examUI;

        void Start()
        {
            examUI = GetComponentInParent<UI_ExamUI>();
        }


        /// <summary>
        /// Method called on begin exam button press
        /// </summary>
        public void BeginExam()
        {
            examUI.StartExam();
        }

        public void OpenLearningMaterial()
        {
            try
            {
                System.Diagnostics.Process.Start(Application.dataPath + "/External Material/Learning Material.pdf");
            }
            catch (Exception ex)
            {
                Debug.LogWarning($"An unexpected error occured. {ex.Message}");
                System.Diagnostics.Process.Start(Application.dataPath + "/External Material");
            }
            
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
