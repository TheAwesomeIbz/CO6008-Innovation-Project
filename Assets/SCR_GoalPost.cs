using Entities.Player;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SCR_GoalPost : MonoBehaviour
{
    [Header("GOAL POST PROPERTIES")]
    [SerializeField] string _sceneName;
    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (!collision.GetType(out SCR_PlayerMovement playerMovement)) { return; }

        SCR_GeneralManager.UIManager.FindUIObject<UI_LevelComplete>().DisplayUI(_sceneName);
        print(Time.time);
    }
}
