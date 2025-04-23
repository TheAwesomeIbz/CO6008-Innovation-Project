using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using Dialogue;
using Entities.Player;
using Overworld;
using TMPro;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.Diagnostics;
using UnityEngine.SceneManagement;

namespace UnityEngine.UI
{
    /// <summary>
    /// 
    /// </summary>
    public class UI_LoadScene : MonoBehaviour
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

        public void LoadTransition(Vector2 playerPosition, Action onFadeToBlack = null,
            Action onTransitionFinished = null)
        {
            if (Loading) { return; }
            SCR_PlayerInputManager.PlayerControlsEnabled = false;
            
            StartCoroutine(TransitionCoroutine(playerPosition, onFadeToBlack, onTransitionFinished));
            
        }

        IEnumerator TransitionCoroutine(Vector2 playerPosition, Action onFadeToBlack = null,
            Action onTransitionFinished = null)
        {
            Loading = true;
            yield return FadeCoroutine(0, 1);
            
            onFadeToBlack?.Invoke();
            
            SCR_PlayerMovement playerTransform = SCR_GeneralManager.LevelManager.playerMovement ?? FindObjectOfType<SCR_PlayerMovement>();
            if (playerTransform) {
                playerTransform.GetComponentInChildren<SCR_PlayerInteraction>().UpdateCollider();
                playerTransform.Rigidbody2D.position = playerPosition;
                SCR_PlayerShooting playerShooting = SCR_GeneralManager.LevelManager.playerMovement.GetComponent<SCR_PlayerShooting>();
                playerShooting?.UpdateTargetPosition(playerPosition);
            }
            
            yield return FadeCoroutine(1, 0);
            _loadingImage.gameObject.SetActive(false);
            SCR_PlayerInputManager.PlayerControlsEnabled = true;
            onTransitionFinished?.Invoke();
            Loading = false;
        }

        
        IEnumerator TransitionCoroutine(TransitionProperties transitionProperties)
        {
            
            //if (!SceneManager.GetSceneByName(transitionProperties.SceneName).IsValid())
            //{
            //    Selectable[] allSelectables = FindObjectsOfType<Selectable>();
            //    foreach (Selectable selectable in allSelectables)
            //    {
            //        selectable.interactable = false;
            //    }

            //    SCR_GeneralManager.UIManager.FindUIObject<SCR_DialogueManager>().DisplayDialogue(
            //        DialogueObject.CreateDialogue($"The scene ({transitionProperties.SceneName}) is not a valid scene within the game.",
            //        "This scene will not be loaded."), OnDialogueEnd: () =>
            //        {
            //            foreach (Selectable selectable in allSelectables){
            //                selectable.interactable = true;
            //            }
            //        });

            //    yield break;
            //}

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
            SCR_PlayerInputManager.PlayerControlsEnabled = transitionProperties.EnablePlayerControls;
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

            public bool EnablePlayerControls { get; set; }
        }
    }

}

