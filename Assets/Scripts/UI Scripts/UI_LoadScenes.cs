using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.Diagnostics;
using UnityEngine.SceneManagement;
using UnityEngine.UIElements;
using static UnityEngine.UI.UI_LoadScenes;

namespace UnityEngine.UI
{
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
            transitionProperties.OnSceneLoaded?.Invoke();

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
                yield return SceneManager.LoadSceneAsync(transitionProperties.SceneName);
            }
            

            yield return FadeCoroutine(1, 0);

            transitionProperties.OnSceneUnloaded?.Invoke();
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
        
        void Update()
        {
            //if (Input.GetKeyUp(KeyCode.Escape))
            //{
            //    StartCoroutine(TransitionCoroutine(new TransitionProperties { SceneName = "BossFight_Linear" }));
            //}
        }


        public class TransitionProperties
        {
            public string SceneName { get; set; }
            public Action OnSceneLoaded { get; set; }
            public Action OnSceneUnloaded { get; set; }
        }
    }

}

