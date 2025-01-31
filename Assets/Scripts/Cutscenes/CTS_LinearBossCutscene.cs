using Dialogue;
using Entities.Boss;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Cutscenes
{
    public class CTS_LinearBossCutscene : CTS_BaseCutscene
    {
        [SerializeField] DialogueObject[] introductionDialogue;
        SCR_LinearBoss linearBoss;

        private void Awake()
        {
            linearBoss = GetComponent<SCR_LinearBoss>();
            linearBoss.enabled = false;
        }

        private void OnDialogueEnd()
        {
            linearBoss.enabled = true;
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
