using System.Collections;
using System.Collections.Generic;
using UnityEngine;


namespace UnityEngine.UI
{
    /// <summary>
    /// UI Class responsible for displaying information and functionality when the level is completed.
    /// </summary>
    public class UI_LevelComplete : MonoBehaviour
    {
        [Header("LEVEL UI PROPERTIES")]
        [SerializeField] GameObject _levelCompleteContentObject;
        string _sceneName;

        void Start()
        {
            _levelCompleteContentObject.SetActive(false);
        }

        /// <summary>
        /// Displays the Level completion UI
        /// </summary>
        /// <param name="sceneName"></param>
        public void DisplayUI(string sceneName)
        {
            //TODO : DISPLAY THE LEVEL COMPLETION TIME AND THE AMOUNT OF COLLECTIBLES OBTAINED
            LevelData levelData = SCR_GeneralManager.LevelManager.GetCurrentLevelData;
            _levelCompleteContentObject.SetActive(true);
            _sceneName = sceneName;
        }

        void Update()
        {
            //if the object is not enabled, then the level hasnt been completed so don't do anything
            if (!_levelCompleteContentObject.activeInHierarchy) { return; }

            //If any key is pressed, then begin the transition
            if (Input.anyKeyDown && !string.IsNullOrEmpty(_sceneName))
            {
                _levelCompleteContentObject.SetActive(false);
                SCR_GeneralManager.UIManager.FindUIObject<UI.UI_LoadScenes>().LoadScene(new UI_LoadScenes.TransitionProperties
                {
                    SceneName = SCR_GeneralManager.LevelManager.GetPreviousSceneName,
                    OnSceneLoaded = SCR_GeneralManager.LevelManager.OnOverworldSceneLoaded,
                    OnTransitionFinished = () => { SCR_PlayerInputManager.PlayerControlsEnabled = true; }
                });
                _sceneName = ""; //set to empty so any key cannot be spammed
            }
        }


    }

}