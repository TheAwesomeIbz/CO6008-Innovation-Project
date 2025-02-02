using Dialogue;
using Entities.Player;
using Overworld;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

namespace UnityEngine.UI
{
    public class UI_GameOver : MonoBehaviour
    {
        [SerializeField] GameObject parentContentObject;
        [SerializeField] TextMeshProUGUI descriptionText;
        [SerializeField] List<GameObject> buttonObject;
        void Start()
        {
            InitialiseScreen();
            SCR_PlayerMovement.OnPlayerDefeated += OnPlayerDefeated;
        }

        private void OnPlayerDefeated(SCR_PlayerMovement obj)
        {
            SCR_GeneralManager.Instance.PlayerData.AmountOfDeaths += 1;
            StartCoroutine(OnGameOverCoroutine());
        }

        private void InitialiseScreen()
        {
            parentContentObject.SetActive(false);
            descriptionText.gameObject?.SetActive(false);

            foreach (GameObject button in buttonObject)
            {
                button.SetActive(false);
            }

        }

        IEnumerator OnGameOverCoroutine()
        {
            descriptionText.text = $"TIMES FAILED : {SCR_GeneralManager.Instance.PlayerData.AmountOfDeaths}";

            parentContentObject.SetActive(true);
            yield return new WaitForSeconds(0.5f);
            descriptionText.gameObject.SetActive(true);
            yield return new WaitForSeconds(0.25f);
            foreach (GameObject button in buttonObject)
            {
                button.SetActive(true);
            }
        }

        public void OnRetryButtonPressed()
        {
            SCR_GeneralManager.UIManager.FindUIObject<UI_LoadScene>().LoadScene(new UI_LoadScene.TransitionProperties
            {
                SceneName = SceneManagement.SceneManager.GetActiveScene().name,
                EnablePlayerControls = true,
                OnTransitionFinished = OnTransitionFinished,
            });
        }

        public void OnContinueButtonPressed()
        {
            SCR_GeneralManager.UIManager.FindUIObject<UI_LoadScene>().LoadScene(new UI_LoadScene.TransitionProperties
            {
                SceneName = SCR_GeneralManager.LevelManager.GetPreviousSceneName,
                EnablePlayerControls = true,
                OnSceneLoaded = SCR_GeneralManager.LevelManager.OnOverworldSceneLoaded
            });
        }

        private void OnTransitionFinished()
        {
            FindObjectOfType<Cutscenes.CTS_BaseCutscene>()?.BeginCutscene();
        }

        private void OnDisable()
        {
            SCR_PlayerMovement.OnPlayerDefeated -= OnPlayerDefeated;
        }
    }
}
