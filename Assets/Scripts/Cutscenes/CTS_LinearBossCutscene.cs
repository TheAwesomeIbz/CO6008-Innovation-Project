using Dialogue;
using Entities.Boss;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Cutscenes
{
    public class CTS_LinearBossCutscene : MonoBehaviour
    {
        [SerializeField] DialogueObject[] introductionDialogue;
        SCR_LinearBoss linearBoss;

        private void Awake()
        {
            linearBoss = GetComponent<SCR_LinearBoss>();
            linearBoss.enabled = false;
        }
        void Start()
        {
            SCR_GeneralManager.UIManager.FindUIObject<SCR_DialogueManager>().DisplayDialogue(introductionDialogue, OnDialogueEnd);
        }

        private void OnDialogueEnd()
        {
            linearBoss.enabled = true;
        }

        // Update is called once per frame
        void Update()
        {

        }
    }
}
