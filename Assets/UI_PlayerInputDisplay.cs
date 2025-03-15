using Dialogue;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

namespace UnityEngine.UI
{
    public class UI_PlayerInputDisplay : MonoBehaviour
    {
        [Header("DISPLAY PROPERTIES")]
        [SerializeField] GameObject parentObject;
        [SerializeField] TextMeshProUGUI displayInstructions;
        string cachedUIText;
        private void Start()
        {
            SCR_DialogueManager.OnDialogueStartEvent += SCR_DialogueManager_OnDialogueStartEvent;
            SCR_DialogueManager.OnDialogueEndEvent += SCR_DialogueManager_OnDialogueEndEvent;

            UI_Menu.OnMenuToggled += UI_Menu_OnMenuToggled;
            HideUI();
        }

        private void UI_Menu_OnMenuToggled(bool obj)
        {
            if (obj) {
                cachedUIText = displayInstructions.text;
                HideUI();
            }
            else
            {
                if (!string.IsNullOrEmpty(cachedUIText)) { DisplayUI(cachedUIText); }
            }
        }

        private void SCR_DialogueManager_OnDialogueStartEvent(DialogueObject[] obj)
        {
            cachedUIText = displayInstructions.text;
            HideUI();
            
        }
        private void SCR_DialogueManager_OnDialogueEndEvent()
        {
            if (!string.IsNullOrEmpty(cachedUIText)) { DisplayUI(cachedUIText); } 
        }

        

        public void DisplayUI(string text = "<color=green>[ENTER]</color> INTERACT")
        {
            parentObject.SetActive(true);
            displayInstructions.text = text;
        }

        public void HideUI()
        {
            parentObject.SetActive(false);
            displayInstructions.text = "";
        }

        private void OnDestroy()
        {
            SCR_DialogueManager.OnDialogueStartEvent -= SCR_DialogueManager_OnDialogueStartEvent;
            SCR_DialogueManager.OnDialogueEndEvent -= SCR_DialogueManager_OnDialogueEndEvent;
            UI_Menu.OnMenuToggled -= UI_Menu_OnMenuToggled;
        }
    }

}