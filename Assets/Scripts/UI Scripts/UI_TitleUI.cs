using Dialogue;
using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.UIElements.Experimental;

namespace UnityEngine.UI.Title
{
    /// <summary>
    /// UI Class responsible for managing the title screen and other important UI elements 
    /// </summary>
    public class UI_TitleUI : MonoBehaviour
    {
        [Header("BUTTON PROPERTIES")]
        [SerializeField] Transform parentButtonObject; 
        [SerializeField] private List<Button> buttons = new List<Button>();
        private Button _newGameButton;
        private Button _continueButton;
        private Button _deleteGameButton;

        [field : Header("SAVE DATA PROPERTIES")]
        [field : SerializeField] public SaveData SaveData { get; private set; }

        void Awake()
        {
            SaveData = SavingOperations.LoadInformation();
            
            
            buttons = new List<Button>();
            foreach (Transform child in parentButtonObject){
                buttons.Add(child.GetComponent<Button>());
            }

            _newGameButton = buttons[0];
            _continueButton = buttons[1];
            _deleteGameButton = buttons[2];

            UpdateButtonState();
        }


        /// <summary>
        /// Updates what buttons should be shown depending on if a save file exists
        /// </summary>
        private void UpdateButtonState()
        {
            bool fileExists = File.Exists(SavingOperations.SaveDataPath);

            _newGameButton.gameObject.SetActive(!fileExists);
            _continueButton.gameObject.SetActive(fileExists);
            _deleteGameButton.gameObject.SetActive(fileExists);
        }

        private void SetButtonActivity(bool activity)
        {
            foreach (Button button in buttons)
            {
                button.interactable = activity;
            }
        }


        #region BUTTON METHONDS
        public void OnNewGameSelected()
        {
            SCR_GeneralManager.UIManager.FindUIObject<UI_LoadScene>().LoadScene(new UI_LoadScene.TransitionProperties
            {
                SceneName = "Naming Scene",
                OnTransitionFinished = () => { SCR_PlayerInputManager.PlayerControlsEnabled = true; }
            });
            
        }

        public void OnContinueSelected()
        {
            SCR_GeneralManager.UIManager.FindUIObject<UI_LoadScene>().LoadScene(new UI_LoadScene.TransitionProperties
            {
                SceneName = SaveData.PlayerData.RecentSceneName,
                OnSceneLoaded = () => {
                    SavingOperations.LoadInformation();
                },
                OnTransitionFinished = () =>
                {
                    SCR_PlayerInputManager.PlayerControlsEnabled = true;
                }
                
            });
        }

        private ChoiceDialogueObject[] OnDeleteButtonDialogue()
        {
            Action onYesChoiceSelected = () =>
            {
                SaveData = null;
                File.Delete(SavingOperations.SaveDataPath);
                UpdateButtonState();
            };


            ChoiceDialogueObject.ChoiceOption yesChoice = new ChoiceDialogueObject.ChoiceOption("YES", null, onYesChoiceSelected);
            ChoiceDialogueObject.ChoiceOption noChoice = new ChoiceDialogueObject.ChoiceOption("NO", null, null);

            string deletionText = $"Do you wish to delete {SaveData.PlayerData.PlayerName}'s progress? This action CANNOT be undone.";

            ChoiceDialogueObject dialogueObject = new ChoiceDialogueObject(new ChoiceDialogueObject.ChoiceOption[] { yesChoice, noChoice }, true, "", deletionText);
            return new ChoiceDialogueObject[] {dialogueObject};
        }
        public void OnDeleteSaveSelected()
        {
            SetButtonActivity(false);
            SCR_GeneralManager.UIManager.FindUIObject<SCR_DialogueManager>().DisplayDialogue(OnDeleteButtonDialogue(), () => SetButtonActivity(true));
        }

        public void OnSettingsSelected()
        {
            SCR_GeneralManager.UIManager.FindUIObject<UI_SettingsUI>().DisplaySettingsUI(true);
        }
        
        public void OnInformationSelected()
        {
            SCR_GeneralManager.UIManager.FindUIObject<UI_LoadScene>().LoadScene(new UI_LoadScene.TransitionProperties { SceneName = "Splash Scene", EnablePlayerControls = true });
        }

        public void OnQuitSelected()
        {
            Application.Quit();
        }

        #endregion
    }

}