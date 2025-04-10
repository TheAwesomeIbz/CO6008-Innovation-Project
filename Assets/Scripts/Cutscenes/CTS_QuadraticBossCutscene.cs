using Dialogue;
using Entities.Boss;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Cutscenes
{
    public class CTS_QuadraticBossCutscene : CTS_BaseCutscene
    {
        [SerializeField] SCR_QuadraticBoss quadraticBoss;
        [SerializeField] DialogueObject[] introductionDialogue;
       

        private void Awake()
        {
            quadraticBoss = GetComponent<SCR_QuadraticBoss>();
            quadraticBoss.enabled = false;
            
        }

        private void OnDialogueEnd()
        {
            quadraticBoss.enabled = true;
        }

        public override void BeginCutscene()
        {
            SCR_GeneralManager.UIManager.FindUIObject<SCR_DialogueManager>().DisplayDialogue(introductionDialogue, OnDialogueEnd);
        }
    }

    public abstract class CTS_BaseCutscene : MonoBehaviour
    {
        public abstract void BeginCutscene();
    }
}
