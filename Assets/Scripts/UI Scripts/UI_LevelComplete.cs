using System.Collections;
using System.Collections.Generic;
using TMPro;
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
        [SerializeField] TextMeshProUGUI descriptionText;
        [SerializeField] GameObject continueButton;
        
        void Start()
        {
            _levelCompleteContentObject.SetActive(false);
            descriptionText.gameObject.SetActive(false);
            continueButton.gameObject.SetActive(false);
        }

        /// <summary>
        /// Displays the Level completion UI
        /// </summary>
        /// <param name="sceneName"></param>
        public void DisplayUI()
        {
            //TODO : DISPLAY THE LEVEL COMPLETION TIME AND THE AMOUNT OF COLLECTIBLES OBTAINED
            StopAllCoroutines();
            StartCoroutine(LevelCompleteCoroutine());
        }

        private IEnumerator LevelCompleteCoroutine()
        {
            float levelCompletedTime = SCR_GeneralManager.LevelManager.GetCurrentLevelData.LevelCompletedTime;
            string ss = (levelCompletedTime % 60).ToString("00");
            string mm = ((levelCompletedTime % 60) / 60).ToString("00");
            descriptionText.text = $"TIME:<color=green>{mm}:{ss}</color>";
            
            _levelCompleteContentObject.SetActive(true);
            yield return new WaitForSeconds(0.25f);
            descriptionText.gameObject.SetActive(true);
            yield return new WaitForSeconds(0.25f);
            continueButton.gameObject.SetActive(true);
        }

        public void OnContinueButtonPressed()
        {
            _levelCompleteContentObject.SetActive(false);
            descriptionText.gameObject.SetActive(false);
            continueButton.gameObject.SetActive(false);

            SCR_GeneralManager.UIManager.FindUIObject<UI.UI_LoadScene>().LoadScene(new UI_LoadScene.TransitionProperties
            {
                SceneName = SCR_GeneralManager.LevelManager.GetPreviousSceneName,
                OnSceneLoaded = SCR_GeneralManager.LevelManager.OnOverworldSceneLoaded,
                OnTransitionFinished = SCR_GeneralManager.LevelManager.OnTransitionFinished,
                EnablePlayerControls = !SCR_GeneralManager.LevelManager.LevelFirstCompleted,
            });
        }

        

    }

}