using System;
using System.Collections;
using System.Collections.Generic;
using Dialogue;
using Entities;
using UnityEngine;

namespace Level
{
    public class SCR_EnemyProgression : MonoBehaviour
    {
        [Header("ENEMY PROGRESSION PROPERTIES")]
        private int enemyProgressionCounter = 0;
        [SerializeField] [Range(2,10)]private int enemyProgressionLimit = 2;

        [Header("ON PROGRESSION COMPLETED PROPERTIES")] 
        [SerializeField] private GameObject barrierObject;
        private void Start()
        {
            enemyProgressionCounter = 0;
        }

        public void UpdateEnemyProgression()
        {
            if (!SCR_GeneralManager.Instance) { return;}
            
            enemyProgressionCounter++;
            if (enemyProgressionCounter >= enemyProgressionLimit)
            {
                DialogueObject[] completionDialogue = DialogueObject.CreateDialogue(
                    "<NAME>, you have successfully defeated all the enemies in the area!",
                    "You can now proceed onto the next section!");
                SCR_GeneralManager.UIManager.FindUIObject<SCR_DialogueManager>().DisplayDialogue(completionDialogue);
                
                barrierObject?.gameObject.SetActive(false);
            }
        }
    }
}
