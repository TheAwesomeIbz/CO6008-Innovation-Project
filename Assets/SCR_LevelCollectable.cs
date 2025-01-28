using Entities;
using Entities.Player;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Level
{
    public class SCR_LevelCollectable : BaseCollectable
    {
        protected override void OnPlayerCollided(SCR_PlayerMovement playerMovement)
        {
            if (SCR_GeneralManager.LevelManager.GetCurrentLevelData == null) { return; }
            if (SCR_GeneralManager.LevelManager.GetCurrentLevelData.LevelCollectablesObtained.Contains(name)) { return; }

            SCR_GeneralManager.LevelManager.GetCurrentLevelData.LevelCollectablesObtained.Add(name);
            gameObject.SetActive(false);
        }
    }
}
