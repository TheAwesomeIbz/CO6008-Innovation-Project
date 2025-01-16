using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.Diagnostics;
using UnityEngine.SceneManagement;

namespace UnityEngine.UI
{
    /// <summary>
    /// 
    /// </summary>
    public class UI_LoadScenes : MonoBehaviour
    {
        [field : Header("LOAD SCENE PROPERTIES")]
        [field : SerializeField] public bool Loading { get; private set; }
        [SerializeField] RawImage _loadingImage;

        private void Start()
        {
            _loadingImage.gameObject.SetActive(false);
        }
        public void LoadScene(TransitionProperties transitionProperties)
        {
            if (Loading) { return; }
            SCR_PlayerInputManager.PlayerControlsEnabled = false;
            StartCoroutine(TransitionCoroutine(transitionProperties));
        }

        
        IEnumerator TransitionCoroutine(TransitionProperties transitionProperties)
        {
            
            Loading = true;
            yield return FadeCoroutine(0, 1);

            if (string.IsNullOrEmpty(transitionProperties.SceneName))
            {

            }
            else
            {
                AsyncOperation asyncOperation = SceneManager.LoadSceneAsync(transitionProperties.SceneName);
                while(!asyncOperation.isDone)
                {
                    yield return null;
                }
                transitionProperties.OnSceneLoaded?.Invoke();
            }
            
            
            yield return FadeCoroutine(1, 0);
            _loadingImage.gameObject.SetActive(false);

            transitionProperties.OnTransitionFinished?.Invoke();
            Loading = false;
        }

        IEnumerator FadeCoroutine(float startValue, float endValue)
        {
            _loadingImage.color = new Color(_loadingImage.color.r, _loadingImage.color.g, _loadingImage.color.b, startValue);
            _loadingImage.gameObject.SetActive(true);
            float value = startValue;

            if (value > endValue)
            {
                while (value > endValue)
                {
                    _loadingImage.color = new Color(_loadingImage.color.r, _loadingImage.color.g, _loadingImage.color.b, value);
                    value -= Time.deltaTime;
                    yield return new WaitForEndOfFrame();
                }
                
            }
            else
            {
                while (value < endValue)
                {
                    _loadingImage.color = new Color(_loadingImage.color.r, _loadingImage.color.g, _loadingImage.color.b, value);
                    value += Time.deltaTime;
                    yield return new WaitForEndOfFrame();
                }
                
            }
            
        }


        /// <summary>
        /// Model class used to store information about transitioning to another scene
        /// </summary>
        public class TransitionProperties
        {
            /// <summary>
            /// The name of the scene to be transitioned to
            /// </summary>
            public string SceneName { get; set; }

            /// <summary>
            /// The Action invoked on the first frame the game transitioned to the consecutive scene.
            /// </summary>
            public Action OnSceneLoaded { get; set; }

            /// <summary>
            /// The Action invoked when the fade transition completely finished.
            /// </summary>
            public Action OnTransitionFinished { get; set; }
        }
    }

}

