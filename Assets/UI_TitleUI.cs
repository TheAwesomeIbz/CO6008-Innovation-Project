using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.UIElements.Experimental;

namespace UnityEngine.UI.Title
{
    public class UI_TitleUI : MonoBehaviour
    {
        [Header("BUTTON PROPERTIES")]
        [SerializeField] private Button _newGameButton;
        [SerializeField] private Button _continueButton;
        [SerializeField] private Button _deleteGameButton;
        UI_TitleButtons _continueButtonTitle;

        [Header("HOVER PROPERTIES")]
        [SerializeField] private Transform _parentObject;
        HoverUIObject _hoverUIObject;

        [Header("SAVE DATA PROPERTIES")]
        [SerializeField] private SaveData _saveData;

        void Start()
        {
            _continueButtonTitle = _continueButton.GetComponent<UI_TitleButtons>();

            UpdateButtonState();
            _hoverUIObject = new HoverUIObject(_parentObject);
            _hoverUIObject.SetItemText("", "", false);

            _saveData = SavingOperations.LoadInformation();
        }


        private void UpdateButtonState()
        {
            bool fileExists = File.Exists(SavingOperations.SaveDataPath);

            _newGameButton.gameObject.SetActive(!fileExists);
            _continueButton.gameObject.SetActive(fileExists);
            _deleteGameButton.gameObject.SetActive(fileExists);
        }
        // Update is called once per frame
        void Update()
        {
            _parentObject.gameObject.SetActive(!_hoverUIObject.HasEmptyFields);
            _hoverUIObject.SetMousePosition();

            if (_saveData != null)
            {
                _continueButtonTitle.name = $"Continue ({_saveData.PlayerData.PlayerName})";
                _continueButtonTitle.SetDescription(
                    $"Last saved on <color=green>{_saveData.PlayerData.DateLastSaved[0]}/{_saveData.PlayerData.DateLastSaved[1]}/{_saveData.PlayerData.DateLastSaved[2]}</color> at <color=green>{_saveData.PlayerData.DateLastSaved[3]}</color>\n" +
                    $"First started on <color=green>{_saveData.PlayerData.DateStarted[0]}/{_saveData.PlayerData.DateStarted[1]}/{_saveData.PlayerData.DateStarted[2]}</color> at <color=green>{_saveData.PlayerData.DateStarted[3]}</color>");
            }
        }

        public void UpdateHoverUI(string title, string description)
        {
            _hoverUIObject.SetItemText(title, description, true);
        }

        public void OnNewGameSelected()
        {
            SCR_GeneralManager.UIManager.FindUIObject<UI_LoadScenes>().LoadScene(new UI_LoadScenes.TransitionProperties
            {
                SceneName = "Naming Scene",
                OnTransitionFinished = () => { SCR_PlayerInputManager.PlayerControlsEnabled = true; }
            });
            
        }

        public void OnContinueSelected()
        {
            SCR_GeneralManager.UIManager.FindUIObject<UI_LoadScenes>().LoadScene(new UI_LoadScenes.TransitionProperties
            {
                SceneName = _saveData.PlayerData.RecentSceneName,
                OnSceneLoaded = () => {
                    SavingOperations.LoadInformation();
                },
                OnTransitionFinished = () =>
                {
                    SCR_PlayerInputManager.PlayerControlsEnabled = true;
                }
                
            });
            //if (_saveData == null) { return; }
            //string description = $"<color=blue>{_saveData.PlayerData.PlayerName} </color>";
            //titleButton.SetDescription(description);
        }

        public void OnDeleteSaveSelected()
        {
            _saveData = null;
            File.Delete(SavingOperations.SaveDataPath);
            UpdateButtonState();
        }

        public void OnSettingsSelected()
        {

        }
        
        public void OnInformationSelected()
        {

        }

        public void OnQuitSelected()
        {
            Application.Quit();
        }
    }

}