using Entities.Player;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class SCR_GoalPost : MonoBehaviour
{
    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (!collision.GetType(out SCR_PlayerMovement playerMovement)) { return; }

        SCR_GeneralManager.LevelManager.OnLevelCompleted();
        SCR_GeneralManager.UIManager.FindUIObject<UI_LevelComplete>().DisplayUI();
    }
}
