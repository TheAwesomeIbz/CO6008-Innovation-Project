using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace Overworld
{
    public class SCR_LevelNode : SCR_GraphNode, iInteractable
    {
        [Header("LEVEL NODE PROPERIES")]
        [SerializeField] protected string sceneName;
        [field : SerializeField] public LevelData LevelData { get; private set; }

        public bool Interactable => true;

        public void InitializeLevelNode(LevelData levelData) => LevelData = levelData;

        public override bool ConditionalNode()
        {
            return LevelData.LevelCompleted;
        }

        public override void OnPlayerMoved(SCR_PlayerOverworldMovement playerOverworldMovement)
        {
            Debug.Log("HIDE LEVEL DESCRIPTION");
        }

        public override void OnPlayerLanded(SCR_PlayerOverworldMovement playerOverworldMovement)
        {
            Debug.Log("OPEN LEVEL DESCRIPTION");
        }

        public void UpdateLevelData(LevelData levelData) => LevelData = levelData;

        public virtual void Interact(object playerObject)
        {
            if (string.IsNullOrEmpty(sceneName)) { return; }

            SCR_GeneralManager.LevelManager.OnLevelTransition(LevelData, playerObject as SCR_PlayerOverworldMovement);
            SCR_GeneralManager.UIManager.FindUIObject<UI_LoadScenes>().LoadScene(new UI_LoadScenes.TransitionProperties
            {
                SceneName = sceneName,
                EnablePlayerControls = true,
                OnSceneLoaded = SCR_GeneralManager.LevelManager.OnSceneLoaded,
        });
        }
    }

}