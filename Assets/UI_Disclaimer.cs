using System.Collections;
using System.Collections.Generic;
using System.IO;
using TMPro;
using UnityEngine;
using UnityEngine.UI;


namespace UnityEngine.UI.Title
{
    public class UI_Disclaimer : MonoBehaviour
    {
        string[] _splashScreenText;
        int _textIndex;
        [SerializeField] TextMeshProUGUI _textObject;

        const string screenTextPath = "Assets/Miscellaneous/Splash Screen Text/SplashScreen.txt";
        void Start()
        {
            _textIndex = 0;
            _splashScreenText = JsonUtility.FromJson<ScreenText>(File.ReadAllText(screenTextPath))?._textValues ?? null;
            _textObject.color = new Color(_textObject.color.r, _textObject.color.g, _textObject.color.b, 0);
            SCR_PlayerInputManager.PlayerControlsEnabled = false;

            StartCoroutine(DisplayTextCoroutine());
        }

        IEnumerator DisplayTextCoroutine()
        {
            bool canPressSubmit = false;
            SCR_PlayerInputManager.PlayerControlsEnabled = true;

            while (_textIndex < _splashScreenText.Length)
            {
                _textObject.text = _splashScreenText[_textIndex];
                LeanTween.value(0, 1, 0.5f).setOnUpdate((value) => {
                    _textObject.color = new Color(_textObject.color.r, _textObject.color.g, _textObject.color.b, value);
                }
                ).setOnComplete(() =>
                {
                    canPressSubmit = true;
                });

                
                yield return new WaitUntil(() => { return canPressSubmit && SCR_GeneralManager.PlayerInputManager.Submit.IsPressed(); });

                LeanTween.value(1, 0, 0.5f).setOnUpdate((value) =>
                     _textObject.color = new Color(_textObject.color.r, _textObject.color.g, _textObject.color.b, value)
                ).setOnComplete(() =>
                {
                    canPressSubmit = false;
                });
                yield return new WaitUntil(() => { return !canPressSubmit; });
                _textIndex++;
            }

            SCR_GeneralManager.UIManager.FindUIObject<UI_LoadScenes>().LoadScene(new UI_LoadScenes.TransitionProperties
            {
                SceneName = "Title Scene"
            });

        }

        [System.Serializable]
        class ScreenText
        {
            public string[] _textValues;

            public ScreenText(string[] textValues)
            {
                _textValues = textValues;
            }
        }
    }
}

