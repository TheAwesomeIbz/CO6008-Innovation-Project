using System.Collections;
using System.Collections.Generic;
using Dialogue;
using Entities.Player;
using UnityEngine;
using UnityEngine.UI;

namespace Level.Tutorial
{
    public class SCR_TutorialGoalPost : MonoBehaviour
    {
        [SerializeField] SO_Item examinationTrophyItem;
        private void OnTriggerEnter2D(Collider2D collision)
        {
            if (!collision.GetType(out SCR_PlayerMovement playerMovement)) { return; }

            SCR_PlayerInputManager.PlayerControlsEnabled = false;
            playerMovement.StopAllCoroutines();
            playerMovement.BoxCollider2D.enabled = false;
            playerMovement.Rigidbody2D.velocity = Vector2.zero;
            
            
            SCR_GeneralManager.UIManager.FindUIObject<UI_LoadScene>().LoadScene(new UI_LoadScene.TransitionProperties
            {
                SceneName = "Overworld Map",
                OnTransitionFinished = () =>
                {
                    DialogueObject[] onTransitionDialogue = DialogueObject.CreateDialogue(
                        "This is the overworld map that the player can navigate.",
                        "There are various nodes on the map that the player can go to.",
                        "Use [WASD] or the [ARROW KEYS] to navigate to each point.",
                        "Press submit to interact with any interactable nodes. These will be denoted with text above them.",
                        "There are bosses you can fight also in this demo. Thank you for listening",
                        "Best of luck and enjoy this demo!"
                        );
                    
                    SCR_GeneralManager.UIManager.FindUIObject<SCR_DialogueManager>().DisplayDialogue(onTransitionDialogue, () =>
                    {

                        bool examinationCompleted = PersistentSettings.LoadSettings()?.ExaminationCompleted ?? false;

                        if (examinationCompleted && examinationTrophyItem &&
                        !SCR_GeneralManager.InventoryManager.Inventory.Contains(examinationTrophyItem))
                        {
                            SCR_GeneralManager.InventoryManager.Inventory.Add(examinationTrophyItem);
                        }
                        SavingOperations.SaveInformation();

                    });
                    
                }
            });
        }
    }
}
