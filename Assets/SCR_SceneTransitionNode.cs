using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace Overworld
{
    /// <summary>
    /// Node that instantly warps the player on contact
    /// </summary>
    public class SCR_SceneTransitionNode : SCR_GraphNode
    {
        [Header("SCENE TRANSITION NODE PROPERTIES")]
        [SerializeField] string sceneName;

        public override void OnPlayerLanded(SCR_PlayerOverworldMovement playerOverworldMovement)
        {
            SCR_GeneralManager.LevelManager.CachePlayerProperties(playerOverworldMovement);
            SCR_GeneralManager.UIManager.FindUIObject<UI_LoadScenes>().LoadScene(new UI_LoadScenes.TransitionProperties
            {
                SceneName = sceneName,
                EnablePlayerControls = true,
            });
        }
    }
}
