using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.EventSystems;

namespace UnityEngine.UI.Title
{
    /// <summary>
    /// UI Monobehaviour that deals with the name scene, allowing the player to name themselves, and create a save file for them
    /// </summary>
    public class UI_NamingScene : MonoBehaviour
    {
        [Header("NAME SCENE PROPERETIES")]
        [SerializeField] private string _sceneName = "Overworld Scene";
        [SerializeField] TMP_InputField _inputField;
        [SerializeField] TextMeshProUGUI _errorText;

        void Start()
        {
            _errorText.text = "";
        }

        /// <summary>
        /// Validates the input entered, to ensure that it is a valid name string
        /// </summary>
        /// <param name="text"></param>
        private void InputValidation(string text)
        {
            bool validInput = true;
            if (string.IsNullOrWhiteSpace(text))
            {
                _errorText.text = "Please enter in a valid name!";
                validInput = false;
            }

            if (text.Length < 3)
            {
                _errorText.text = "Please enter in a <color=red>longer name</color>!";
                validInput = false;
            }

            if (text.Length > 15)
            {
                _errorText.text = "Please enter in a <color=red>shorter name</color>!";
                validInput = false;
            }

            if (!validInput)
            {
                EventSystem.current.SetSelectedGameObject(_inputField.gameObject);
                return;
            }

            OnValidNameEntered(text);
        }

        /// <summary>
        /// Called when a valid name is entered. The game will then transition the user to the next scene.
        /// </summary>
        /// <param name="text"></param>
        private void OnValidNameEntered(string text)
        {
            SCR_GeneralManager.UIManager.FindUIObject<UI_LoadScenes>().LoadScene(new UI_LoadScenes.TransitionProperties
            {
                SceneName = _sceneName,
                OnTransitionFinished = OnTransitionFinished,
            });

            SCR_GeneralManager.Instance.LoadPlayerData(new PlayerData(text));
            SavingOperations.SaveInformation();
        }

        private void OnTransitionFinished()
        {
            SCR_PlayerInputManager.PlayerControlsEnabled = true;
        }

        /// <summary>
        /// Called every time the input field is updated
        /// </summary>
        public void OnStringChanged()
        {
            if (string.IsNullOrWhiteSpace(_inputField.text))
            {
                _errorText.text = "Please enter in a valid name!";
                return;
            }

            _errorText.text = _inputField.text.Length > 15 ? "Please enter in a <color=red>shorter name</color>!" : "";
        }

        void Update()
        {
            if (SCR_GeneralManager.PlayerInputManager.Submit.PressedThisFrame())
            {
                InputValidation(_inputField.text);
            }
        }


    }
}
