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
        

        [SerializeField] private Toggle[] examConfirmationToggles;
        [SerializeField] private Button[] examConfirmationButtons;
        void Start()
        {
            examUI = GetComponentInParent<UI_ExamUI>();
        }

        private bool AllTogglesSelected()
        {
            foreach (Toggle toggle in examConfirmationToggles)
            {
                if (!toggle.isOn)
                {
                    SCR_GeneralManager.UIManager.FindUIObject<SCR_DialogueManager>().DisplayDialogue(
                        DialogueObject.CreateDialogue("Please agree to all the terms provided above before you take part within this examination."),
                        OnDialogueEnd: () => SetInteractableElementsActivity(true));
                    return false;
                }
            }
            return true;
        }

        /// <summary>
        /// Method called on begin exam button press
        /// </summary>
        public void BeginExam()
        {
            if (!AllTogglesSelected())
            {
                SetInteractableElementsActivity(false);
                return;
            }
            
            examUI.StartExam();
        }


        private void SetInteractableElementsActivity(bool state)
        {
            foreach (Toggle toggle in examConfirmationToggles)
            {
                toggle.interactable = state;
            }

            foreach (Button button in examConfirmationButtons)
            {
                button.interactable = state;
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
