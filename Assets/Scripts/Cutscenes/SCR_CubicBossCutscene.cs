using System.Collections;
using System.Collections.Generic;
using Dialogue;
using UnityEngine;

namespace Cutscenes
{
    public class SCR_CubicBossCutscene : CTS_BaseCutscene
    {
       [SerializeField] private GameObject cubicBossUI;
        void Start()
        {
            cubicBossUI.SetActive(false);
        }

        public override void BeginCutscene()
        {
            SCR_GeneralManager.UIManager.FindUIObject<SCR_DialogueManager>().DisplayDialogue(DialogueObject.CreateDialogue(
                "This is the stage of the game, what your whole adventure has culminated into <NAME>.",
                "You will be tasked through various steps to solve this cubic equation.", 
                "Formulae will be displayed to guide you through the essential steps to defeat this boss.",
                "Best of luck with everything!"), 
                OnDialogueEnd: () =>
            {
                cubicBossUI.SetActive(true);
            });
        }
    }
}
