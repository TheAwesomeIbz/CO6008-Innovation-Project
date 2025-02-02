using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.Rendering;

namespace UnityEngine.UI
{
    public class UI_SettingsUI : MonoBehaviour
    {
        [field : SerializeField] public bool SettingsMenuEnabled { get; private set; }
        [field: SerializeField] public SettingsInformation SettingsInformation { get; private set; }

        [Header("SETTINGS UI PROPERTIES")]
        [SerializeField] GameObject settingsParentObject;

        [SerializeField] TMP_Dropdown gameModeDropdown;
        [SerializeField] TMP_Dropdown textSpeedDropdown;
        [SerializeField] Slider uiScaleSlider;
        [SerializeField] TextMeshProUGUI uiScaleText;
        [SerializeField] Slider audioScaleSlider;
        [SerializeField] TextMeshProUGUI audioScaleText;

        void Awake()
        {
            Settings settings = new Settings();
            SettingsInformation = settings.SettingsInformation;
            InitialiseSettingsUI();
            SettingsMenuEnabled = false;
            settingsParentObject.gameObject.SetActive(false);

        }

        /// <summary>
        /// Sets the settings UI object to the settings information values
        /// </summary>
        private void InitialiseSettingsUI()
        {
            gameModeDropdown.value = (int)SettingsInformation.GameMode;
            textSpeedDropdown.value = (int)SettingsInformation.TextSpeed;
            uiScaleSlider.value = (SettingsInformation.UIScale - 0.75f) * 2f;
            audioScaleSlider.value = SettingsInformation.AudioVolume;
        }

        public void DisplaySettingsUI(bool activity)
        {
            SettingsMenuEnabled = activity;
            settingsParentObject.gameObject.SetActive(SettingsMenuEnabled);
            if (!SettingsMenuEnabled)
            {
                SettingsInformation.SaveSettings();
            }
        }

        #region BUTTON METHODS

        public void OnGameModeUpdated()
        {
            SettingsInformation.GameMode = (Settings.SettingOption)gameModeDropdown.value;
        }

        public void OnTextSpeedUpdated()
        {
            SettingsInformation.TextSpeed = (Settings.SettingOption)textSpeedDropdown.value;
        }

        public void OnUIScaleUpdated()
        {
            double processedValue = Math.Round((uiScaleSlider.value / 2f) + 0.75f, 2); 
            uiScaleText.text = $"{processedValue * 100}%";

            SettingsInformation.UIScale = (float)processedValue;
        }

        public void OnAudioScaleUpdated()
        {
            float processedValue = (float)Math.Round(audioScaleSlider.value, 2);
            audioScaleText.text = $"{processedValue * 100}%";
            SettingsInformation.AudioVolume = processedValue;
        }
        
        public void OnResetSettingsButtonPressed()
        {
            SettingsInformation = Settings.DefaultSettings;
            InitialiseSettingsUI();
        }

        public void OnResumeButtonPressed()
        {
            DisplaySettingsUI(false);
            
            UI_Menu menu = SCR_GeneralManager.UIManager.FindUIObject<UI_Menu>();
            menu?.SetButtonInteractability(true);

        }

        #endregion

    }
}
