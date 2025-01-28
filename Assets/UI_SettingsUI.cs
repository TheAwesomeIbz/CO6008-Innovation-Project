using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.Rendering;
using static Settings;

namespace UnityEngine.UI
{
    public class UI_SettingsUI : MonoBehaviour
    {
        [field : SerializeField] public bool SettingsMenuEnabled { get; private set; }
        [field: SerializeField] public SettingsInformation settingsInformation { get; private set; }

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
            settingsInformation = settings.SettingsInformation;
            InitialiseSettingsUI();
            SettingsMenuEnabled = false;
            settingsParentObject.gameObject.SetActive(false);

        }

        private void InitialiseSettingsUI()
        {
            gameModeDropdown.value = (int)settingsInformation.GameMode;
            textSpeedDropdown.value = (int)settingsInformation.TextSpeed;
            uiScaleSlider.value = (settingsInformation.UIScale - 0.75f) * 2f;
            audioScaleSlider.value = settingsInformation.AudioVolume;
        }

        public void DisplaySettingsUI(bool activity)
        {
            SettingsMenuEnabled = activity;
            OnSettingsMenuPressed();
        }
        public void OnGameModeUpdated()
        {
            settingsInformation.GameMode = (Settings.SettingOption)gameModeDropdown.value;
        }

        public void OnTextSpeedUpdated()
        {
            settingsInformation.TextSpeed = (Settings.SettingOption)textSpeedDropdown.value;
        }

        public void OnUIScaleUpdated()
        {
            double processedValue = Math.Round((uiScaleSlider.value / 2f) + 0.75f, 2); 
            uiScaleText.text = $"{processedValue * 100}%";

            settingsInformation.UIScale = (float)processedValue;
        }

        public void OnAudioScaleUpdated()
        {
            float processedValue = (float)Math.Round(audioScaleSlider.value, 2);
            audioScaleText.text = $"{processedValue * 100}%";
            settingsInformation.AudioVolume = processedValue;
        }

        public void OnResetSettingsButtonPressed()
        {
            settingsInformation = new SettingsInformation
            {
                GameMode = SettingOption.DEFAULT,
                TextSpeed = SettingOption.DEFAULT,
                AudioVolume = 0.5f,
                UIScale = 1f
            };
            InitialiseSettingsUI();
        }

        public void OnResumeButtonPressed()
        {
            DisplaySettingsUI(false);
        }
        public void OnSettingsMenuPressed()
        {
            settingsParentObject.gameObject.SetActive(SettingsMenuEnabled);
            if (!SettingsMenuEnabled)
            {
                settingsInformation.SaveSettings();
            }
        }
        void Update()
        {
            if (Input.GetKeyDown(KeyCode.F4) && SCR_PlayerInputManager.PlayerControlsEnabled) {
                SettingsMenuEnabled = !SettingsMenuEnabled;
                OnSettingsMenuPressed();
            }
        }


    }
}
