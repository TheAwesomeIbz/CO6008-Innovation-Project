using Entities;
using Entities.Player;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Level
{
    public class SCR_LevelCollectable : BaseCollectable
    {
        [field: Header("LEVEL COLLECTABLE PROPERTIES")]
        [field : SerializeField] public LevelCollectable LevelCollectable { get; private set; }
        protected override void OnPlayerCollided(SCR_PlayerMovement playerMovement)
        {
            if (SCR_GeneralManager.LevelManager.GetCurrentLevelData == null) { return; }

            LevelCollectable existingLevelCollectable = SCR_GeneralManager.LevelManager.GetCurrentLevelData.LevelCollectablesObtained.Find(lvl => lvl.CollectableID == LevelCollectable.CollectableID);
            
            if (!existingLevelCollectable?.CollectableObtained ?? false)
            {
                existingLevelCollectable.Collect();
            }
            gameObject.SetActive(false);
        }
    }
}
