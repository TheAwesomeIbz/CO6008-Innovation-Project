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

        SCR_PlayerInputManager.PlayerControlsEnabled = false;
        playerMovement.StopAllCoroutines();
        playerMovement.BoxCollider2D.enabled = false;
        playerMovement.Rigidbody2D.velocity = Vector2.zero;
        SCR_GeneralManager.LevelManager.OnLevelCompleted();
        
        SCR_GeneralManager.UIManager.FindUIObject<UI_LevelComplete>().DisplayUI();
    }
}
