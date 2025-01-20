using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using TMPro;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.UI;


namespace UnityEngine.UI.Title
{
    /// <summary>
    /// UI Class to be used in Disclaimer Scene to show all information necessary for the project.
    /// </summary>
    public class UI_Disclaimer : MonoBehaviour
    {
        
        [Header("SPLASH SCREEN PROPERTIES")]
        [SerializeField] TextMeshProUGUI _textObject;
        [SerializeField] [TextArea(4, 4)] string[] _disclaimer;
        int _textIndex;

        void Start()
        {
            _textIndex = 0;
            _textObject.color = new Color(_textObject.color.r, _textObject.color.g, _textObject.color.b, 0);
            SCR_PlayerInputManager.PlayerControlsEnabled = false;
            StartCoroutine(DisplayTextCoroutine());
        }

        /// <summary>
        /// Coroutine the runs and loops through each text in disclaimer object in a fading fashion.
        /// </summary>
        /// <returns></returns>
        IEnumerator DisplayTextCoroutine()
        {
            bool canPressSubmit = false;
            SCR_PlayerInputManager.PlayerControlsEnabled = true;


            Func<bool> inputPredicate = () => {
                return canPressSubmit &&
                (SCR_GeneralManager.PlayerInputManager.Submit.IsPressed() ||
                (SCR_PlayerInputManager.PlayerControlsEnabled && Input.GetMouseButton(0))
                );
            };
            //fades in and out of the nth string in disclaimer object until there aren't any more
            while (_textIndex < _disclaimer.Length)
            {
                _textObject.text = _disclaimer[_textIndex];
                //fade routine with leantween in
                LeanTween.value(0, 1, 0.5f).setOnUpdate((value) => {
                    _textObject.color = new Color(_textObject.color.r, _textObject.color.g, _textObject.color.b, value);
                }
                ).setOnComplete(() =>
                {
                    canPressSubmit = true;
                });
                
                yield return new WaitUntil(inputPredicate);

                //fade routine with leantween out
                LeanTween.value(1, 0, 0.5f).setOnUpdate((value) =>
                     _textObject.color = new Color(_textObject.color.r, _textObject.color.g, _textObject.color.b, value)
                ).setOnComplete(() =>
                {
                    canPressSubmit = false;
                });
                yield return new WaitUntil(() => { return !canPressSubmit; });
                _textIndex++;
            }

            SCR_GeneralManager.UIManager.FindUIObject<UI.UI_LoadScenes>().LoadScene(new UI_LoadScenes.TransitionProperties
            {
                SceneName = "Title Scene"
            });

        }
    }
}

