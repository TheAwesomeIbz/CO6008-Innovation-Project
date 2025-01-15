using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.EventSystems;

namespace UnityEngine.UI.Title
{
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


        private void ParseInputText(string text)
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

        private void OnValidNameEntered(string text)
        {
            SCR_GeneralManager.UIManager.FindUIObject<UI.UI_LoadScenes>().LoadScene(new UI_LoadScenes.TransitionProperties
            {
                SceneName = _sceneName,
                OnTransitionFinished = OnSceneUnloaded,
            });

            SCR_GeneralManager.Instance.LoadPlayerData(new PlayerData(text));
            SavingOperations.SaveInformation();
        }

        private void OnSceneUnloaded()
        {
            SCR_PlayerInputManager.PlayerControlsEnabled = true;
        }
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
                ParseInputText(_inputField.text);
            }
        }


    }
}
