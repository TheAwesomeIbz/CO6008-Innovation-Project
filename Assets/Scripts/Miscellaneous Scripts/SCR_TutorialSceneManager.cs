using Dialogue;
using Entities;
using Entities.Player;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Level
{
    public class SCR_TutorialSceneManager : MonoBehaviour
    {
        [Header("First NPC Properties")]
        [SerializeField] SCR_DialogueNPC introductionNPC;
        [SerializeField] GameObject firstBarrier;

        [Header("Second NPC Properties")]
        [SerializeField] SCR_DialogueNPC explanationNPC;
        [SerializeField] GameObject secondBarrier;

        [Header("Third NPC Properties")]
        [SerializeField] SCR_DialogueNPC weaponNPC;
        [SerializeField] SO_Item weaponItem;
        [SerializeField] GameObject thirdBarrier;

        [Header("Practice Shooting Properties")]
        [SerializeField] PracticeShootingProperties practiceShootingProperties;
        

        void Awake()
        {
            SCR_PlayerMovement playerMovement = FindObjectOfType<SCR_PlayerMovement>();
            SCR_PlayerShooting playerShooting = playerMovement.GetComponent<SCR_PlayerShooting>();
            playerShooting.enabled = false;
            playerShooting.SetTargetDisplay(false);

            introductionNPC.SetDialogueFinishAction(On1stNPCDialogueFinished);
            explanationNPC.SetDialogueFinishAction(On2ndNPCDialogueFinish);
            weaponNPC.SetDialogueFinishAction(On3rdNPCDialogueFinish);
        }

        private void Start()
        {
            practiceShootingProperties.SetTargetsActivity(false);
        }

        private void On1stNPCDialogueFinished()
        {
            firstBarrier.SetActive(false);
        }

        private void On2ndNPCDialogueFinish()
        {
            secondBarrier.SetActive(false);
            
        }

        private void On3rdNPCDialogueFinish()
        {
            thirdBarrier.gameObject.SetActive(false);
            practiceShootingProperties.SetTargetsActivity(true);

            SCR_PlayerMovement playermovement = FindObjectOfType<SCR_PlayerMovement>();
            SCR_PlayerShooting playerShooting = playermovement.GetComponent<SCR_PlayerShooting>();
            playerShooting.enabled = true;
            playerShooting.SetTargetDisplay(true);

            if (SCR_GeneralManager.InventoryManager.AddItemWithDialogue(weaponItem))
            {
                SCR_GeneralManager.UIManager.FindUIObject<SCR_DialogueManager>().DisplayDialogue(DialogueObject.CreateDialogue("It seems you already have this weapon!", "No point in me giving you another one haha!"));
            }
        }

        public void UpdateTargets()
        {
            practiceShootingProperties.destroyedTargets++;
            if (practiceShootingProperties.destroyedTargets >= practiceShootingProperties.practiceTargets.Length)
            {
                practiceShootingProperties.barrierObject.SetActive(false);
                SCR_GeneralManager.UIManager.FindUIObject<SCR_DialogueManager>().DisplayDialogue(
                    DialogueObject.CreateDialogue("Congratulations! You have successfully shot all the targets!", "The tutorial is now completed. Please progress further to exit the tutorial!"));
            }
        }

        [Serializable] class PracticeShootingProperties
        {
            public SCR_PracticeTarget[] practiceTargets;
            public int destroyedTargets;
            public GameObject barrierObject;

            public void SetTargetsActivity(bool state)
            {
                foreach (SCR_PracticeTarget practiceTarget in practiceTargets)
                {
                    practiceTarget.gameObject.SetActive(state);
                }
            }
        }
    }

}