using System;
using Entities;
using Entities.Player;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Level
{
    public class SCR_LevelCollectable : MonoBehaviour
    {
        [field: Header("LEVEL COLLECTABLE PROPERTIES")]
        [field : SerializeField] public LevelCollectable LevelCollectable { get; private set; }
        [SerializeField] SO_Item piTrophyItem;
        

        public void CollectItem()
        {
            if (SCR_GeneralManager.LevelManager.GetCurrentLevelData == null) { return; }
            LevelCollectable existingLevelCollectable = SCR_GeneralManager.LevelManager.GetCurrentLevelData.LevelCollectablesObtained.Find(lvl => lvl.CollectableID == LevelCollectable.CollectableID);

            if (!existingLevelCollectable?.CollectableObtained ?? false) {
                existingLevelCollectable.Collect();

                //add trophy to inventory if the item doesnt exist already
                if (SCR_GeneralManager.LevelManager.GetCollectableCount == 6 && 
                    !SCR_GeneralManager.InventoryManager.Inventory.Contains(piTrophyItem))
                {
                    SCR_GeneralManager.InventoryManager.AddItem(piTrophyItem);
                }
            }
            gameObject.SetActive(false);
        }
        
        
    }
}
