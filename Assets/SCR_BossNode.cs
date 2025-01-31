using System.Collections;
using System.Collections.Generic;
using Cutscenes;
using UnityEngine;
using UnityEngine.UI;

namespace Overworld
{
    public class SCR_BossNode : SCR_LevelNode
    {
        public override void Interact(object playerObject)
        {
            if (string.IsNullOrEmpty(sceneName)) { return; }

            SCR_GeneralManager.LevelManager.OnLevelTransition(LevelData, playerObject as SCR_PlayerOverworldMovement);
            SCR_GeneralManager.UIManager.FindUIObject<UI_LoadScenes>().LoadScene(new UI_LoadScenes.TransitionProperties
            {
                SceneName = sceneName,
                OnSceneLoaded = SCR_GeneralManager.LevelManager.OnSceneLoaded,
                OnTransitionFinished = OnTransitionEnded
            });
            
        }

        private void OnTransitionEnded()
        {
            FindObjectOfType<CTS_BaseCutscene>()?.BeginCutscene();
        }
    }

}