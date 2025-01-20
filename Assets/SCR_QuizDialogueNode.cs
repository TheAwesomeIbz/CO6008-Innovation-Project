using Dialogue;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Overworld
{
    /// <summary>
    /// Overworld node that can quiz the player, and may reqard them or grant access to other nodes
    /// </summary>
    public class SCR_QuizDialogueNode : SCR_ChoiceDialogueNode, IQuizInterface
    {
        [Header("QUIZ DIALOGUE PROPERTIES")]
        [Tooltip("Depending on whether this is toggled or not, the question can only be answered once")]
        [SerializeField] bool onlyOneChance;

        /// <summary>
        /// The inspector and other external fields will represent the select choice from 1 to 3 instead of starting from 0. Make sure to always subtract 1 when determining the integer location.
        /// </summary>
        [SerializeField] [Range(1,3)] int correctChoice;

        [Header("REWARD PROPERTIES")]
        [SerializeField] SO_Item rewardItem;

        public override bool ConditionalNode()
        {
            return correctChoice - 1 == savableChoice.SelectedChoice;
        }
        public int CorrectChoice => correctChoice - 1;
        public bool OnlyOneChance => onlyOneChance;

        public void OnCorrectChoiceMade()
        {
            if (rewardItem){
                SCR_GeneralManager.InventoryManager.AddItem(rewardItem);
            }
            print("ON RIGHT");
        }

        public void OnIncorrectChoiceMade()
        {
            print("ON WRONG");
        }
    }
}
